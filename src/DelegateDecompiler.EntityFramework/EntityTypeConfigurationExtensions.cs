using System;
using System.Data.Entity.ModelConfiguration;
using System.Linq.Expressions;

namespace DelegateDecompiler.EntityFramework
{
    public static class EntityTypeConfigurationExtensions
    {
        public static EntityTypeConfiguration<T> Computed<T, T1>(this EntityTypeConfiguration<T> configuration, Expression<Func<T, T1>> propertyExpression) where T : class
        {
            var configurator = Configuration.Instance as DefaultConfiguration;

            var memberInfo = ((MemberExpression) propertyExpression.Body).Member;

            configurator?.ComputedMembers.Add(memberInfo);

            return configuration;
        }
    }
}
