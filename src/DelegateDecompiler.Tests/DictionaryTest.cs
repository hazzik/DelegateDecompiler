using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NUnit.Framework;

namespace DelegateDecompiler.Tests
{
    [TestFixture]
    public class DictionaryTest : DecompilerTestsBase
    {
        [Test]
        public void CanDecompileDictionaryIndexerGetter()
        {
	        Expression<Func<IDictionary<string, object>, object>> expected = x => x["A"];
	        Func<IDictionary<string, object>, object> compiled = x => x["A"];
            Test(expected, compiled);
        }
    }
}
