using System;
using System.Linq.Expressions;
using NUnit.Framework;

namespace DelegateDecompiler.Tests
{
    [TestFixture]
    public class AssignmentExpressionTests : DecompilerTestsBase
    {
        class MyClass
        {
            public string MyProperty { get; set; } = "";
        }
        
        [Test]
        public void ShouldDecompilePropertyAssignmentAsAssignmentExpression()
        {
            // Create a function that assigns to a property
            string TestAssignment(MyClass v) => v.MyProperty = "test value";
            Func<MyClass, string> compiled = TestAssignment;
            
            // Decompile it
            LambdaExpression decompiled = DecompileExtensions.Decompile(compiled);
            
            // The body should be a single assignment expression, not a block
            Assert.That(decompiled.Body.NodeType, Is.EqualTo(ExpressionType.Assign));
            Assert.That(decompiled.Body.Type, Is.EqualTo(typeof(string)));
            
            // Verify it's a proper assignment expression
            var assignment = (BinaryExpression)decompiled.Body;
            Assert.That(assignment.Left.NodeType, Is.EqualTo(ExpressionType.MemberAccess));
            Assert.That(assignment.Right.NodeType, Is.EqualTo(ExpressionType.Constant));
            
            var constant = (ConstantExpression)assignment.Right;
            Assert.That(constant.Value, Is.EqualTo("test value"));
        }

        [Test]
        public void ShouldDecompileMultiplePropertyAssignmentAsAssignmentExpression()
        {
            // Test with another property to make sure the fix is general
            DateTime TestPropertyAssignment(TestClass v) => v.StartDate = new DateTime(2023, 1, 1);
            Func<TestClass, DateTime> compiled = TestPropertyAssignment;
            
            // Decompile it
            LambdaExpression decompiled = DecompileExtensions.Decompile(compiled);
            
            // The body should be a single assignment expression, not a block
            Assert.That(decompiled.Body.NodeType, Is.EqualTo(ExpressionType.Assign));
            Assert.That(decompiled.Body.Type, Is.EqualTo(typeof(DateTime)));
        }
    }
}