using System.Linq;
using IntSharp;
using IntSharp.Types;
using NUnit.Framework;
using Math = IntSharp.Math;

namespace IntSharpTests
{
    [TestFixture]
    internal class OptimizationTests
    {
        private static Interval F1(IntervalVector x)
        {
            return Math.Sqr(x.Items[0] - 4);
        }

        private static IntervalVector GradF1(IntervalVector x)
        {
            return new IntervalVector(new[]
            {
                    2*(x.Items[0]-4)
                });
        }

        private static IntervalMatrix HessF1(IntervalVector x)
        {
            return new IntervalMatrix(new[,]
            {
                { Interval.FromDoublePrecise(2) }
            });
        }


        private static Interval F2(IntervalVector x)
        {
            return Math.Sqr(x.Items[0] - 4) + Math.Sqr(x.Items[1] - 7);
        }

        private static IntervalVector GradF2(IntervalVector x)
        {
            return new IntervalVector(new[]
            {
                    2*(x.Items[0]-4),
                    2*(x.Items[1]-7)
                });
        }

        private static IntervalMatrix HessF2(IntervalVector x)
        {
            return new IntervalMatrix(new[,]
            {
                { Interval.FromDoublePrecise(2), Interval.Zero },
                {  Interval.Zero, Interval.FromDoublePrecise(2) }
            });
        }


        private static Interval F3(IntervalVector x)
        {
            return Math.Exp(x.Items[0]) + Math.Exp(x.Items[1]);
        }

        private static IntervalVector GradF3(IntervalVector x)
        {
            return new IntervalVector(new[]
            {
                    Math.Exp(x.Items[0]),
                    Math.Exp(x.Items[1])
                });
        }

        private static IntervalMatrix HessF3(IntervalVector x)
        {
            return new IntervalMatrix(new[,]
           {
                { Math.Exp(x.Items[0]), Interval.Zero },
                {  Interval.Zero, Math.Exp(x.Items[1]) }
            });
        }


        private static Interval Rosenbrock(IntervalVector x)
        {
            return 100 * Math.Sqr(x.Items[0] - Math.Sqr(x.Items[1])) + Math.Sqr(x.Items[0] - 1);
        }

        private static IntervalVector GradRosenbrock(IntervalVector x)
        {
            return new IntervalVector(new[]
            {
               400*Math.Pown(x.Items[0],3)+(2-400*x.Items[1])*x.Items[0]-2,
               200*(x.Items[1]-Math.Sqr(x.Items[0]))
            });
        }

        private static IntervalMatrix HessRosenbrock(IntervalVector x)
        {
            return new IntervalMatrix(new[,]
            {
                { 1200*Math.Sqr(x.Items[0])-400*x.Items[1]+2 , -400*x.Items[0] },
                {  -400*x.Items[0], Interval.FromDoublePrecise(200) }
            });
        }


        [Test]
        public void TestOptimization()
        {
            // Test with F1.
            var searchBox = new IntervalVector(new[]
            {
                Interval.FromInfSup(-100, 100)
            });
            var result = Optimization.Optimize(F1, GradF1, HessF1, searchBox, 1e-7);
            Assert.IsTrue(result.Item2.In(0));
            Assert.IsTrue(result.Item1.First().Item1.Items[0].In(4));

            // Test with F2
            searchBox = new IntervalVector(new[]
            {
                Interval.FromInfSup(1, 100),
                Interval.FromInfSup(1, 100)
            });
            result = Optimization.Optimize(F2, GradF2, HessF2, searchBox, 1e-7);
            Assert.IsTrue(result.Item2.In(0));
            Assert.IsTrue(result.Item1.First().Item1.Items[0].In(4));
            Assert.IsTrue(result.Item1.First().Item1.Items[1].In(7));

            // Test with F3
            result = Optimization.Optimize(F3, GradF3, HessF3, searchBox, 1e-7);
            Assert.IsTrue(result.Item2.In(2 * System.Math.Exp(1)));
            Assert.IsTrue(result.Item1.First().Item1.Items[0].In(1));
            Assert.IsTrue(result.Item1.First().Item1.Items[1].In(1));

            // Test with Rosenbrock
            searchBox = new IntervalVector(new[]
            {
                Interval.FromInfSup(-100, 100),
                Interval.FromInfSup(-100, 100)
            });
            result = Optimization.Optimize(Rosenbrock, GradRosenbrock, HessRosenbrock, searchBox, 1e-14);
            Assert.IsTrue(result.Item1.First().Item1.Items[0].In(1));
            Assert.IsTrue(result.Item1.First().Item1.Items[1].In(1));
        }
    }
}
