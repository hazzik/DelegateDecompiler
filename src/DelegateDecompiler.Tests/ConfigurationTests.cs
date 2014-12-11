using System;
using System.Linq.Expressions;
using NUnit.Framework;

namespace DelegateDecompiler.Tests
{
    [TestFixture]
    public class ConfigurationTests
    {
        private static readonly Func<Configuration> instanceGetter = BuildInstanceGetter();

        private static readonly Action<Configuration> instanceSetter = BuildInstanceSetter();

        private static Func<Configuration> BuildInstanceGetter()
        {
            return Expression.Lambda<Func<Configuration>>(Expression.Field(null, typeof (Configuration), "instance")).Compile();
        }

        private static Action<Configuration> BuildInstanceSetter()
        {
            var arg = Expression.Parameter(typeof (Configuration));
            return Expression.Lambda<Action<Configuration>>(Expression.Assign(Expression.Field(null, typeof (Configuration), "instance"), arg), arg).Compile();
        }

        [TearDown]
        public void SetUp()
        {
            instanceSetter(null);
        }

        [SetUp]
        public void TearDown()
        {
            instanceSetter(null);
        }

        [Test]
        public void ShouldReturnDefaultConfigurationIfUnconfigured()
        {
            Assert.Null(instanceGetter());
            var instance = Configuration.Instance;
            Assert.IsInstanceOf<DefaultConfiguration>(instance);
        }

        [Test]
        public void ShouldThrowExceptionIfAlreadyConfigured()
        {
            Assert.Null(instanceGetter());
            var instance = Configuration.Instance;
            Assert.NotNull(instance);
            Assert.Throws<InvalidOperationException>(() => Configuration.Configure(new DefaultConfiguration()));
        }

        [Test]
        public void ShouldBeAbleToConfigure()
        {
            Assert.Null(instanceGetter());
            var configuration = new DefaultConfiguration();
            Configuration.Configure(configuration);
            var configured = Configuration.Instance;
            Assert.AreSame(configuration, configured);
        }

    }
}
