using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using NUnit.Framework;

namespace DelegateDecompiler.Tests
{
    [TestFixture]
    public class AbstractMethodTests:DecompilerTestsBase
    {
        public abstract class A
        {
            public abstract string M1();

            public abstract string M2();
        }

        public class B : A
        {
            public string Me = "B";
            
            public override string M1() => "B";
            
            public override string M2() => Me;
        }

        public class C : A
        {
            public string Me = "C";

            public override string M1() => "C";
           
            public override string M2() => Me;
        }

        public class D : C
        {
            public string Me = "C";
            
            public override string M1() => "D";

            public override string M2() => Me;
        }

        public abstract class E : C
        {
            public string Me = "C";
            
            public override string M1() => "E";
        
            public override string M2() => Me;
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
    }
}