using System;
using System.Linq.Expressions;
using NUnit.Framework;

namespace DelegateDecompiler.Tests
{
    [TestFixture]
    public class IntegerConstantTests : DecompilerTestsBase
    {
        [Test]
        public void MethodAcceptingBoolean()
        {
            Expression<Func<string>> expected = () => Method(true);
            Func<string> compiled = () => Method(true);
            Test(expected, compiled);
        }

        [Test]
        public void ConstantBooleanToString()
        {
            Expression<Func<string>> expected = () => true.ToString();
            Func<string> compiled = () => true.ToString();
            Test(expected, compiled);
        }

        [Test]
        public void ParameterBooleanToString()
        {
            Expression<Func<bool, string>> expected = x => x.ToString();
            Func<bool, string> compiled = x => x.ToString();
            Test(expected, compiled);
        }

        public static string Method(bool arg)
        {
            return arg.ToString();
        }

        [Test]
        public void MethodAcceptingByte()
        {
            Expression<Func<string>> expected = () => Method((byte)1);
            Func<string> compiled = () => Method((byte)1);
            Test(expected, compiled);
        }

        [Test]
        public void ConstantByteToString()
        {
            Expression<Func<string>> expected = () => ((byte)1).ToString();
            Func<string> compiled = () => ((byte)1).ToString();
            Test(expected, compiled);
        }

        [Test]
        public void ParameterByteToString()
        {
            Expression<Func<byte, string>> expected = x => x.ToString();
            Func<byte, string> compiled = x => x.ToString();
            Test(expected, compiled);
        }

        public static string Method(byte arg)
        {
            return arg.ToString();
        }

        [Test]
        public void MethodAcceptingSByte()
        {
            Expression<Func<string>> expected = () => Method((sbyte)1);
            Func<string> compiled = () => Method((sbyte)1);
            Test(expected, compiled);
        }

        [Test]
        public void ConstantSByteToString()
        {
            Expression<Func<string>> expected = () => ((sbyte)1).ToString();
            Func<string> compiled = () => ((sbyte)1).ToString();
            Test(expected, compiled);
        }

        [Test]
        public void ParameterSByteToString()
        {
            Expression<Func<sbyte, string>> expected = x => x.ToString();
            Func<sbyte, string> compiled = x => x.ToString();
            Test(expected, compiled);
        }

        public static string Method(sbyte arg)
        {
            return arg.ToString();
        }

        [Test]
        public void MethodAcceptingInt16()
        {
            Expression<Func<string>> expected = () => Method((short)1);
            Func<string> compiled = () => Method((short)1);
            Test(expected, compiled);
        }

        [Test]
        public void ConstantInt16ToString()
        {
            Expression<Func<string>> expected = () => ((short)1).ToString();
            Func<string> compiled = () => ((short)1).ToString();
            Test(expected, compiled);
        }

        [Test]
        public void ParameterInt16ToString()
        {
            Expression<Func<short, string>> expected = x => x.ToString();
            Func<short, string> compiled = x => x.ToString();
            Test(expected, compiled);
        }

        public static string Method(short arg)
        {
            return arg.ToString();
        }

        [Test]
        public void MethodAcceptingUInt16()
        {
            Expression<Func<string>> expected = () => Method((ushort)1);
            Func<string> compiled = () => Method((ushort)1);
            Test(expected, compiled);
        }

        [Test]
        public void ConstantUInt16ToString()
        {
            Expression<Func<string>> expected = () => ((ushort)1).ToString();
            Func<string> compiled = () => ((ushort)1).ToString();
            Test(expected, compiled);
        }

        [Test]
        public void ParameterUInt16ToString()
        {
            Expression<Func<ushort, string>> expected = x => x.ToString();
            Func<ushort, string> compiled = x => x.ToString();
            Test(expected, compiled);
        }

        public static string Method(ushort arg)
        {
            return arg.ToString();
        }

        [Test]
        public void MethodAcceptingInt32()
        {
            Expression<Func<string>> expected = () => Method((int)1);
            Func<string> compiled = () => Method((int)1);
            Test(expected, compiled);
        }

        [Test]
        public void ConstantInt32ToString()
        {
            Expression<Func<string>> expected = () => ((int)1).ToString();
            Func<string> compiled = () => ((int)1).ToString();
            Test(expected, compiled);
        }

        [Test]
        public void ParameterInt32ToString()
        {
            Expression<Func<int, string>> expected = x => x.ToString();
            Func<int, string> compiled = x => x.ToString();
            Test(expected, compiled);
        }

        public static string Method(int arg)
        {
            return arg.ToString();
        }

        [Test]
        public void MethodAcceptingUInt32()
        {
            Expression<Func<string>> expected = () => Method((uint)1);
            Func<string> compiled = () => Method((uint)1);
            Test(expected, compiled);
        }

        [Test]
        public void ConstantUInt32ToString()
        {
            Expression<Func<string>> expected = () => ((uint)1).ToString();
            Func<string> compiled = () => ((uint)1).ToString();
            Test(expected, compiled);
        }

