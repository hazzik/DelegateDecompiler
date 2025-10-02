using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using NUnit.Framework;

namespace DelegateDecompiler.Tests
{
    [TestFixture]
    public class StargTests : DecompilerTestsBase
    {
        [Test]
        public void StargProcessor_ShouldHandleStargOpcode()
        {
            // Create a dynamic method that uses Starg opcodes
            var dynamicMethod = new DynamicMethod("TestStarg", typeof(int), new[] { typeof(int) });
            var il = dynamicMethod.GetILGenerator();
            
            // Load argument 0 (parameter)
            il.Emit(OpCodes.Ldarg_0);
            // Load constant 1
            il.Emit(OpCodes.Ldc_I4_1);
            // Add them
            il.Emit(OpCodes.Add);
            // Store back to argument 0 (this generates Starg)
            il.Emit(OpCodes.Starg_S, (byte)0);
            // Load argument 0 and return it
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ret);
            
            // Test that our processor can handle decompiling this method
            var decompiled = dynamicMethod.Decompile();
            
            Assert.That(decompiled, Is.Not.Null);
            Assert.That(decompiled.Body, Is.Not.Null);
            
            Console.WriteLine($"Decompiled dynamic method: {decompiled}");
            Console.WriteLine($"Body: {decompiled.Body}");
            
            // Verify the method actually works
            var compiled = (Func<int, int>)dynamicMethod.CreateDelegate(typeof(Func<int, int>));
            Assert.That(compiled(5), Is.EqualTo(6)); // 5 + 1 = 6
        }

        [Test]
        public void StargProcessor_ShouldHandleStargWithMultipleParameters()
        {
            // Create a dynamic method with multiple parameters using Starg
            var dynamicMethod = new DynamicMethod("TestMultiStarg", typeof(int), new[] { typeof(int), typeof(int) });
            var il = dynamicMethod.GetILGenerator();
            
            // Modify first parameter: arg0 = arg0 + arg1
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Add);
            il.Emit(OpCodes.Starg_S, (byte)0);
            
            // Modify second parameter: arg1 = arg1 * 2
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldc_I4_2);
            il.Emit(OpCodes.Mul);
            il.Emit(OpCodes.Starg, (short)1);  // Use Starg instead of Starg_S
            
            // Return modified first parameter
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ret);
            
            // Test decompilation
            var decompiled = dynamicMethod.Decompile();
            
            Assert.That(decompiled, Is.Not.Null);
            Assert.That(decompiled.Body, Is.Not.Null);
            
            Console.WriteLine($"Decompiled multi-param method: {decompiled}");
            Console.WriteLine($"Body: {decompiled.Body}");
            
            // Verify functionality
            var compiled = (Func<int, int, int>)dynamicMethod.CreateDelegate(typeof(Func<int, int, int>));
            Assert.That(compiled(3, 4), Is.EqualTo(7)); // (3 + 4) = 7, even though arg1 gets modified too
        }
    }
}