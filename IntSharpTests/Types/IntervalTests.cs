using System;
using IntSharp;
using IntSharp.Types;
using NUnit.Framework;
using Math = IntSharp.Math;

namespace IntSharpTests.Types
{
    [TestFixture]
    public class IntervalTests
    {
        [Test]
        public void FromInfSupTest()
        {
            // filter [-inf,-inf] and [inf,inf]
            Assert.Throws<Exception>(() => Interval.FromInfSup(double.NegativeInfinity, double.NegativeInfinity));
            Assert.Throws<Exception>(() => Interval.FromInfSup(double.PositiveInfinity, double.PositiveInfinity));

            // filter swaps
            Assert.Throws<Exception>(() => Interval.FromInfSup(2, -3));

            // filter NaNs
            Assert.Throws<Exception>(() => Interval.FromInfSup(double.NaN, 3));
            Assert.Throws<Exception>(() => Interval.FromInfSup(3, double.NaN));

            // valid call
            var i = Interval.FromInfSup(-2, 3);
            Assert.AreEqual(i.Infimum, -2);
            Assert.AreEqual(i.Supremum, 3);
        }

        [Test]
        public void FromMidRadTest()
        {
            // filter negativ radius
            Assert.Throws<Exception>(() => Interval.FromMidRad(0, -1));
            Assert.Throws<Exception>(() => Interval.FromMidRad(0, double.NegativeInfinity));

            // filter infinite mid
            Assert.Throws<Exception>(() => Interval.FromMidRad(double.PositiveInfinity, 1));
            Assert.Throws<Exception>(() => Interval.FromMidRad(double.NegativeInfinity, 1));

            // +inf radius = entire
            Assert.IsTrue(Interval.FromMidRad(3,double.PositiveInfinity).IsEntire());

            // filter NaNs
            Assert.Throws<Exception>(() => Interval.FromMidRad(double.NaN, 3));
            Assert.Throws<Exception>(() => Interval.FromMidRad(3, double.NaN));

            // valid call
            var i = Interval.FromMidRad(2, 1);
            Assert.AreEqual(i.Infimum,1);
            Assert.AreEqual(i.Supremum,3);
        }

        [Test]
        public void FromDoublePrecise()
        {
            // filter inifity
            Assert.Throws<Exception>(() => Interval.FromDoublePrecise(double.PositiveInfinity));
            Assert.Throws<Exception>(() => Interval.FromDoublePrecise(double.NegativeInfinity));

            // filter NaN
            Assert.Throws<Exception>(() => Interval.FromDoublePrecise(double.NaN));

            // valid call
            var i = Interval.FromDoublePrecise(2);
            Assert.AreEqual(i.Infimum,2);
            Assert.AreEqual(i.Supremum,2);
        }
        
        [Test]
        public void FromDoubleWithInflationTest()
        {
            // filter inifity
            Assert.Throws<Exception>(() => Interval.FromDoubleWithEpsilonInflation(double.PositiveInfinity));
            Assert.Throws<Exception>(() => Interval.FromDoubleWithEpsilonInflation(double.NegativeInfinity));

            // filter NaN
            Assert.Throws<Exception>(() => Interval.FromDoubleWithEpsilonInflation(double.NaN));

            // valid call
            var i = Interval.FromDoubleWithEpsilonInflation(2);
            Assert.Greater(i.Supremum,2);
            Assert.Less(i.Infimum,2);
        }
        
        [Test]
        public void ToStringTest()
        {
            var i = Interval.FromInfSup(-2, 2);
            Assert.AreEqual("[ -2 , 2 ]", i.ToString(IntervalFormat.InfSup));
            Assert.AreEqual("< 0 , 2 >", i.ToString(IntervalFormat.MidRad));
        }

        [Test]
        public void ToStringWithDecimalDigitsTest()
        {
            var i = Interval.FromInfSup(-9.8765, 1.2345);

            var res = i.ToString(IntervalFormat.InfSup, 2);
            var expected = Interval.FromInfSup(-9.89, 1.24).ToString(IntervalFormat.InfSup);
            Assert.AreEqual(expected, res);

            res = i.ToString(IntervalFormat.MidRad, 2);
            expected = Interval.FromInfSup(-9.89, 1.24).ToString(IntervalFormat.MidRad);
            Assert.AreEqual(expected, res);

        }

        [Test]
        public void RoundVerifiedTest()
        {
            var i = Interval.FromInfSup(-9.8765, 1.2345);
            var res = i.RoundVerified(2);

            var expected = Interval.FromInfSup(-9.89, 1.24);
            Assert.AreEqual(expected,res);
        }

