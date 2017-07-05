using IntSharp;
using NUnit.Framework;

namespace IntSharpTests
{
    [TestFixture]
    internal class InflationTests
    {
        [Test]
        public void InflateDownTest()
        {
            var testValue = double.MaxValue;
            var inflatedValue = testValue.InflateDown();
            Assert.Greater(testValue,inflatedValue);

            testValue = double.MinValue;
            inflatedValue = testValue.InflateDown();
            Assert.Greater(testValue, inflatedValue);

            testValue = 0.0;
            inflatedValue = testValue.InflateDown();
            Assert.Greater(testValue, inflatedValue);

            testValue = 0.99999999999999999999999999;
            inflatedValue = testValue.InflateDown();
            Assert.Greater(testValue, inflatedValue);

            testValue = 0.33333333333333333333333333;
            inflatedValue = testValue.InflateDown();
            Assert.Greater(testValue, inflatedValue);

            testValue = -0.99999999999999999999999999;
            inflatedValue = testValue.InflateDown();
            Assert.Greater(testValue, inflatedValue);

            testValue = -0.33333333333333333333333333;
            inflatedValue = testValue.InflateDown();
            Assert.Greater(testValue, inflatedValue);
        }
        
        [Test]
        public void ReturnInfinityIfInfinityIsParameter()
        {
            var inflatedValue = double.PositiveInfinity.InflateDown();
            Assert.AreEqual(double.PositiveInfinity, inflatedValue);

            inflatedValue = double.NegativeInfinity.InflateUp();
            Assert.AreEqual(double.NegativeInfinity, inflatedValue);
        }

        [Test]
        public void InflateUpTest()
        {
            var a = double.MaxValue;
            var inflatedValue = a.InflateUp();
            Assert.Less(a, inflatedValue);

            a = double.MinValue;
            inflatedValue = a.InflateUp();
            Assert.Less(a, inflatedValue);

            a = 0.0;
            inflatedValue = a.InflateUp();
            Assert.Less(a, inflatedValue);

            a = 0.99999999999999999999999999;
            inflatedValue = a.InflateUp();
            Assert.Less(a, inflatedValue);

            a = 0.33333333333333333333333333;
            inflatedValue = a.InflateUp();
            Assert.Less(a, inflatedValue);

            a = -0.99999999999999999999999999;
            inflatedValue = a.InflateUp();
            Assert.Less(a, inflatedValue);

            a = -0.33333333333333333333333333;
            inflatedValue = a.InflateUp();
            Assert.Less(a, inflatedValue);
        }

        [Test]
        public void EpsilonInflationTest()
        {
            var i = IntSharp.Types.Interval.Zero;
            for (var k = 0; k < 1500; k++)
            {
                var iOld = i;
                i = i.EpsilonInflation();
                Assert.IsTrue(iOld.Interior(i));
            }

            var v = IntSharp.Types.IntervalVector.Zero(7);
            for (var k = 0; k < 1500; k++)
            {
                var vOld = v;
                v = v.EpsilonInflation();
                Assert.IsTrue(vOld.Interior(v));
            }
        }
    }
}
