using NUnit.Framework;

namespace DelegateDecompiler.Tests
{
    [TestFixture]
    public class Issue46 : DecompilerTestsBase
    {
        [Test]
        public void CanDecompileSelfReferencedProperty()
        {
            var property = typeof(TestClass).GetProperty(nameof(TestClass.FullAddress));
            Assert.That(property, Is.Not.Null);
            Assert.DoesNotThrow(() => property.GetGetMethod().Decompile());
        }

        public class TestClass
        {
            public TestClass Base { get; set; }
            
            public string Part { get; set; }

            [Computed]
            public string FullAddress
            {
                get
                {
                    if (Base == null)
                        return Part;

                    return Base.FullAddress + Part;
                }
            } 
        }
    }
}
