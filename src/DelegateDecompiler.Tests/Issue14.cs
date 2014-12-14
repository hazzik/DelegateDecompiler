using System;
using System.Linq.Expressions;
using NUnit.Framework;

namespace DelegateDecompiler.Tests
{
    [TestFixture]
    public class Issue14 : DecompilerTestsBase
    {
        class MyClass
        {
            public enum ValidityMode
            {
                None,
                UntilSpecificDateTime,
                ForSpecificTimeSpan
            }

            public ValidityMode Mode { get; set; }
            public DateTime StartTime { get; set; }

            public int ValidForXMinutes { get; set; }
            public DateTime ValidUntil { get; set; }

            [Computed]
            public bool IsValid
            {
                get { return Mode == ValidityMode.None || (Mode == ValidityMode.UntilSpecificDateTime ? ValidUntil > DateTime.Now.AddMinutes(5) : StartTime.AddMinutes(ValidForXMinutes) > DateTime.Now.AddMinutes(5)); }
            }
        }

        [Test, Ignore("Difference is expected")]
        public void Test()
        {
            Expression<Func<MyClass, bool>> expected = x => x.Mode == MyClass.ValidityMode.None || (x.Mode == MyClass.ValidityMode.UntilSpecificDateTime ? x.ValidUntil > DateTime.Now.AddMinutes(5) : x.StartTime.AddMinutes(x.ValidForXMinutes) > DateTime.Now.AddMinutes(5));
            Func<MyClass, bool> compiled = x => x.Mode == MyClass.ValidityMode.None || (x.Mode == MyClass.ValidityMode.UntilSpecificDateTime ? x.ValidUntil > DateTime.Now.AddMinutes(5) : x.StartTime.AddMinutes(x.ValidForXMinutes) > DateTime.Now.AddMinutes(5));
            Test(expected, compiled);
        }
    }
}