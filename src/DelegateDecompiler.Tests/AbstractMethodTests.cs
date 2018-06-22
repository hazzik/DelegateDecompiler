using System;
using System.Linq.Expressions;
using NUnit.Framework;

namespace DelegateDecompiler.Tests
{
    [TestFixture]
    public class AbstractMethodTests : DecompilerTestsBase
    {
        public interface IA
        {
            string M();
            string M5();
        }

        public class X : IA
        {
            public string M() => "X";

            public virtual string M5() => "X";
        }

        public class Y : X
        {
            public override string M5() => "Y";            
        }

        public class Y1 : Y
        {
            public override string M5() => "Y1";            
        }
        
        public class Y2 : Y
        {
            public override string M5() => "Y2";            
        }

        public class Z : X
        {
            public override string M5() => "Z";
        }

        public abstract class A : IA
        {
            public string M() => "A";

            public virtual string M5() => "A";

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

            public override string M5() => "B";
        }

        public class C : A
        {
            public string Me = "C";

            public override string M1() => "C";

            public override string M2() => Me;

            public override string M3() => "C";

            public override string M5() => "C";
        }

        public class D : C
        {
            public string Me = "C";

            public override string M1() => "D";

            public override string M2() => Me;

            public override string M3() => "D";

            public override string M5() => "D";
        }

        public abstract class E : C
        {
            public string Me = "C";

            public override string M1() => "E";

            public override string M2() => Me;

            public override string M3() => "E";

            public override string M5() => "E";
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

        [Test]
        public void DecompileInterfaceMethod()
        {
            Expression<Func<IA, string>> e = @this =>
                @this is X ? "X"
                : @this is A ? "A"
                : null;

            Test(e, typeof(IA).GetMethod(nameof(IA.M)));
        }

        [Test]
        public void DecompileVirtualInterfaceMethod()
        {
            Expression<Func<IA, string>> e = @this =>
                @this is Z ? "Z"
                : @this is Y2 ? "Y2"
                : @this is Y1 ? "Y1"
                : @this is Y ? "Y"
                : @this is X ? "X"
                : @this is E ? "E"
                : @this is D ? "D"
                : @this is C ? "C"
                : @this is B ? "B"
                : @this is A ? "A"
                : null;

            Test(e, typeof(IA).GetMethod(nameof(IA.M5)));
        }
    }
}
