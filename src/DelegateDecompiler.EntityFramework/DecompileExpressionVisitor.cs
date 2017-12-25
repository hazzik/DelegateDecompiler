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
    public class DecompileExpressionVisitor
        : DelegateDecompiler.DecompileExpressionVisitor
    {
        public DecompileExpressionVisitor()
        {
        }

        private ObjectContext _objectContext = null;

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

            /* TODO resolve closure entity or entity collection constants either by brute force
             * or lookup against the _objectContext by primaryKey.
             * This would mean replacing the constant by one of the followin expressions :
             *  => ObjectQuery<T>().Where(o => o == constant)
             *  => ObjectQuery<T>().Where(o => constantIsCollection.Any(constantItem => o == constantItem))
             */
            return base.VisitConstant(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            return base.VisitMember(node);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            var operandType = node.Left.Type;
            var isOperandAnEntity = !operandType.IsValueType && operandType.GetCustomAttribute<ComplexTypeAttribute>(true) == null;

            #region EF annoying limitation lift : replace Entities' comparison Expressions by comparison of their PrimaryKey

            if ((node.NodeType == ExpressionType.Equal || node.NodeType == ExpressionType.NotEqual) && isOperandAnEntity)
            {
                Expression translated = null;
                List<PropertyInfo> primaryKeyDefinition = GetPrimaryKeyProperties(operandType);
                foreach (PropertyInfo keyPpty in primaryKeyDefinition)
                {
                    var keyComponentEquality = Expression.Equal(Expression.MakeMemberAccess(node.Left, keyPpty), Expression.MakeMemberAccess(node.Right, keyPpty));
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