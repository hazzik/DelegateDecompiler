using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace DelegateDecompiler.Tests
{
    public class Issue90
    {
        public class Test 
        { 
            public string A { get; set; }
        }

        public class Test2
        {
            public List<Test> Collection { get; } = new List<Test>();

            [Computed]
            public Test Last => Collection.LastOrDefault();
        }

        [Test]
        public void ShouldBeAbleToDecompileLastOrDefault()
        {
            var actual = Enumerable.Range(0, 3)
                .Select(x => new Test2
                {
                    Collection =
                    {
                        new Test { A = (x * 3 + 0).ToString() },
                        new Test { A = (x * 3 + 1).ToString() },
                        new Test { A = (x * 3 + 2).ToString() },
                    }
                })
                .AsQueryable()
                .Select(x => x.Last)
                .Select(x => x.A)
                .Decompile()
                .ToList();

            Assert.That(actual, Is.EqualTo(new[] { "2", "5", "8" }));
        }
    }
}