        [Test]
        public void EqualsTest()
        {
            // not equal if wrong type
            var lhs = Interval.FromInfSup(-2, 3);
            var wrongType = "Wrong type";
            // ReSharper disable once SuspiciousTypeConversion.Global
            Assert.IsFalse(lhs.Equals(wrongType));

            // swapped sign
            lhs = Interval.FromInfSup(-3, 7);
            var rhs = Interval.FromInfSup(3, 7);
            Assert.IsFalse(lhs.Equals(rhs));

            // nearly the same
            lhs = Interval.FromInfSup(-0.0001, 0);
            rhs = Interval.FromInfSup(-0.001, 0);
            Assert.IsFalse(lhs.Equals(rhs));

            // negativ zero
            lhs = Interval.FromInfSup(0, -0);
            rhs = Interval.FromInfSup(-0, 0);
            Assert.IsTrue(lhs.Equals(rhs));
        }
        
        [Test]
        public void IntervalSmallerIntervalTest()
        {
            var lhs = Interval.FromInfSup(-2, 1);

            var rhs = Interval.FromInfSup(1.1, 2);
            Assert.IsTrue(lhs < rhs);
            
            rhs = Interval.FromInfSup(1, 2);
            Assert.IsFalse(lhs < rhs);
        }

        [Test]
        public void IntervalSmallerDoubleTest()
        {
            var lhs = Interval.FromInfSup(-2, 1);

            Assert.IsFalse(lhs < 1);
            Assert.IsTrue(lhs < 1.1);
        }

        [Test]
        public void DoubleSmallerIntervalTest()
        {
            var rhs = Interval.FromInfSup(-2, 1);

            Assert.IsFalse(-2 < rhs);
            Assert.IsTrue(-2.1 < rhs);
        }
        
        [Test]
        public void IntervalSmallerEqualIntervalTest()
        {
            var lhs = Interval.FromInfSup(-2, 1);

            var rhs = Interval.FromInfSup(1.1, 2);
            Assert.IsTrue(lhs <= rhs);

            rhs = Interval.FromInfSup(1, 2);
            Assert.IsTrue(lhs <= rhs);

            rhs = Interval.FromInfSup(0.5, 2);
            Assert.IsFalse(lhs <= rhs);
        }

        [Test]
        public void IntervalSmallerEqualDoubleTest()
        {
            var lhs = Interval.FromInfSup(-2, 1);

            Assert.IsTrue(lhs <= 1);
            Assert.IsTrue(lhs <= 1.1);
            Assert.IsFalse(lhs <= 0.9);
        }

        [Test]
        public void DoubleSmallerEqualIntervalTest()
        {
            var rhs = Interval.FromInfSup(-2, 1);

            Assert.IsTrue(-2 <= rhs);
            Assert.IsTrue(-2.1 <= rhs);
            Assert.IsFalse(-1.9 <= rhs);
        }
        
        [Test]
        public void IntervalBiggerIntervalTest()
        {
            var lhs = Interval.FromInfSup(-2, 1);
            var rhs = Interval.FromInfSup(-4, -2);
            Assert.IsFalse(lhs > rhs);

            rhs = Interval.FromInfSup(-4, -2.1);
            Assert.IsTrue(lhs > rhs);
        }

        [Test]
        public void IntervalBiggerDoubleTest()
        {
            var lhs = Interval.FromInfSup(-2, 1);
            Assert.IsFalse(lhs > -2);
            Assert.IsTrue(lhs > -2.1);
        }

        [Test]
        public void DoubleBiggerIntervalTest()
        {
            var rhs = Interval.FromInfSup(-2, 1);
            Assert.IsFalse(1 > rhs);
            Assert.IsTrue(1.1 > rhs);
        }

        [Test]
        public void IntervalBiggerEqualIntervalTest()
        {
            var lhs = Interval.FromInfSup(1, 2);
            var rhs = Interval.FromInfSup(-1, 1);
            Assert.IsTrue(lhs >= rhs);

            rhs = Interval.FromInfSup(-1, 1.1);
            Assert.IsFalse(lhs >= rhs);
        }

        [Test]
        public void IntervalBiggerEqualDoubleTest()
        {
            var lhs = Interval.FromInfSup(-2, 1);
            Assert.IsTrue(lhs >= -2);
            Assert.IsFalse(lhs >= -1.9);
        }

        [Test]
        public void DoubleBiggerEqualIntervalTest()
        {
            var rhs = Interval.FromInfSup(-2, 1);
            Assert.IsTrue(1 >= rhs);
            Assert.IsFalse(0.9 >= rhs);
        }
        
        [Test]
        public void TestIntervalIntervalAddition()
        {
            // [-3,7]+[-7,3]=[-10,10]
            var lhs = Interval.FromInfSup(-3, 7);
            var rhs = Interval.FromInfSup(-7, 3);
            var res = lhs + rhs;
            Assert.AreEqual(res.Infimum, -10, 0.0001);
            Assert.AreEqual(res.Supremum, 10, 0.0001);
        }
        
        [Test]
        public void TestIntervalDoubleAddition()
        {
            // [-3,7]+3=[0,10]
            var lhs = Interval.FromInfSup(-3, 7);
            var res = lhs + 3;
            Assert.AreEqual(res.Infimum, 0, 0.0001);
            Assert.AreEqual(res.Supremum, 10, 0.0001);
        }

