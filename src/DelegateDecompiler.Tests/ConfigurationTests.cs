using System;
using System.Linq.Expressions;
using Xunit;

namespace DelegateDecompiler.Tests
{
    public class ConfigurationTests : IDisposable
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

        public ConfigurationTests()
        {
            instanceSetter(null);
        }

        [Fact]
        public void ShouldReturnDefaultConfigurationIfUnconfigured()
        {
            Assert.Null(instanceGetter());
            var instance = Configuration.Instance;
            Assert.IsType<DefaultConfiguration>(instance);
        }

        [Fact]
        public void ShouldThrowExceptionIfAlreadyConfigured()
        {
            Assert.Null(instanceGetter());
            var instance = Configuration.Instance;
            Assert.NotNull(instance);
            Assert.Throws<InvalidOperationException>(() => Configuration.Configure(new DefaultConfiguration()));
        }

        [Fact]
        public void ShouldBeAbleToConfigure()
        {
            Assert.Null(instanceGetter());
            var configuration = new DefaultConfiguration();
            Configuration.Configure(configuration);
            var configured = Configuration.Instance;
            Assert.Same(configuration, configured);
        }

        public void Dispose()
        {
            instanceSetter(null);
        }
    }
}
