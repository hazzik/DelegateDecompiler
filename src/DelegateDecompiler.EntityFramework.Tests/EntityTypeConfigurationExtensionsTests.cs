using System.Data.Entity.ModelConfiguration;
using DelegateDecompiler.Tests;
using NUnit.Framework;

namespace DelegateDecompiler.EntityFramework.Tests
{
    public class EntityTypeConfigurationExtensionsTests : ConfigurationTestsBase
    {
        [Test]
        public void ComputedShouldRegisterPropertyAsDecompileable()
        {
            Assert.Null(InstanceGetter());
            var configuration = new DefaultConfiguration();
            Configuration.Configure(configuration);

            default(EntityTypeConfiguration<TestClass>).Computed(x => x.Property);

            Assert.IsTrue(configuration.ShouldDecompile(typeof(TestClass).GetProperty(nameof(TestClass.Property))));
        }

        [Test]
        public void ComputedShouldRegisterMethodAsDecompileable()
        {
            Assert.Null(InstanceGetter());
            var configuration = new DefaultConfiguration();
            Configuration.Configure(configuration);

            default(EntityTypeConfiguration<TestClass>).Computed(x => x.Method());

            Assert.IsTrue(configuration.ShouldDecompile(typeof(TestClass).GetMethod(nameof(TestClass.Method))));
        }

        [Test]
        public void ComputedShouldThrowExceptionIfUnsupportedExpression()
        {
            Assert.Null(InstanceGetter());
            var configuration = new DefaultConfiguration();
            Configuration.Configure(configuration);

            Assert.That(() =>
            {
                default(EntityTypeConfiguration<TestClass>).Computed(x => x.Property == 0);
            }, Throws.ArgumentException);
        }

        [Test]
        public void ComputedShouldThrowExceptionIfUnsupportedMemberInfo()
        {
            Assert.Null(InstanceGetter());
            var configuration = new DefaultConfiguration();
            Configuration.Configure(configuration);

            Assert.That(() =>
            {
                default(EntityTypeConfiguration<TestClass>).Computed(x => x.Field);
            }, Throws.InvalidOperationException);
        }

        class TestClass
        {
            public int Field = 1;

            public int Property => Field;

            public int Method()
            {
                return Field;
            }
        }
    }
}