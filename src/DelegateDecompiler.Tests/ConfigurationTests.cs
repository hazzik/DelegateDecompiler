using System;
using NUnit.Framework;

namespace DelegateDecompiler.Tests
{
    [TestFixture]
    public class ConfigurationTests : ConfigurationTestsBase
    {
        [Test]
        public void ShouldReturnDefaultConfigurationIfUnconfigured()
        {
            Assert.Null(InstanceGetter());
            var instance = Configuration.Instance;
            Assert.IsInstanceOf<DefaultConfiguration>(instance);
        }

        [Test]
        public void ShouldThrowExceptionIfAlreadyConfigured()
        {
            Assert.Null(InstanceGetter());
            var instance = Configuration.Instance;
            Assert.NotNull(instance);
            Assert.Throws<InvalidOperationException>(() => Configuration.Configure(new DefaultConfiguration()));
        }

        [Test]
        public void ShouldBeAbleToConfigure()
        {
            Assert.Null(InstanceGetter());
            var configuration = new DefaultConfiguration();
            Configuration.Configure(configuration);
            var configured = Configuration.Instance;
            Assert.That(configured, Is.SameAs(configuration));
        }
    }
}
