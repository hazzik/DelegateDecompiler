using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DelegateDecompiler.EntityFramework
{
    public class EFDecompileExpressionVisitor
        : DecompileExpressionVisitor
    {
        public EFDecompileExpressionVisitor()
            : base()
        {
        }

        public EFDecompileExpressionVisitor(string targetProviderName)
            : base(/*targetProviderName*/)
        {
        }

        private ObjectContext _objectContext { get; set; }
        private Assembly _dslTranslationResolver { get; set; }

        private Dictionary<Type, KeyValuePair<string, List<PropertyInfo>>> _entityToKeyEqualityCache = new Dictionary<Type, KeyValuePair<string, List<PropertyInfo>>>();
        private Dictionary<Type, string> _underlyingTables = new Dictionary<Type, string>();

        protected override Expression VisitConstant(ConstantExpression node)
        {
            #region Reference the ObjectContext from which to get models metadata

            if (typeof(ObjectQuery).IsAssignableFrom(node.Type))
            {
                var objectQuery = (node.Value as ObjectQuery);
                var targetContext = objectQuery.Context;
                if (_objectContext != null && _objectContext != targetContext)
                {
                    //TODO ?? allow for multiple object contexts or evaluate the expression as a constant against the target provider
                    throw new NotSupportedException("Cannot use different ObjectContext instances in the same query");
                }
                //_dslTranslationResolver = (_objectContext as IQueryable).Provider;
                _objectContext = targetContext;
            }

            #endregion

            #region Resolve entity constants by an object query filtered on primary key(s)

            else if (IsEntityType(node.Type) || (!typeof(Queryable).IsAssignableFrom(node.Type) && node.Type.IsGenericType && typeof(IEnumerable).IsAssignableFrom(node.Type) && IsEntityType(node.Type.GetGenericArguments().First())))
            {
                // first check whether the constant is an entity or a collection of constants
                bool isCollection = !IsEntityType(node.Type);

                //TODO try later to solve this case with performance in mind
                if (isCollection)
                {
                    if (node.Value == null) return node;
                    throw new NotSupportedException($"Yet unable to handle a collection of '{node.Type.GetGenericArguments().First()}' constant values. Use an ObjectQuery from the current context instead.");
                }
                // Build the base ObjectQuery
                var setName = _entityToKeyEqualityCache[node.Type].Key;
                var sqlQuery = $"OFTYPE( {setName}, [{node.Type.FullName}])";
                var createQueryMethod = _objectContext.GetType().GetMethod("CreateQuery"/*, BindingFlags.Public | BindingFlags.Instance, null, new Type[] { }, null*/).MakeGenericMethod(node.Type);
                var queryableType = typeof(ObjectQuery<>).MakeGenericType(node.Type);
                var testConstant = Activator.CreateInstance(queryableType, new object[] { sqlQuery, _objectContext, MergeOption.NoTracking });
                // Build a PrimaryKey match predicate
                var comparableParameter = Expression.Parameter(node.Type, "item");
                var matchEntityPredicate = Expression.Lambda(this.VisitBinary(Expression.Equal(comparableParameter, node)), comparableParameter);

                // Extend the ObjectQuery with .FirstOrDefault(predicate) call
                var firstOrDefaultMethod = typeof(Queryable).GetMethods().Where(m => m.Name == "FirstOrDefault" && m.GetParameters().Length == 2).First().MakeGenericMethod(node.Type);
                var constant = Expression.Call(null,
                        firstOrDefaultMethod,
                        Expression.Constant(createQueryMethod.Invoke(_objectContext, new object[] { sqlQuery, new ObjectParameter[0] }), queryableType),
                        matchEntityPredicate
                    );
                return constant;
            }

            #endregion

            return base.VisitConstant(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            #region EF annoying limitation lift : Constants resolution

            if (!ShouldDecompile(node.Member))
            {
                var propertyInfo = node.Member as PropertyInfo;
                if (propertyInfo != null)
                {
                    // We do not resolve navigation properties
                    var targetType = propertyInfo.PropertyType;
                    targetType = targetType.GetInterfaces().Contains(typeof(IEnumerable)) && targetType.IsGenericType ? targetType.GetGenericArguments().First() : targetType;
                    if (IsEntityType(targetType))
                    {
                        return node.Update(Visit(node.Expression));
                    }
                }
                var fieldInfo = node.Member as FieldInfo;
                var targetExpression = Visit(node.Expression) as ConstantExpression;
                if ((fieldInfo?.IsStatic ?? false) || (propertyInfo?.GetGetMethod()?.IsStatic ?? false) || targetExpression != null)
                {
                    var targetInstance = targetExpression?.Value;
                    var value =
                        (fieldInfo != null && (fieldInfo.IsStatic || targetInstance != null))
                        ? fieldInfo.GetValue(targetInstance)
                        : (propertyInfo != null && ((propertyInfo.GetGetMethod()?.IsStatic ?? false) || targetInstance != null))
                        ? propertyInfo.GetValue(targetInstance)
                        : null;
                    ConstantExpression result = Expression.Constant(value, node.Type);
                    if (value!=null && IsEntityType(value.GetType()))
                    {
                        return Visit(result);// node = node.Update(result);
                    }
                    return result;
                }
            }

            #endregion

            return base.VisitMember(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            return base.VisitMethodCall(node);
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            return base.VisitUnary(node);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            var operandType = new TypeHierarchyComparer().Compare(node.Left.Type, node.Right.Type) <= 0 ? node.Left.Type : node.Right.Type;

            #region EF annoying limitation lift : replace Entities' comparison Expressions by comparison of their PrimaryKey

            //TODO Expand the comparison on alternate keys (unique indexes)
            if ((node.NodeType == ExpressionType.Equal || node.NodeType == ExpressionType.NotEqual) && IsEntityType(operandType))
            {
                Expression translated = null;
                // First check whether left or right is null
                var left = (node.Left as ConstantExpression);
                var right = (node.Right as ConstantExpression);
                var compareWithNull = (left != null && left.Value == null) || (right != null && right.Value == null);
                if (compareWithNull)
                {
                    translated = Expression.Equal(left ?? base.Visit(node.Left), right ?? base.Visit(node.Right));
                }
                else
                {
                    // Otherwise compare the operands by unique keys
                    //TODO find and expand alternate keys
                    List<PropertyInfo> primaryKeyDefinition = GetPrimaryKeyComponents(operandType);
                    foreach (PropertyInfo keyPpty in primaryKeyDefinition)
                    {
                        Expression leftValue = left != null ? (Expression)Expression.Constant(keyPpty.GetValue(left.Value)) : Expression.MakeMemberAccess(node.Left, keyPpty);
                        Expression rightValue = right != null ? (Expression)Expression.Constant(keyPpty.GetValue(right.Value)) : Expression.MakeMemberAccess(node.Right, keyPpty); ;
                        var keyComponentEquality = Expression.Equal(leftValue, rightValue);
                        translated = translated == null ? keyComponentEquality : Expression.AndAlso(translated, keyComponentEquality);
                    }
                    //translated = base.Visit(translated);
                }
                if (node.NodeType == ExpressionType.NotEqual)
                {
                    translated = Expression.Not(translated);
                }
                return translated;
            }

            #endregion

            return base.VisitBinary(node);
        }

        #region helpers

        private ConstantExpression GetDefaultValue(Type type)
        {
            if (type.IsValueType && !typeof(Nullable).IsAssignableFrom(type))
            {
                return Expression.Constant(Activator.CreateInstance(type), type);
            }
            return Expression.Constant(null, type);
        }

        private bool IsEntityType(Type type)
        {
            return !type.IsValueType
                && type.GetCustomAttribute<ComplexTypeAttribute>(true) == null
                && GetPrimaryKeyComponents(type) != null;
        }

        //TODO expand this to find all unique keys (primary and alternate) => List<List<PropertyInfo>>
        private List<PropertyInfo> GetPrimaryKeyComponents(Type entityType)
        {
            KeyValuePair<string, List<PropertyInfo>> primaryKey;
            List<PropertyInfo> properties = null;
            if (!_entityToKeyEqualityCache.TryGetValue(entityType, out primaryKey))
            {
                try
                {
                    EntitySet container = null;
                    Type dbSetType = entityType;
                    while (container == null && dbSetType != null)
                    {
                        container = _objectContext.MetadataWorkspace.GetEntityContainer(_objectContext.DefaultContainerName, DataSpace.CSpace).EntitySets.Where(set => set.ElementType.Name == dbSetType.Name).FirstOrDefault();
                        dbSetType = dbSetType.BaseType;
                    }
                    string setName = null;
                    if (container != null)
                    {
                        setName = container.Name;
                        var instanceType = !entityType.IsAbstract ? entityType
                            : AppDomain.CurrentDomain.GetAssemblies()
                                .SelectMany(a => a.GetTypes())
                                .Where(t => entityType.IsAssignableFrom(t) && !t.IsAbstract).First();
                        var instance = Activator.CreateInstance(instanceType);
                        var keyProperties = _objectContext.CreateEntityKey(setName, instance).EntityKeyValues.Select(k => k.Key).ToList();
                        properties = entityType.GetProperties().Where(ppty => keyProperties.Contains(ppty.Name)).ToList();
                        _underlyingTables[entityType] = container.Name /*???Table???*/;
                    }
                    primaryKey = new KeyValuePair<string, List<PropertyInfo>>(setName, properties);
                    _entityToKeyEqualityCache[entityType] = primaryKey;
                }
                catch
                {
                    throw;
                }
            }
            return primaryKey.Value;
        }

        #endregion
    }
}