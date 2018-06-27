using System;
using System.Linq.Expressions;
using System.Reflection;
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

            public virtual string M6() => "A";

            public virtual string M7 => "A";
        }

        public class B : A
        {
            public string Me = "B";

            public override string M1() => "B";

            public override string M2() => Me;

            public override string M3() => "B";

            public override string M5() => "B";

            public override string M6() => "B is child of " + base.M6().ToString();
           
            public override string M7 => "B is child of " + base.M7.ToString();
        }

        public class C : A
        {
            public string Me = "C";

            public override string M1() => "C";

            public override string M2() => Me;

            public override string M3() => "C";

            public override string M5() => "C";
            
            public override string M6() => "C is child of " + base.M6().ToString();
           
            public override string M7 => "C is child of " + base.M7.ToString();
        }

        public class D : C
        {
            public string Me = "C";

            public override string M1() => "D";

            public override string M2() => Me;

            public override string M3() => "D";

            public override string M5() => "D";

            public override string M6() => "D is child of " + base.M6().ToString();
            
            public override string M7 => "D is child of " + base.M7.ToString();
        }

        public abstract class E : C
        {
            public string Me = "C";

            public override string M1() => "E";

            public override string M2() => Me;

            public override string M3() => "E";

            public override string M5() => "E";

            public override string M6() => "E is child of " + base.M6().ToString();

            public override string M7 => "E is child of " + base.M7.ToString();

            // Not used directly but here to create a method collision by name with M1
            // ReSharper disable once UnusedMember.Global
            public string M1(int a) => a.ToString();
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
        public void DecompileVirtualMethodWithBaseCall()
        {
            Expression<Func<A, string>> e = @this =>
                @this is E ? "E is child of " + ("C is child of " + "A".ToString()).ToString()
                : @this is D ? "D is child of " + ("C is child of " + "A".ToString()).ToString()
                : @this is C ? "C is child of " + "A".ToString()
                : @this is B ? "B is child of " + "A".ToString()
                : "A";

            Test(e, typeof(A).GetMethod(nameof(A.M6)));
        }

        [Test]
        public void DecompileVirtualPropertyWithBaseCall()
        {
            Expression<Func<A, string>> e = @this =>
                @this is E ? "E is child of " + ("C is child of " + "A".ToString()).ToString()
                : @this is D ? "D is child of " + ("C is child of " + "A".ToString()).ToString()
                : @this is C ? "C is child of " + "A".ToString()
                : @this is B ? "B is child of " + "A".ToString()
                : "A";

            var property = typeof(A).GetProperty(nameof(A.M7));
            Assert.NotNull(property);
            Test(e, property.GetGetMethod());
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
