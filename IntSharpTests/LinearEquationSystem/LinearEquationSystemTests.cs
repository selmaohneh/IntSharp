using System;
using IntSharp;
using IntSharp.Types;
using NUnit.Framework;

namespace IntSharpTests.LinearEquationSystem
{
    [TestFixture]
    public class LinearEquationTests
    {
        [Test]
        public void TestUnderdeterminedSolving()
        {
            var aItems = new[,]
            {
                {Interval.FromDoublePrecise(2), Interval.FromDoublePrecise(1)},
                {Interval.FromDoublePrecise(4), Interval.FromDoublePrecise(2)}
            };

            var bItems = new[]
            {
                Interval.FromDoublePrecise(4),
                Interval.FromDoublePrecise(8)
            };

            var a = new IntervalMatrix(aItems);
            var b = new IntervalVector(bItems);

            Assert.Throws<Exception>(()=>IntSharp.LinearEquationSystem.LinearEquationSystem.Solve(a, b, false));
        }

        [Test]
        public void TestSolve()
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

            var result = IntSharp.LinearEquationSystem.LinearEquationSystem.Solve(a, b, false);
            var slope = result.Items[0];
            var intercept = result.Items[1];

            Assert.IsTrue(slope.In(1));
            Assert.IsTrue(intercept.In(0));
        }
    }
}
