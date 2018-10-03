﻿using System;
using System.Text;
using NUnit.Framework;

namespace BreakInfinity.Tests
{
    [TestFixture]
    public class Tests
    {
        public static BigDouble TestValueExponent4 = BigDouble.Parse("1.23456789e1234");
        public static BigDouble TestValueExponent1 = BigDouble.Parse("1.234567893e3");

        [Test]
        public void TestToString()
        {
            Assert.That(TestValueExponent4.ToString(), Is.EqualTo("1.23456789e+1234"));
        }

        [Test]
        public void TestToExponential()
        {
            Assert.That(TestValueExponent4.ToString("E0"), Is.EqualTo("1e+1234"));
            Assert.That(TestValueExponent4.ToString("E4"), Is.EqualTo("1.2346e+1234"));
            Assert.That(TestValueExponent1.ToString("E0"), Is.EqualTo("1e+3"));
            Assert.That(TestValueExponent1.ToString("E4"), Is.EqualTo("1.2346e+3"));
        }

        [Test]
        public void TestToFixed()
        {
            var aLotOfZeroes = new StringBuilder(1226)
                .Insert(0, "0", 1226)
                .ToString();
            Assert.That(TestValueExponent4.ToString("F0"), Is.EqualTo("123456789" + aLotOfZeroes));
            Assert.That(TestValueExponent4.ToString("F4"), Is.EqualTo("123456789" + aLotOfZeroes + ".0000"));
            Assert.That(TestValueExponent1.ToString("F0"), Is.EqualTo("1235"));
            Assert.That(TestValueExponent1.ToString("F4"), Is.EqualTo("1234.5679"));
        }

        [Test]
        public void TestAdd()
        {
            var addSelf = TestValueExponent4 + TestValueExponent4;
            Assert.That(addSelf.Mantissa, Is.EqualTo(TestValueExponent4.Mantissa * 2));
            Assert.That(addSelf.Exponent, Is.EqualTo(TestValueExponent4.Exponent));
            var oneExponentLess = BigDouble.Parse("1.23456789e1233");
            var addOneExponentLess = TestValueExponent4 + oneExponentLess;
            var expectedMantissa = TestValueExponent4.Mantissa + oneExponentLess.Mantissa / 10;
            Assert.That(addOneExponentLess.Mantissa, Is.EqualTo(expectedMantissa));
            Assert.That(addOneExponentLess.Exponent, Is.EqualTo(TestValueExponent4.Exponent));
            var aLotSmaller = BigDouble.Parse("1.23456789e123");
            var addALotSmaller = TestValueExponent4 + aLotSmaller;
            Assert.That(addALotSmaller.Mantissa, Is.EqualTo(TestValueExponent4.Mantissa));
            Assert.That(addALotSmaller.Exponent, Is.EqualTo(TestValueExponent4.Exponent));
            var negative = BigDouble.Parse("-1.23456789e1234");
            var addNegative = TestValueExponent4 + negative;
            Assert.That(addNegative.Mantissa, Is.EqualTo(0));
            Assert.That(addNegative.Exponent, Is.EqualTo(0));
            var addSmallNumbers = new BigDouble(299) + new BigDouble(18);
            Assert.That(addSmallNumbers.Mantissa, Is.EqualTo(3.17));
            Assert.That(addSmallNumbers.Exponent, Is.EqualTo(2));
        }

        [Test]
        public void TestCompareTo()
        {
            Assert.That(new BigDouble(299).CompareTo(300), Is.EqualTo(-1));
            Assert.That(new BigDouble(299).CompareTo(new BigDouble(299)), Is.EqualTo(0));
            Assert.That(new BigDouble(299).CompareTo(BigDouble.Parse("298")), Is.EqualTo(1));
            Assert.That(new BigDouble(0).CompareTo(0.0), Is.EqualTo(0));
        }

        [Test]
        [Repeat(10000)]
        public void TestDoubleCompatibility()
        {
            var first = BigMath.RandomBigDouble(100);
            var second = BigMath.RandomBigDouble(100);
            var aDouble = first.ToDouble();
            var bDouble = second.ToDouble();
            AssertEqual(first + second, aDouble + bDouble);
            AssertEqual(first - second, aDouble - bDouble);
            AssertEqual(first * second, aDouble * bDouble);
            AssertEqual(first / second, aDouble / bDouble);
            Assert.That(first.CompareTo(second), Is.EqualTo(aDouble.CompareTo(bDouble)));
            var smallNumber = BigDouble.Abs(BigMath.RandomBigDouble(2));
            var smallDouble = Math.Abs(smallNumber.ToDouble());
            AssertEqual(BigDouble.Log(first, smallDouble), Math.Log(aDouble, smallDouble));
            AssertEqual(BigDouble.Pow(first, smallDouble), Math.Pow(aDouble, smallDouble));
        }

        private static void AssertEqual(BigDouble actual, double expected)
        {
            if (BigDouble.IsFinite(actual.ToDouble()) == !BigDouble.IsFinite(expected))
            {
                Assert.Fail($"One of the values is finite, other is not: BigDouble {actual.ToDouble()}, double {expected}");
            }

            if (!BigDouble.IsFinite(expected))
            {
                return;
            }

            if (actual.Exponent < -324 && Math.Abs(expected) < double.Epsilon)
            {
                return;
            }

            // TODO: Inconsistency after e-300
            if (actual.Exponent < -300 && (Math.Log10(expected) < -300 || double.IsNaN(Math.Log10(expected))))
            {
                return;
            }

            Assert.That(actual.Equals(expected, 1E-9));
        }
    }
}