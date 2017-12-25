using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DelegateDecompiler.EntityFramework
{
    //TODO override every VisitXxx method to first resolve constants using node.Update within the current call context since EntityFramework do not support resolution of non-primitive or non-enum constants
    public class DecompileExpressionVisitor
        : DelegateDecompiler.DecompileExpressionVisitor
    {
        public DecompileExpressionVisitor()
        {
        }

        private ObjectContext _objectContext;

        private Dictionary<Type, List<PropertyInfo>> _entityToKeyEqualityCache = new Dictionary<Type, List<PropertyInfo>>();

        protected override Expression VisitConstant(ConstantExpression node)
        {
            #region Auto-detect the ObjectContext to use

            //TODO check if it is safe to assume that only one objectcontext can be used in a single queryable
            if (typeof(ObjectQuery).IsAssignableFrom(node.Type))
            {
                var targetContext = (node.Value as ObjectQuery).Context;
                if (_objectContext != null && _objectContext != targetContext)
                {
                    throw new NotSupportedException("Cannot use different ObjectContext instances in the same query");
                }
                _objectContext = targetContext;
            }

            #endregion

            return base.VisitConstant(node);
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            node = node.Update(base.Visit(node.Operand));
            if (node.NodeType == ExpressionType.TypeAs && node.Operand is ConstantExpression)
            {
                return Expression.Constant((node.Operand as ConstantExpression).Value, node.Type);
            }
            return base.VisitUnary(node);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            var operandType = node.Left.Type;

            #region EF annoying limitation lift : replace Entities' comparison Expressions by comparison of their PrimaryKey

            if (IsEntityType(operandType) && (node.NodeType == ExpressionType.Equal || node.NodeType == ExpressionType.NotEqual))
            {
                node = node.Update(base.Visit(node.Left), null, base.Visit(node.Right));
                Expression translated = null;
                List<PropertyInfo> primaryKeyDefinition = GetPrimaryKeyProperties(operandType);
                foreach (PropertyInfo keyPpty in primaryKeyDefinition)
                {
                    var keyComponentEquality = Expression.Equal(Expression.MakeMemberAccess(base.Visit(node.Left), keyPpty), Expression.MakeMemberAccess(base.Visit(node.Right), keyPpty));
                    translated = translated == null ? keyComponentEquality : Expression.AndAlso(translated, keyComponentEquality);
                }
                if (node.NodeType == ExpressionType.NotEqual)
                {
                    translated = Expression.Not(translated);
                }
                return base.Visit(translated);
            }

            #endregion

            return base.VisitBinary(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            #region EF annoying limitation lift : Unable to create a constant value of type T, where T entity

            node = node.Update(base.Visit(node.Expression));
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
                        return result;
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
            node = node.Update(base.Visit(node.Object), node.Arguments.Select(arg => base.Visit(arg)).ToArray());
            // It seems Entityframework does not forward null method calls
            if (node.Object == null && !node.Method.IsStatic)
            {
                return GetDefaultValue(node.Method.ReturnType);
            }
            return base.VisitMethodCall(node);
        }

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
    }
}