using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DelegateDecompiler.EntityFramework
{
    public class DecompileExpressionVisitor
        : DelegateDecompiler.DecompileExpressionVisitor
    {
        public DecompileExpressionVisitor(DbContext targetContext)
        {
            _model = targetContext;
        }

        private DbContext _model;

        private ObjectContext ObjectContext { get { return ((IObjectContextAdapter)_model).ObjectContext; } }

        private Dictionary<Type, List<PropertyInfo>> _entityToKeyEqualityCache = new Dictionary<Type, List<PropertyInfo>>();

        protected override Expression VisitBinary(BinaryExpression node)
        {
            var operandType = node.Left.Type;
            var isOperandAnEntity = !operandType.IsValueType && operandType.GetCustomAttribute<ComplexTypeAttribute>(true) == null;

            #region First EF annoying limitation lift : replace Entities' comparison Expressions by comparison of their PrimaryKey

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

        public override Expression Visit(Expression node)
        {
            return base.Visit(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            return base.VisitMethodCall(node);
        }

        private List<PropertyInfo> GetPrimaryKeyProperties(Type entityType)
        {
            List<PropertyInfo> properties;
            if (!_entityToKeyEqualityCache.TryGetValue(entityType, out properties))
            {
                var container = ObjectContext.MetadataWorkspace.GetEntityContainer(ObjectContext.DefaultContainerName, DataSpace.CSpace);
                string setName = container.EntitySets.Where(set => set.ElementType.Name == entityType.Name).Select(set => set.Name).FirstOrDefault();
                var instance = _model.Set(entityType).Create();
                var keyProperties = ObjectContext.CreateEntityKey(setName, instance).EntityKeyValues.Select(k => k.Key).ToList();
                properties = entityType.GetProperties().Where(ppty => keyProperties.Contains(ppty.Name)).ToList();
                _entityToKeyEqualityCache[entityType] = properties;
            }
            return properties;
        }
    }
}