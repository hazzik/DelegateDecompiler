using DelegateDecompiler.EntityFrameworkCore;
using DelegateDecompiler.Tests;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NUnit.Framework;

namespace DelegateDecompiler.EntityFramework.Tests
{
    public class EntityTypeBuilderExtensionsTests : ConfigurationTestsBase
    {
        [Test]
        public void ComputedShouldRegisterPropertyAsDecompileable()
        {
            Assert.Null(InstanceGetter());
            var configuration = new DefaultConfiguration();
            Configuration.Configure(configuration);

            default(EntityTypeBuilder<TestClass>).Computed(x => x.Property);

            Assert.IsTrue(configuration.ShouldDecompile(typeof(TestClass).GetProperty(nameof(TestClass.Property))));
        }

        [Test]
        public void ComputedShouldRegisterMethodAsDecompileable()
        {
            Assert.Null(InstanceGetter());
            var configuration = new DefaultConfiguration();
            Configuration.Configure(configuration);

            default(EntityTypeBuilder<TestClass>).Computed(x => x.Method());

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
                default(EntityTypeBuilder<TestClass>).Computed(x => x.Property == 0);
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
                default(EntityTypeBuilder<TestClass>).Computed(x=>x.Field);
            }, Throws.InvalidOperationException);
        }

        class TestClass
        {
            public int Field;
            public int Property { get { return Field; } }

            public int Method()
            {
                return Field;
            }
        }
    }
}
