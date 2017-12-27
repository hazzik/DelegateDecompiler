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
    public class DecompileExpressionVisitor
        : DelegateDecompiler.DecompileExpressionVisitor
    {
        public DecompileExpressionVisitor()
        {
        }

        private ObjectContext _objectContext = null;

        private Dictionary<Type, List<PropertyInfo>> _entityToKeyEqualityCache = new Dictionary<Type, List<PropertyInfo>>();
        private Dictionary<Type, string> _underlyingTables = new Dictionary<Type, string>();

        protected override Expression VisitConstant(ConstantExpression node)
        {
            #region Reference the ObjectContext from which to get models metadata

            if (typeof(ObjectQuery).IsAssignableFrom(node.Type))
            {
                var objectQuery = (node.Value as ObjectQuery);

                //objectQuery.MergeOption = DecompileExtensions.CurrentMergeOption;
                var targetContext = objectQuery.Context;
                if (_objectContext != null && _objectContext != targetContext)
                {
                    throw new NotSupportedException("Cannot use different ObjectContext instances in the same query");
                }
                _objectContext = targetContext;

                #endregion
            }

            #region Resolve entity constants by an object query filtered on primary key

            else if (IsEntityType(node.Type) || (node.Type.IsGenericType && typeof(IEnumerable).IsAssignableFrom(node.Type) && IsEntityType(node.Type.GetGenericArguments().First())))
            {
                // first check whether the constant is an entity or a collection of constants
                bool isCollection = !IsEntityType(node.Type);

                //TODO try later to solve this case with performance in mind
                if (isCollection) throw new NotSupportedException($"Yet unable to create a collection of '{node.Type.GetGenericArguments().First()}' constant values. Use an ObjectQuery from the current context instead.");

                // Build the base ObjectQuery
                string sqlQuery = $"SELECT VALUE {node.Type.Name} FROM {_underlyingTables[node.Type]} as {node.Type.Name}";
                var createQueryMethod = _objectContext.GetType().GetMethod("CreateQuery", BindingFlags.Public | BindingFlags.Instance).MakeGenericMethod(node.Type);
                var queryableType = typeof(ObjectQuery<>).MakeGenericType(node.Type);

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
            #region EF annoying limitation lift : Unable to create a constant value of type T, where T entity

            if (node.Expression != null && node.Expression.NodeType == ExpressionType.Constant)
            {
                var targetInstance = (node.Expression as ConstantExpression).Value;
                var fieldInfo = node.Member as FieldInfo;
                var propertyInfo = node.Member as PropertyInfo;
                if ((fieldInfo != null && !fieldInfo.IsStatic) || (propertyInfo != null && !propertyInfo.GetMethod.IsStatic))
                {
                    if (targetInstance != null)
                    {
                        var value = fieldInfo != null && !fieldInfo.IsStatic
                            ? fieldInfo.GetValue(targetInstance)
                            : propertyInfo != null && !propertyInfo.GetMethod.IsStatic
                            ? propertyInfo.GetValue(targetInstance)
                            : null /*??? MethodInfo ???*/;
                        Expression result = Expression.Constant(value, node.Type);
                        return Visit(result);
                    }
                    else
                    {
                        return GetDefaultValue(node.Type);
                    }
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
            var operandType = GetMostSpecificType(node.Left.Type, node.Right.Type);

            #region EF annoying limitation lift : replace Entities' comparison Expressions by comparison of their PrimaryKey

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
                    // Otherwise compare the operands by primary key
                    List<PropertyInfo> primaryKeyDefinition = GetPrimaryKeyProperties(operandType);
                    foreach (PropertyInfo keyPpty in primaryKeyDefinition)
                    {
                        var keyComponentEquality = Expression.Equal(Expression.MakeMemberAccess(node.Left, keyPpty), Expression.MakeMemberAccess(node.Right, keyPpty));
                        translated = translated == null ? keyComponentEquality : Expression.AndAlso(translated, keyComponentEquality);
                    }
                    translated = base.Visit(translated);
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
            return !type.IsValueType && type.GetCustomAttribute<ComplexTypeAttribute>(true) == null && GetPrimaryKeyProperties(type) != null; ;
        }

        private List<PropertyInfo> GetPrimaryKeyProperties(Type entityType)
        {
            List<PropertyInfo> properties = null;
            if (!_entityToKeyEqualityCache.TryGetValue(entityType, out properties))
            {
                try
                {
                    var container = _objectContext.MetadataWorkspace.GetEntityContainer(_objectContext.DefaultContainerName, DataSpace.CSpace).EntitySets.Where(set => set.ElementType.Name == entityType.Name).FirstOrDefault();
                    if (container != null)
                    {
                        string setName = container.Name;
                        var instance = Activator.CreateInstance(entityType);
                        var keyProperties = _objectContext.CreateEntityKey(setName, instance).EntityKeyValues.Select(k => k.Key).ToList();
                        properties = entityType.GetProperties().Where(ppty => keyProperties.Contains(ppty.Name)).ToList();
                        _underlyingTables[entityType] = container.Name /*???Table???*/;
                    }
                    _entityToKeyEqualityCache[entityType] = properties;
                }
                catch
                {
                    throw;
                }
            }
            return properties;
        }

        #endregion
    }
}