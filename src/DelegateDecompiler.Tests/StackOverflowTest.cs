using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using NUnit.Framework;
using System.Threading;

namespace DelegateDecompiler.Tests
{
    [TestFixture]
    public class StackOverflowTest : DecompilerTestsBase
    {
        [Test]
        public void StackOverflowTestOnCurrentThread()
        {
            Expression<Func<Employee, string>> expected = e => e.FirstName != null ? e.FirstName :
                (e.FirstName != null ? e.FirstName :
                (e.FirstName != null ? e.FirstName :
                (e.FirstName != null ? e.FirstName :
                (e.FirstName != null ? e.FirstName :
                (e.FirstName != null ? e.FirstName :
                (e.FirstName != null ? e.FirstName : e.LastName))))));
            Func<Employee, string> compiled = e => e.FirstName != null ? e.FirstName :
                (e.FirstName != null ? e.FirstName :
                (e.FirstName != null ? e.FirstName :
                (e.FirstName != null ? e.FirstName :
                (e.FirstName != null ? e.FirstName :
                (e.FirstName != null ? e.FirstName :
                (e.FirstName != null ? e.FirstName : e.LastName))))));
            Test(compiled, expected);
        }

        [Test]
        public void StackOverflowTestOnThreadWithLargeStack()
        {
            Thread thread = new Thread(() =>
            {
                Expression<Func<Employee, string>> expected = e => e.FirstName != null ? e.FirstName :
                    (e.FirstName != null ? e.FirstName :
                    (e.FirstName != null ? e.FirstName :
                    (e.FirstName != null ? e.FirstName :
                    (e.FirstName != null ? e.FirstName :
                    (e.FirstName != null ? e.FirstName :
                    (e.FirstName != null ? e.FirstName : e.LastName))))));
                Func<Employee, string> compiled = e => e.FirstName != null ? e.FirstName :
                    (e.FirstName != null ? e.FirstName :
                    (e.FirstName != null ? e.FirstName :
                    (e.FirstName != null ? e.FirstName :
                    (e.FirstName != null ? e.FirstName :
                    (e.FirstName != null ? e.FirstName :
                    (e.FirstName != null ? e.FirstName : e.LastName))))));
                Test(compiled, expected);
            }, Int32.MaxValue);
            thread.Start();
            thread.Join();
        }

        [Test]
        public void StackOverflowTestOnThreadWithSmallStack()
        {
            Thread thread = new Thread(() =>
            {
                Expression<Func<Employee, string>> expected = e => e.FirstName != null ? e.FirstName :
                    (e.FirstName != null ? e.FirstName :
                    (e.FirstName != null ? e.FirstName :
                    (e.FirstName != null ? e.FirstName :
                    (e.FirstName != null ? e.FirstName :
                    (e.FirstName != null ? e.FirstName :
                    (e.FirstName != null ? e.FirstName : e.LastName))))));
                Func<Employee, string> compiled = e => e.FirstName != null ? e.FirstName :
                    (e.FirstName != null ? e.FirstName :
                    (e.FirstName != null ? e.FirstName :
                    (e.FirstName != null ? e.FirstName :
                    (e.FirstName != null ? e.FirstName :
                    (e.FirstName != null ? e.FirstName :
                    (e.FirstName != null ? e.FirstName : e.LastName))))));
                Test(compiled, expected);
            }, 256);
            thread.Start();
            thread.Join();
        }
    }
}
