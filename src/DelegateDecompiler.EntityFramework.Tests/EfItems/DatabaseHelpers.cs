// Contributed by @JonPSmith (GitHub) www.thereformedprogrammer.com

using System;
using System.Collections.Generic;
using System.Linq;
using DelegateDecompiler.EntityFramework.Tests.EfItems.Abstracts;
using DelegateDecompiler.EntityFramework.Tests.EfItems.Concretes;

namespace DelegateDecompiler.EntityFramework.Tests.EfItems
{
    internal static class DatabaseHelpers
    {
        public const int ParentIntUniqueValue = 987;

        public static readonly ICollection<EfParent> BaseData = new List<EfParent>
        {
            new EfParent
            {
                ParentBool = true,
                ParentInt = 123,
                ParentNullableInt = 123,
                ParentDouble = 123.456,
                ParentString = "123",
                StartDate = new DateTime(2001, 2, 3),
                EndDate = new DateTime(2014, 1, 1),
                ParentTimeSpan = new TimeSpan(0, 1, 2, 3),
                Children = new List<EfChild>
                {
                    new EfChild
                    {
                        ChildBool = true,
                        ChildInt = 234,
                        ChildDouble = 234.567,
                        ChildString = "234",
                        ChildDateTime = new DateTime(2002, 3, 4),
                        ChildTimeSpan = new TimeSpan(0, 2, 3, 4)
                    },
                    new EfChild
                    {
                        ChildBool = false,
                        ChildInt = 345,
                        ChildDouble = 345.678,
                        ChildString = "345",
                        ChildDateTime = new DateTime(2003, 4, 5),
                        ChildTimeSpan = new TimeSpan(0, 3, 4, 5)
                    }
                }
            },
            new EfParent
            {
                ParentBool = false,
                ParentInt = ParentIntUniqueValue,
                ParentNullableInt = null,
                ParentDouble = 987.654,
                ParentString = "987",
                StartDate = new DateTime(2009, 8, 7),
                EndDate = new DateTime(2014, 1, 1),
                ParentTimeSpan = new TimeSpan(0, 9, 8, 7),
                Children = new List<EfChild>
                {
                    new EfChild
                    {
                        ChildBool = true,
                        ChildInt = 876,
                        ChildDouble = 876.543,
                        ChildString = "876",
                        ChildDateTime = new DateTime(2008, 7, 6),
                        ChildTimeSpan = new TimeSpan(0, 8, 7, 6)
                    }
                }
            },
            new EfParent
            {
                ParentBool = false,
                ParentInt = 111,
                ParentNullableInt = 111,
                ParentDouble = 111.222,
                ParentString = "111",
                StartDate = new DateTime(2001, 1, 1),
                EndDate = new DateTime(2014, 1, 1),
                ParentTimeSpan = new TimeSpan(0, 1, 1, 1),
                Children = new List<EfChild>()
            }
        };

        public static readonly ICollection<EfPerson> PersonsData = new List<EfPerson>
        {
            new EfPerson {FirstName = "Jon", LastName = "Smith"},
            new EfPerson {FirstName = "Jon", MiddleName = "P", LastName = "Smith"},
            new EfPerson {FirstName = "Fred", LastName = "Blogs", NameOrder = true}
        };

        public static readonly ICollection<LivingBeeing> LivingBeeingsData = InitializeLivingBeeings();

        private static ICollection<LivingBeeing> InitializeLivingBeeings()
        {
            var animal1 = new Dog { Age = 2};
            var animal2 = new Dog { Age = 3};
            return new List<LivingBeeing>
            {
                animal1,
                animal2,
                new HoneyBee(),
                new HoneyBee(),
                new Person {Age = 1, Birthdate = new DateTime(1900, 1, 1), Name = "Joseph"},
                new Person {Age = 2, Birthdate = new DateTime(1900, 1, 2), Name = "Maria"},
                new Person
                {
                    Age = 3,
                    Birthdate = new DateTime(1900, 1, 2),
                    Name = "John Doe",
                    Animals = new List<Animal> {animal1, animal2}
                }
            };
        }


        public static void ResetDatabaseContent(this EfTestDbContext db)
        {
            //check that ParentIntUniqueValue is unique
            if (BaseData.Count(x => x.ParentInt == ParentIntUniqueValue) != 1)
                throw new InvalidOperationException(
                    "The test data must have only one item with ParentInt equal to ParentIntUniqueValue");

            //wipe out all exsiting data
            db.EfParents.RemoveRange(db.EfParents);
            db.EfPersons.RemoveRange(db.EfPersons);
            db.LivingBeeing.RemoveRange(db.LivingBeeing);
            db.SaveChanges();

            db.EfParents.AddRange(BaseData);
            db.EfPersons.AddRange(PersonsData);
            db.LivingBeeing.AddRange(LivingBeeingsData);
            db.SaveChanges();
        }
    }
}