        [Test]
        public void TestDoubleIntervalAddition()
        {
            // 3+[-3,7]=[0,10]
            var rhs = Interval.FromInfSup(-3, 7);
            var res = 3 + rhs;
            Assert.AreEqual(res.Infimum, 0, 0.0001);
            Assert.AreEqual(res.Supremum, 10, 0.0001);
        }
        
        [Test]
        public void TestIntervalIntervalDivision()
        {
            // [-7,-3]/[3,7]=[-2.333,-0.428]
            var lhs = Interval.FromInfSup(-7, -3);
            var rhs = Interval.FromInfSup(3, 7);
            var res = lhs / rhs;
            Assert.AreEqual(-2.333, res.Infimum, 0.001);
            Assert.AreEqual(-0.428, res.Supremum, 0.001);
        }

        [Test]
        public void TestIntervalDoubleDivision()
        {
            // [-3,7]/3=[-1,2.333]
            var lhs = Interval.FromInfSup(-3, 7);
            var res = lhs / 3;
            Assert.AreEqual(-1, res.Infimum, 0.001);
            Assert.AreEqual(2.333, res.Supremum, 0.001);
        }
        
        [Test]
        public void TestDoubleIntervalDivision()
        {
            // 3/[3,7]=[0.428,1]
            var rhs = Interval.FromInfSup(3, 7);
            var res = 3 / rhs;
            Assert.AreEqual(0.428, res.Infimum, 0.001);
            Assert.AreEqual(1, res.Supremum, 0.001);
        }
        
        [Test]
        public void TestIntervalIntervalMultiplication()
        {
            // [-3,7]*[-7,3]=[-49,21]
            var lhs = Interval.FromInfSup(-3, 7);
            var rhs = Interval.FromInfSup(-7, 3);
            var res = lhs * rhs;
            Assert.AreEqual(-49, res.Infimum, 0.0001);
            Assert.AreEqual(21, res.Supremum, 0.0001);
        }

        [Test]
        public void TestIntervalDoubleMultiplication()
        {
            // [-3,7]*3=[-9,21]
            var lhs = Interval.FromInfSup(-3, 7);
            var res = lhs * 3;
            Assert.AreEqual(-9, res.Infimum, 0.0001);
            Assert.AreEqual(21, res.Supremum, 0.0001);
        }
       
        [Test]
        public void TestDoubleIntervalMultiplication()
        {
            // 3*[-3,7]=[-9,21]
            var rhs = Interval.FromInfSup(-3, 7);
            var res = 3 * rhs;
            Assert.AreEqual(-9, res.Infimum, 0.0001);
            Assert.AreEqual(21, res.Supremum, 0.0001);
        }
       
        [Test]
        public void TestIntervalIntervalSubstraction()
        {
            // [-3,7]-[-7,3]=[-6,14]
            var lhs = Interval.FromInfSup(-3, 7);
            var rhs = Interval.FromInfSup(-7, 3);
            var res = lhs - rhs;
            Assert.AreEqual(-6, res.Infimum, 0.0001);
            Assert.AreEqual(14, res.Supremum, 0.0001);
        }

        [Test]
        public void TestIntervalDoubleSubstraction()
        {
            // [-3,7]-3=[-6,4]
            var lhs = Interval.FromInfSup(-3, 7);
            var res = lhs - 3;
            Assert.AreEqual(-6, res.Infimum, 0.0001);
            Assert.AreEqual(4, res.Supremum, 0.0001);
        }
        
        [Test]
        public void TestDoubleIntervalSubstraction()
        {
            // 3-[-3,7]=[-4,6]
            var rhs = Interval.FromInfSup(-3, 7);
            var res = 3 - rhs;
            Assert.AreEqual(-4, res.Infimum, 0.0001);
            Assert.AreEqual(6, res.Supremum, 0.0001);
        }

        [Test]
        public void TestIntervalIntervalVectorMultiplication()
        {
            var interval = Interval.FromInfSup(2, 3);
            var vector = new IntervalVector(new[]{ Interval.FromDoublePrecise(2), Interval.FromDoublePrecise(3)});

            var result = interval*vector;
            Assert.AreEqual(result.Items[0],vector.Items[0]*interval);
        }

        [Test]
        public void TestPropagationOfErrorExample()
        {
            // Init interval with MidRad factory
            var height = Interval.FromMidRad(3.8, 0.2);

            // Init interval with InfSup factory
            var accelerationOfGravity = Interval.FromInfSup(9.8, 9.82);

            // Calculate the result
            var velocity = Math.Sqrt(2 * height * accelerationOfGravity);

            // Assert if result is rigorously bounded
            var expectedVelocity = Interval.FromInfSup(8.4, 8.863);
            Assert.True(expectedVelocity.Subset(velocity));
        }
    }
}