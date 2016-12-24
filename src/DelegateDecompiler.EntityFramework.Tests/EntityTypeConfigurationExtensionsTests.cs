using System.Data.Entity.ModelConfiguration;
using DelegateDecompiler.EntityFramework.Tests.EfItems;
using NUnit.Framework;

namespace DelegateDecompiler.EntityFramework.Tests
{
    public class EntityTypeConfigurationExtensionsTests
    {
        [Test]
        public void ComputedAddMemberInfoInComputedMembersHashSet()
        {
            var defaultConfiguration = new DefaultConfiguration();
            Configuration.Configure(defaultConfiguration);

            default(EntityTypeConfiguration<EfPerson>).Computed(x => x.FullNameNoAttibute);

            Assert.IsTrue(defaultConfiguration.ComputedMembers.Contains(typeof(EfPerson).GetProperty(nameof(EfPerson.FullNameNoAttibute))));
        }
    }
}