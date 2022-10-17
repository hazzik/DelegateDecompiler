using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace DelegateDecompiler.Tests
{
    [TestFixture]
    public class Issue13 : DecompilerTestsBase
    {
        [Test]
        public void Test()
        {
            var users = new[]
            {
                new User
                {
                    Level = UserLevel.All
                },
                new User
                {
                    Company = new Company
                    {
                        Regions =
                        {
                            new Region {ID = 6}
                        }
                    },
                    Level = UserLevel.Company
                },
                new User
                {
                    AllowedRegions =
                    {
                        new Region {ID = 6}
                    },
                    Level = UserLevel.Region
                },
                new User
                {
                    Company = new Company
                    {
                        Regions =
                        {
                            new Region {ID = 5}
                        }
                    },
                    Level = UserLevel.Company
                },
                new User
                {
                    AllowedRegions =
                    {
                        new Region {ID = 7}
                    },
                    Level = UserLevel.Region
                }
            };
            var testRegionID = 6;
            var result = users.AsQueryable().Where(e => e.IsRegionAllowed(testRegionID)).Decompile().ToList();
            Assert.That(result, Has.Count.EqualTo(3));
        }

        public class Company
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public List<Region> Regions { get; private set; }

            public Company()
            {
                Regions = new List<Region>();
            }
        }

        public class Region
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public Company Company { get; set; }
        }

        public class User
        {
            public int ID { get; set; }
            public string Name { get; set; }

            public Company Company { get; set; }

            public UserLevel Level { get; set; }
            public List<Region> AllowedRegions { get; private set; }

            public User()
            {
                AllowedRegions = new List<Region>();
            }

            [Computed]
            public bool IsRegionAllowed(int regionID)
            {
                return Level == UserLevel.All ||
                       Level == UserLevel.Company && Company.Regions.Any(e => e.ID == regionID) ||
                       Level == UserLevel.Region && AllowedRegions.Any(e => e.ID == regionID);
            }
        }

        public enum UserLevel
        {
            All = 1,
            Company = 2,
            Region = 3
        }
    }
}