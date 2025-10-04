using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace DelegateDecompiler.Tests
{
    public class Issue90 : DecompilerTestsBase
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
            var query = Enumerable.Range(0, 3)
                .Select(x => new Test2
                {
                    Collection =
                    {
                        new Test { A = (x * 3 + 0).ToString() },
                        new Test { A = (x * 3 + 1).ToString() },
                        new Test { A = (x * 3 + 2).ToString() },
                    }
                }).AsQueryable();

            var expected = query.Select(x => x.Collection.LastOrDefault()).Select(x => x.A);
            var actual = query.Select(x => x.Last).Select(x => x.A).Decompile();

            AssertAreEqual(actual.Expression, expected.Expression);

            Assert.That(actual.ToList(), Is.EqualTo(new[] { "2", "5", "8" }));
        }
    }
}