        [Test]
        public void ParameterUInt32ToString()
        {
            Expression<Func<uint, string>> expected = x => x.ToString();
            Func<uint, string> compiled = x => x.ToString();
            Test(expected, compiled);
        }

        public static string Method(uint arg)
        {
            return arg.ToString();
        }

        [Test, Ignore("Acceptable difference")]
        public void MethodAcceptingInt64()
        {
            Expression<Func<string>> expected = () => Method((long)1);
            Func<string> compiled = () => Method((long)1);
            Test(expected, compiled);
        }

        [Test, Ignore("Acceptable difference")]
        public void ConstantInt64ToString()
        {
            Expression<Func<string>> expected = () => ((long)1).ToString();
            Func<string> compiled = () => ((long)1).ToString();
            Test(expected, compiled);
        }

        [Test]
        public void ParameterInt64ToString()
        {
            Expression<Func<long, string>> expected = x => x.ToString();
            Func<long, string> compiled = x => x.ToString();
            Test(expected, compiled);
        }

        public static string Method(long arg)
        {
            return arg.ToString();
        }

        [Test, Ignore("Not fixed yet")]
        public void MethodAcceptingUInt64()
        {
            Expression<Func<string>> expected = () => Method((ulong)1);
            Func<string> compiled = () => Method((ulong)1);
            Test(expected, compiled);
        }

        [Test, Ignore("Not fixed yet")]
        public void ConstantUInt64ToString()
        {
            Expression<Func<string>> expected = () => ((ulong)1).ToString();
            Func<string> compiled = () => ((ulong)1).ToString();
            Test(expected, compiled);
        }

        [Test]
        public void ParameterUInt64ToString()
        {
            Expression<Func<ulong, string>> expected = x => x.ToString();
            Func<ulong, string> compiled = x => x.ToString();
            Test(expected, compiled);
        }

        public static string Method(ulong arg)
        {
            return arg.ToString();
        }

        [Test]
        public void MethodAcceptingSingle()
        {
            Expression<Func<string>> expected = () => Method((float)1);
            Func<string> compiled = () => Method((float)1);
            Test(expected, compiled);
        }

        [Test]
        public void ConstantSingleToString()
        {
            Expression<Func<string>> expected = () => ((float)1).ToString();
            Func<string> compiled = () => ((float)1).ToString();
            Test(expected, compiled);
        }

        [Test]
        public void ParameterSingleToString()
        {
            Expression<Func<float, string>> expected = x => x.ToString();
            Func<float, string> compiled = x => x.ToString();
            Test(expected, compiled);
        }

        public static string Method(float arg)
        {
            return arg.ToString();
        }

        [Test]
        public void MethodAcceptingDouble()
        {
            Expression<Func<string>> expected = () => Method((double)1);
            Func<string> compiled = () => Method((double)1);
            Test(expected, compiled);
        }

        [Test]
        public void ConstantDoubleToString()
        {
            Expression<Func<string>> expected = () => ((double)1).ToString();
            Func<string> compiled = () => ((double)1).ToString();
            Test(expected, compiled);
        }

        [Test]
        public void ParameterDoubleToString()
        {
            Expression<Func<double, string>> expected = x => x.ToString();
            Func<double, string> compiled = x => x.ToString();
            Test(expected, compiled);
        }

        public static string Method(double arg)
        {
            return arg.ToString();
        }

        [Test, Ignore("Compiler optimizes code")]
        public void MethodAcceptingDecimalOne()
        {
            Expression<Func<string>> expected1 = () => Method((decimal)1);
            Expression<Func<string>> expected2 = () => Method(decimal.One);
            Func<string> compiled = () => Method((decimal)1);
            Test(expected1, expected2, compiled);
        }

        [Test, Ignore("Compiler optimizes code")]
        public void ConstantDecimalOneToString()
        {
            Expression<Func<string>> expected1 = () => ((decimal)1).ToString();
            Expression<Func<string>> expected2 = () => decimal.One.ToString();
            Func<string> compiled = () => ((decimal)1).ToString();
            Test(expected1, expected2, compiled);
        }

        [Test]
        public void ParameterDecimalToString()
        {
            Expression<Func<decimal, string>> expected = x => x.ToString();
            Func<decimal, string> compiled = x => x.ToString();
            Test(expected, compiled);
        }

        [Test]
        public void MethodAcceptingDecimal()
        {
            Expression<Func<string>> expected1 = () => Method((decimal)1.23);
            Expression<Func<string>> expected2 = () => Method(new decimal(123, 0, 0, false, 2));
            Func<string> compiled = () => Method((decimal)1.23);
            Test(expected1, expected2, compiled);
        }

        [Test]
        public void ConstantDecimalToString()
        {
            Expression<Func<string>> expected1 = () => ((decimal)1.23).ToString();
            Expression<Func<string>> expected2 = () => new decimal(123, 0, 0, false, 2).ToString();
            Func<string> compiled = () => ((decimal)1.23).ToString();
            Test(expected1, expected2, compiled);
        }

        public static string Method(decimal arg)
        {
            return arg.ToString();
        }
    }
}