// Contributed by @JonPSmith (GitHub) www.thereformedprogrammer.com

using System.Linq;
using DelegateDecompiler.EfTests.EfItems;
using DelegateDecompiler.EfTests.Helpers;
using NUnit.Framework;

namespace DelegateDecompiler.EfTests.TestGroup01Infrastructure
{
    class Test01EfDatabase
    {
        [Test]
        public void Test01FillDatabaseOk()
        {
            using (var db = new EfTestDbContext())
            {
                //SETUP
                

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
