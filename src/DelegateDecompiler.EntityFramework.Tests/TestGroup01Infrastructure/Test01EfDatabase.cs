// Contributed by @JonPSmith (GitHub) www.thereformedprogrammer.com

using System.Linq;
using DelegateDecompiler.EntityFramework.Tests.EfItems;
using DelegateDecompiler.EntityFramework.Tests.EfItems.Concretes;
using DelegateDecompiler.EntityFramework.Tests.Helpers;
using NUnit.Framework;
#if EF_CORE
using Microsoft.EntityFrameworkCore;
#endif

namespace DelegateDecompiler.EntityFramework.Tests.TestGroup01Infrastructure
{
    class Test01EfDatabase
    {
        [Test]
        public void Test01FillDatabaseOk()
        {
            using (var db = new EfTestDbContext())
            {
                //SETUP
#if EF_CORE
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
#endif
                //ATTEMPT
                db.ResetDatabaseContent();

                //VERIFY
                db.EfParents.Count().ShouldEqual(DatabaseHelpers.BaseData.Count);
                db.EfChildren.Count().ShouldEqual(DatabaseHelpers.BaseData.SelectMany( x => x.Children).Count());

                db.EfPersons.Count().ShouldEqual(DatabaseHelpers.PersonsData.Count);
            }
        }

    }
}
