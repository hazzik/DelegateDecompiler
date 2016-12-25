using System;
using System.Linq.Expressions;
using NUnit.Framework;

namespace DelegateDecompiler.Tests
{
    public class ConfigurationTestsBase
    {
        protected static readonly Func<Configuration> InstanceGetter = BuildInstanceGetter();
        protected static readonly Action<Configuration> InstanceSetter = BuildInstanceSetter();

        static Func<Configuration> BuildInstanceGetter()
        {
            return Expression.Lambda<Func<Configuration>>(Expression.Field(null, typeof (Configuration), "instance")).Compile();
        }

        static Action<Configuration> BuildInstanceSetter()
        {
            var arg = Expression.Parameter(typeof (Configuration));
            return Expression.Lambda<Action<Configuration>>(Expression.Assign(Expression.Field(null, typeof (Configuration), "instance"), arg), arg).Compile();
        }

        [TearDown]
        public void SetUp()
        {
            InstanceSetter(null);
        }

        [SetUp]
        public void TearDown()
        {
            InstanceSetter(null);
        }
    }
}