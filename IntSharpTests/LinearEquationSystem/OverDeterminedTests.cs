using IntSharp;
using IntSharp.LinearEquationSystem;
using IntSharp.Types;
using NUnit.Framework;

namespace IntSharpTests.LinearEquationSystem
{
    [TestFixture]
    internal class OverDeterminedTests
    {
        [Test]
        public void TestOverDeterminedSolvingWithNarrowIntervals()
        {
            var aItems = new[,]
            {
                {Interval.FromDoublePrecise(1),Interval.FromDoublePrecise(1)},
                {Interval.FromDoublePrecise(1),Interval.FromDoublePrecise(1)},
                {Interval.FromDoublePrecise(2),Interval.FromDoublePrecise(1)},
                {Interval.FromDoublePrecise(2),Interval.FromDoublePrecise(1)},
                {Interval.FromDoublePrecise(3),Interval.FromDoublePrecise(1)},
                {Interval.FromDoublePrecise(3),Interval.FromDoublePrecise(1)},
                {Interval.FromDoublePrecise(4),Interval.FromDoublePrecise(1)},
                {Interval.FromDoublePrecise(4),Interval.FromDoublePrecise(1)}
            };

            var bItems = new[]
            {
                Interval.FromDoublePrecise(0),
                Interval.FromDoublePrecise(2),
                Interval.FromDoublePrecise(1),
                Interval.FromDoublePrecise(3),
                Interval.FromDoublePrecise(2),
                Interval.FromDoublePrecise(4),
                Interval.FromDoublePrecise(3),
                Interval.FromDoublePrecise(5)
            };

            var a = new IntervalMatrix(aItems);
            var b = new IntervalVector(bItems);

            var result = OverDetermined.Solve(a, b);
            var slope = result.Items[0];
            var intercept = result.Items[1];

            Assert.IsTrue(slope.In(1));
            Assert.IsTrue(intercept.In(0));
        }

        [Test]
        public void TestOverDeterminedSolvingWithBuggedIntervals()
        {
            var aItems = new[,]
            {
                {Interval.FromMidRad(1,0.05),Interval.FromDoublePrecise(1)},
                {Interval.FromMidRad(1,0.05),Interval.FromDoublePrecise(1)},
                {Interval.FromMidRad(2,0.05),Interval.FromDoublePrecise(1)},
                {Interval.FromMidRad(2,0.05),Interval.FromDoublePrecise(1)},
                {Interval.FromMidRad(3,0.05),Interval.FromDoublePrecise(1)},
                {Interval.FromMidRad(3,0.05),Interval.FromDoublePrecise(1)},
                {Interval.FromMidRad(4,0.05),Interval.FromDoublePrecise(1)},
                {Interval.FromMidRad(4,0.05),Interval.FromDoublePrecise(1)}
            };

            var bItems = new[]
            {
                Interval.FromMidRad(0,0.05),
                Interval.FromMidRad(2,0.05),
                Interval.FromMidRad(1,0.05),
                Interval.FromMidRad(3,0.05),
                Interval.FromMidRad(2,0.05),
                Interval.FromMidRad(4,0.05),
                Interval.FromMidRad(3,0.05),
                Interval.FromMidRad(5,0.05)
            };

            var a = new IntervalMatrix(aItems);
            var b = new IntervalVector(bItems);

            var result = OverDetermined.Solve(a, b);
            var slope = result.Items[0];
            var intercept = result.Items[1];

            Assert.AreEqual(true, slope.In(1));
            Assert.AreEqual(true, intercept.In(0));
        }
    }
}
