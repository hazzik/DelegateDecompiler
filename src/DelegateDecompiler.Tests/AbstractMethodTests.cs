using System;
using System.Linq.Expressions;
using NUnit.Framework;

namespace DelegateDecompiler.Tests
{
    [TestFixture]
    public class AbstractMethodTests : DecompilerTestsBase
    {
        public abstract class A
        {
            public abstract string M1();

            public abstract string M2();

            public virtual string M3() => "A";

            public virtual string M4() => "A";
        }

        public class B : A
        {
            public string Me = "B";

            public override string M1() => "B";

            public override string M2() => Me;
            
            public override string M3() => "B";
        }

        public class C : A
        {
            public string Me = "C";

            public override string M1() => "C";

            public override string M2() => Me;

            public override string M3() => "C";
        }

        public class D : C
        {
            public string Me = "C";

            public override string M1() => "D";

            public override string M2() => Me;

            public override string M3() => "D";
        }

        public abstract class E : C
        {
            public string Me = "C";

            public override string M1() => "E";

            public override string M2() => Me;

            public override string M3() => "E";
        }

        [Test]
        public void DecompileMethod()
        {
            Expression<Func<A, string>> e = @this =>
                @this is E ? "E"
                : @this is D ? "D"
                : @this is C ? "C"
                : @this is B ? "B"
                : null;

            Test(e, typeof(A).GetMethod(nameof(A.M1)));
        }

        [Test]
        public void DecompileMethodWithReferences()
        {
            // ReSharper disable MergeCastWithTypeCheck
            Expression<Func<A, string>> e = @this =>
                @this is E ? ((E) @this).Me
                : @this is D ? ((D) @this).Me
                : @this is C ? ((C) @this).Me
                : @this is B ? ((B) @this).Me
                : null;
            // ReSharper restore MergeCastWithTypeCheck

            Test(e, typeof(A).GetMethod(nameof(A.M2)));
        }

        [Test]
        public void DecompileVirtualMethod()
        {
            Expression<Func<A, string>> e = @this =>
                @this is E ? "E"
                : @this is D ? "D"
                : @this is C ? "C"
                : @this is B ? "B"
                : "A";

            Test(e, typeof(A).GetMethod(nameof(A.M3)));
        }

        [Test]
        public void DecompileVirtualMethodWithoutOverrides()
        {
            Expression<Func<A, string>> e = @this => "A";

            Test(e, typeof(A).GetMethod(nameof(A.M4)));
        }
    }
}
