using IntSharp;
using IntSharp.Types;
using MathNet.Numerics.LinearAlgebra;
using NUnit.Framework;

namespace IntSharpTests.Types
{
    [TestFixture]
    internal class IntervalVectorTests
    {
        [Test]
        public void TestGetMidVector()
        {
            var items = new[]
            {
                Interval.FromMidRad(1,0.2),
                Interval.FromMidRad(2,0.2),
                Interval.FromMidRad(3,0.2),
            };

            var vector = new IntervalVector(items).Mid();

            Assert.AreEqual(1, vector.At(0));
            Assert.AreEqual(2, vector.At(1));
            Assert.AreEqual(3, vector.At(2));
        }

        [Test]
        public void TestVectorVectorSubstraction()
        {
            var vector = new IntervalVector(new[]
                  {
                    Interval.FromInfSup(3,7),
                    Interval.FromInfSup(1,2),
                    Interval.FromInfSup(-5,-3)
                }
              );

            var res = vector - vector;

            Assert.IsTrue(res.Items[0].ContainsZero());
            Assert.IsTrue(res.Items[1].ContainsZero());
            Assert.IsTrue(res.Items[2].ContainsZero());
        }

        [Test]
        public void TestVectorVectorAddition()
        {
            var vector = new IntervalVector(new[]
                  {
                    Interval.FromInfSup(3,7),
                    Interval.FromInfSup(1,2)
                }
              );

            var res = vector + vector;

            Assert.AreEqual(6, res.Items[0].Infimum, 0.0001);
            Assert.AreEqual(14, res.Items[0].Supremum, 0.0001);
            Assert.AreEqual(2, res.Items[1].Infimum, 0.0001);
            Assert.AreEqual(4, res.Items[1].Supremum, 0.0001);
        }

        [Test]
        public void TestSum()
        {
            var vector = new IntervalVector(new[]
                  {
                    Interval.FromInfSup(3,7),
                    Interval.FromInfSup(1,2),
                    Interval.FromInfSup(2,5)
                }
              );

            var res = vector.Sum;

            Assert.IsTrue(res.In(6));
            Assert.IsTrue(res.In(14));
        }

        [Test]
        public void TestMean()
        {
            var vector = new IntervalVector(new[]
                  {
                    Interval.FromInfSup(3,7),
                    Interval.FromInfSup(1,2),
                    Interval.FromInfSup(2,5)
                }
              );

            var res = vector.Mean;

            Assert.IsTrue(res.In(2));
            Assert.IsTrue(res.In(4.666));
        }

        [Test]
        public void TestDoubleMatrixVectorMultiplication()
        {
            var lhsItems = new[,] {
                {
                    1.0,2,3
                },
                {
                    4,5,6
                },
                {
                    7,8,9
                }
            };
            var lhs = Matrix<double>.Build.DenseOfArray(lhsItems);

            var rhsItems = new[] { Interval.FromDoublePrecise(1), Interval.FromDoublePrecise(2), Interval.FromDoublePrecise(3) };
            var rhs = new IntervalVector(rhsItems);

            var res = lhs * rhs;

            var expRes = new IntervalVector(new[] {
                Interval.FromDoublePrecise(14),
                Interval.FromDoublePrecise(32),
                Interval.FromDoublePrecise(50)
            });

            for (var row = 0; row < expRes.RowCount; row++)
            {
                Assert.AreEqual(expRes.Items[row].Infimum, res.Items[row].Infimum, 0.0001);
                Assert.AreEqual(expRes.Items[row].Supremum, res.Items[row].Supremum, 0.0001);
            }
        }

        [Test]
        public void TestVectorIntervalMultiplication()
        {
            var vector = new IntervalVector(new[]
                  {
                    Interval.FromInfSup(3,7),
                    Interval.FromInfSup(1,2)
                }
              );

            var interval = Interval.FromDoublePrecise(2);

            var res = vector * interval;

            Assert.AreEqual(6, res.Items[0].Infimum, 0.0001);
            Assert.AreEqual(14, res.Items[0].Supremum, 0.0001);
            Assert.AreEqual(2, res.Items[1].Infimum, 0.0001);
            Assert.AreEqual(4, res.Items[1].Supremum, 0.0001);
        }

        [Test]
        public void TestIntervalVectorIntervalVectorMultiplication()
        {
            var vector = new IntervalVector(new[]
                  {
                    Interval.FromDoublePrecise(2),
                    Interval.FromDoublePrecise(3)
                }
              );

            var res = vector * vector;

            Assert.AreEqual(13, res.Infimum, 0.0001);
            Assert.AreEqual(13, res.Supremum, 0.0001);
        }

        [Test]
        public void TestVectorIntervalAdditionn()
        {
            var vector = new IntervalVector(new[]
                  {
                    Interval.FromInfSup(3,7),
                    Interval.FromInfSup(1,2)
                }
              );

            var interval = Interval.FromDoublePrecise(2);

            var res = vector + interval;

            Assert.AreEqual(5, res.Items[0].Infimum, 0.0001);
            Assert.AreEqual(9, res.Items[0].Supremum, 0.0001);
            Assert.AreEqual(3, res.Items[1].Infimum, 0.0001);
            Assert.AreEqual(4, res.Items[1].Supremum, 0.0001);
        }

        [Test]
        public void TestDoubleVectorVectorAddition()
        {
            var items = new[]
            {
                1.0, 2
            };

            var vector = new IntervalVector(new[]
                  {
                    Interval.FromDoublePrecise(1.0),
                    Interval.FromDoublePrecise(2)
                }
              );

            var doubleVector = Vector<double>.Build.DenseOfArray(items);

            var res = doubleVector + vector;

            Assert.AreEqual(2, res.Items[0].Infimum, 0.0001);
            Assert.AreEqual(2, res.Items[0].Supremum, 0.0001);
            Assert.AreEqual(4, res.Items[1].Infimum, 0.0001);
            Assert.AreEqual(4, res.Items[1].Supremum, 0.0001);
        }
    }
}
