using IntSharp;
using IntSharp.LinearEquationSystem;
using IntSharp.Types;
using NUnit.Framework;

namespace IntSharpTests.LinearEquationSystem
{
    [TestFixture]
    internal class DetermindedTests
    {
        [Test]
        public void TestDeterminedSolving1()
        {
            var aItems = new[,]
            {
                {Interval.FromDoublePrecise(1), Interval.FromDoublePrecise(1)},
                {Interval.FromDoublePrecise(2), Interval.FromDoublePrecise(1)}
            };

            var bItems = new[]
            {
                Interval.FromDoublePrecise(1),
                Interval.FromDoublePrecise(2)
            };

            var a = new IntervalMatrix(aItems);
            var b = new IntervalVector(bItems);

            var result = Determined.Solve(a, b);
            var slope = result.Items[0];
            var intercept = result.Items[1];

            Assert.AreEqual(1, slope.Infimum, 0.00001);
            Assert.AreEqual(1, slope.Supremum, 0.00001);

            Assert.AreEqual(0, intercept.Infimum, 0.00001);
            Assert.AreEqual(0, intercept.Supremum, 0.00001);
        }

        [Test]
        public void TestDeterminedSolving2()
        {
            var aItems = new[,]
            {
                {Interval.FromMidRad(1,0.05), Interval.FromDoublePrecise(1)},
                {Interval.FromMidRad(2,0.05), Interval.FromDoublePrecise(1)}
            };

            var bItems = new[]
            {
                Interval.FromMidRad(1,0.05),
                Interval.FromMidRad(2,0.05)
            };

            var a = new IntervalMatrix(aItems);
            var b = new IntervalVector(bItems);

            var result = Determined.Solve(a, b);
            var slope = result.Items[0];
            var intercept = result.Items[1];

            Assert.AreEqual(true, slope.In(1));
            Assert.AreEqual(true, intercept.In(0));
        }

        [Test]
        public void TestDeterminedSolving3()
        {
            var aItems = new[,]
            {
                {Interval.FromMidRad(2,0.2), Interval.FromDoublePrecise(1)},
                {Interval.FromMidRad(7,0.2), Interval.FromDoublePrecise(1)}
            };

            var bItems = new[]
            {
                Interval.FromMidRad(4,0.2),
                Interval.FromMidRad(1,0.2)
            };

            var a = new IntervalMatrix(aItems);
            var b = new IntervalVector(bItems);

            var result = Determined.Solve(a, b);
            var slope = result.Items[0];
            var intercept = result.Items[1];

            Assert.AreEqual(true, slope.In(-0.6));
            Assert.AreEqual(true, intercept.In(5.2));
        }
    }
}
