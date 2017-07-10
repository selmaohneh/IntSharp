using System;
using IntSharp;
using IntSharp.Types;
using MathNet.Numerics.LinearAlgebra;
using NUnit.Framework;

namespace IntSharpTests.Types
{
    [TestFixture]
    public class IntervalMatrixTests
    {
        [Test]
        public void TestGetRow()
        {
            var items = new[,]
            {
                {Interval.FromDoublePrecise(1), Interval.FromDoublePrecise(2)},
                {Interval.FromDoublePrecise(3), Interval.FromDoublePrecise(4)},
                {Interval.FromDoublePrecise(5), Interval.FromDoublePrecise(6)}
            };

            var matrix = new IntervalMatrix(items);

            var row = matrix.GetRow(1);

            Assert.IsTrue(row.Items[0].In(3));
            Assert.IsTrue(row.Items[1].In(4));

            Assert.Throws<Exception>(() => matrix.GetRow(17));
        }

        [Test]
        public void TestGetColumn()
        {
            var items = new[,]
            {
                {Interval.FromDoublePrecise(1), Interval.FromDoublePrecise(2)},
                {Interval.FromDoublePrecise(3), Interval.FromDoublePrecise(4)},
                {Interval.FromDoublePrecise(5), Interval.FromDoublePrecise(6)}
            };

            var matrix = new IntervalMatrix(items);
            var column = matrix.GetColumn(0);

            Assert.AreEqual(column.Items[0], matrix.Items[0, 0]);
            Assert.AreEqual(column.Items[1], matrix.Items[1, 0]);
            Assert.AreEqual(column.Items[2], matrix.Items[2, 0]);

            column = matrix.GetColumn(1);

            Assert.AreEqual(column.Items[0], matrix.Items[0, 1]);
            Assert.AreEqual(column.Items[1], matrix.Items[1, 1]);
            Assert.AreEqual(column.Items[2], matrix.Items[2, 1]);
        }

        [Test]
        public void TestGetMidMatrix()
        {
            var items = new[,]
            {
                {Interval.FromMidRad(1,0.2), Interval.FromMidRad(2,0.2)},
                {Interval.FromMidRad(3,0.2), Interval.FromMidRad(4,0.2)},
                {Interval.FromMidRad(5,0.2), Interval.FromMidRad(6,0.2)}
            };

            var matrix = new IntervalMatrix(items).Mid();

            Assert.AreEqual(3, matrix.At(1, 0));
            Assert.AreEqual(5, matrix.At(2, 0));
            Assert.AreEqual(6, matrix.At(2, 1));
        }

        [Test]
        public void TestTranspose()
        {
            var items = new[,]
            {
                {Interval.FromDoublePrecise(1), Interval.FromDoublePrecise(2)},
                {Interval.FromDoublePrecise(3), Interval.FromDoublePrecise(4)},
                {Interval.FromDoublePrecise(5), Interval.FromDoublePrecise(6)}
            };

            var matrix = new IntervalMatrix(items);
            var transposedMatrix = matrix.Transpose();

            for (var row = 0; row < matrix.RowCount; row++)
            {
                for (var col = 0; col < matrix.ColumnCount; col++)
                {
                    Assert.AreEqual(matrix.Items[row,col],transposedMatrix.Items[col,row]);
                }
            }
        }

        [Test]
        public void TestIntervalMatrixIntervalMatrixAddition()
        {
            var lhs = new IntervalMatrix(new[,] {
            {
                Interval.FromDoublePrecise(1),Interval.FromDoublePrecise(2),Interval.FromDoublePrecise(3)
            },
            {
                Interval.FromDoublePrecise(4),Interval.FromDoublePrecise(5),Interval.FromDoublePrecise(6)
            },
            {
                Interval.FromDoublePrecise(7),Interval.FromDoublePrecise(8),Interval.FromDoublePrecise(9)
            }
            });

            var res = lhs + lhs;

            var expRes = new IntervalMatrix(new[,] {
            {
                Interval.FromDoublePrecise(2),Interval.FromDoublePrecise(4),Interval.FromDoublePrecise(6)
            },
            {
                Interval.FromDoublePrecise(8),Interval.FromDoublePrecise(10),Interval.FromDoublePrecise(12)
            },
            {
                Interval.FromDoublePrecise(14),Interval.FromDoublePrecise(16),Interval.FromDoublePrecise(18)
            }
            });

            for (var row = 0; row < expRes.RowCount; row++)
            {
                for (var col = 0; col < expRes.RowCount; col++)
                {
                    Assert.AreEqual(expRes.Items[row, col].Infimum, res.Items[row, col].Infimum, 0.0001);
                    Assert.AreEqual(expRes.Items[row, col].Supremum, res.Items[row, col].Supremum, 0.0001);
                }
            }
        }

        [Test]
        public void TestIntervalMatrixIntervalAddition()
        {
            var lhs = new IntervalMatrix(new[,] {
            {
                Interval.FromDoublePrecise(1),Interval.FromDoublePrecise(2),Interval.FromDoublePrecise(3)
            },
            {
                Interval.FromDoublePrecise(4),Interval.FromDoublePrecise(5),Interval.FromDoublePrecise(6)
            },
            {
                Interval.FromDoublePrecise(7),Interval.FromDoublePrecise(8),Interval.FromDoublePrecise(9)
            }
            });

            var rhs = Interval.FromMidRad(1, 0.00001);

            var res = lhs + rhs;

            var expRes = new IntervalMatrix(new[,] {
            {
                Interval.FromDoublePrecise(2),Interval.FromDoublePrecise(3),Interval.FromDoublePrecise(4)
            },
            {
                Interval.FromDoublePrecise(5),Interval.FromDoublePrecise(6),Interval.FromDoublePrecise(7)
            },
            {
                Interval.FromDoublePrecise(8),Interval.FromDoublePrecise(9),Interval.FromDoublePrecise(10)
            }
            });

            for (var row = 0; row < expRes.RowCount; row++)
            {
                for (var col = 0; col < expRes.RowCount; col++)
                {
                    Assert.AreEqual(expRes.Items[row, col].Infimum, res.Items[row, col].Infimum, 0.0001);
                    Assert.AreEqual(expRes.Items[row, col].Supremum, res.Items[row, col].Supremum, 0.0001);
                }
            }
        }

        [Test]
        public void TestDoubleMatrixIntervalMatrixAddition()
        {
            var items = new[,] {{1.0, 2, 3}, {4.0, 5, 6}, {7.0, 8, 9}};
            var lhs = Matrix<double>.Build.DenseOfArray(items);

            var rhs = new IntervalMatrix(new[,] {
            {
                Interval.FromDoublePrecise(1),Interval.FromDoublePrecise(2),Interval.FromDoublePrecise(3)
            },
            {
                Interval.FromDoublePrecise(4),Interval.FromDoublePrecise(5),Interval.FromDoublePrecise(6)
            },
            {
                Interval.FromDoublePrecise(7),Interval.FromDoublePrecise(8),Interval.FromDoublePrecise(9)
            }
            });

            var res = lhs + rhs;

            var expRes = new IntervalMatrix(new[,] {
            {
                Interval.FromDoublePrecise(2),Interval.FromDoublePrecise(4),Interval.FromDoublePrecise(6)
            },
            {
                Interval.FromDoublePrecise(8),Interval.FromDoublePrecise(10),Interval.FromDoublePrecise(12)
            },
            {
                Interval.FromDoublePrecise(14),Interval.FromDoublePrecise(16),Interval.FromDoublePrecise(18)
            }
            });

            for (var row = 0; row < expRes.RowCount; row++)
            {
                for (var col = 0; col < expRes.RowCount; col++)
                {
                    Assert.AreEqual(expRes.Items[row, col].Infimum, res.Items[row, col].Infimum, 0.0001);
                    Assert.AreEqual(expRes.Items[row, col].Supremum, res.Items[row, col].Supremum, 0.0001);
                }
            }
        }

        [Test]
        public void TestIntervalMatrixIntervalMatrixSubstraction()
        {
            var lhs = new IntervalMatrix(new[,] {
            {
                Interval.FromDoublePrecise(1),Interval.FromDoublePrecise(2),Interval.FromDoublePrecise(3)
            },
            {
                Interval.FromDoublePrecise(4),Interval.FromDoublePrecise(5),Interval.FromDoublePrecise(6)
            },
            {
                Interval.FromDoublePrecise(7),Interval.FromDoublePrecise(8),Interval.FromDoublePrecise(9)
            }
            });

            var res = lhs - lhs;

            var expRes = new IntervalMatrix(new[,] {
            {
                Interval.Zero, Interval.Zero, Interval.Zero
            },
            {
                 Interval.Zero, Interval.Zero, Interval.Zero
            },
            {
                Interval.Zero, Interval.Zero, Interval.Zero
            }
            });

            for (var row = 0; row < expRes.RowCount; row++)
            {
                for (var col = 0; col < expRes.RowCount; col++)
                {
                    Assert.AreEqual(expRes.Items[row, col].Infimum, res.Items[row, col].Infimum, 0.0001);
                    Assert.AreEqual(expRes.Items[row, col].Supremum, res.Items[row, col].Supremum, 0.0001);
                }
            }
        }

        [Test]
        public void TestDoubleMatrixIntervalMatrixSubstraction()
        {
            var items = new[,] { { 1.0, 2, 3 }, { 4.0, 5, 6 }, { 7.0, 8, 9 } };
            var lhs = Matrix<double>.Build.DenseOfArray(items);

            var rhs = new IntervalMatrix(new[,] {
            {
                Interval.FromDoublePrecise(1),Interval.FromDoublePrecise(2),Interval.FromDoublePrecise(3)
            },
            {
                Interval.FromDoublePrecise(4),Interval.FromDoublePrecise(5),Interval.FromDoublePrecise(6)
            },
            {
                Interval.FromDoublePrecise(7),Interval.FromDoublePrecise(8),Interval.FromDoublePrecise(9)
            }
            });

            var res = lhs - rhs;

            var expRes = new IntervalMatrix(new[,] {
            {
                Interval.Zero, Interval.Zero, Interval.Zero
            },
            {
                 Interval.Zero, Interval.Zero, Interval.Zero
            },
            {
                Interval.Zero, Interval.Zero, Interval.Zero
            }
            });

            for (var row = 0; row < expRes.RowCount; row++)
            {
                for (var col = 0; col < expRes.RowCount; col++)
                {
                    Assert.AreEqual(expRes.Items[row, col].Infimum, res.Items[row, col].Infimum, 0.0001);
                    Assert.AreEqual(expRes.Items[row, col].Supremum, res.Items[row, col].Supremum, 0.0001);
                }
            }
        }

        [Test]
        public void TestIntervalMatrixIntervalMatrixMultiplication()
        {
            var lhs = new IntervalMatrix(new[,] {
            {
                Interval.FromDoublePrecise(1),Interval.FromDoublePrecise(2),Interval.FromDoublePrecise(3)
            },
            {
                Interval.FromDoublePrecise(4),Interval.FromDoublePrecise(5),Interval.FromDoublePrecise(6)
            },
            {
                Interval.FromDoublePrecise(7),Interval.FromDoublePrecise(8),Interval.FromDoublePrecise(9)
            }
            });

            var res = lhs * lhs;

            var expRes = new IntervalMatrix(new[,] {
           {
                Interval.FromDoublePrecise(30),Interval.FromDoublePrecise(36),Interval.FromDoublePrecise(42)
            },
            {
                Interval.FromDoublePrecise(66),Interval.FromDoublePrecise(81),Interval.FromDoublePrecise(96)
            },
            {
                Interval.FromDoublePrecise(102),Interval.FromDoublePrecise(126),Interval.FromDoublePrecise(150)
            }
            });

            for (var row = 0; row < expRes.RowCount; row++)
            {
                for (var col = 0; col < expRes.RowCount; col++)
                {
                    Assert.AreEqual(expRes.Items[row, col].Infimum, res.Items[row, col].Infimum, 0.0001);
                    Assert.AreEqual(expRes.Items[row, col].Supremum, res.Items[row, col].Supremum, 0.0001);
                }
            }
        }

        [Test]
        public void TestDoubleMatrixIntervalMatrixMultiplications()
        {
            var items = new[,] { { 1.0, 2, 3 }, { 4.0, 5, 6 }, { 7.0, 8, 9 } };
            var lhs = Matrix<double>.Build.DenseOfArray(items);

            var rhs = new IntervalMatrix(new[,] {
            {
                Interval.FromDoublePrecise(1),Interval.FromDoublePrecise(2),Interval.FromDoublePrecise(3)
            },
            {
                Interval.FromDoublePrecise(4),Interval.FromDoublePrecise(5),Interval.FromDoublePrecise(6)
            },
            {
                Interval.FromDoublePrecise(7),Interval.FromDoublePrecise(8),Interval.FromDoublePrecise(9)
            }
            });

            var expRes = new IntervalMatrix(new[,] {
           {
                Interval.FromDoublePrecise(30),Interval.FromDoublePrecise(36),Interval.FromDoublePrecise(42)
            },
            {
                Interval.FromDoublePrecise(66),Interval.FromDoublePrecise(81),Interval.FromDoublePrecise(96)
            },
            {
                Interval.FromDoublePrecise(102),Interval.FromDoublePrecise(126),Interval.FromDoublePrecise(150)
            }
            });

            var res = lhs * rhs;

            for (var row = 0; row < expRes.RowCount; row++)
            {
                for (var col = 0; col < expRes.RowCount; col++)
                {
                    Assert.AreEqual(expRes.Items[row, col].Infimum, res.Items[row, col].Infimum, 0.0001);
                    Assert.AreEqual(expRes.Items[row, col].Supremum, res.Items[row, col].Supremum, 0.0001);
                }
            }

            res = rhs * lhs;

            for (var row = 0; row < expRes.RowCount; row++)
            {
                for (var col = 0; col < expRes.RowCount; col++)
                {
                    Assert.AreEqual(expRes.Items[row, col].Infimum, res.Items[row, col].Infimum, 0.0001);
                    Assert.AreEqual(expRes.Items[row, col].Supremum, res.Items[row, col].Supremum, 0.0001);
                }
            }
        }

        [Test]
        public void TestIntervalMatrixIntervalMultiplication()
        {
            var lhs = new IntervalMatrix(new[,] {
            {
                Interval.FromDoublePrecise(1),Interval.FromDoublePrecise(2),Interval.FromDoublePrecise(3)
            },
            {
                Interval.FromDoublePrecise(4),Interval.FromDoublePrecise(5),Interval.FromDoublePrecise(6)
            },
            {
                Interval.FromDoublePrecise(7),Interval.FromDoublePrecise(8),Interval.FromDoublePrecise(9)
            }
            });

            var rhs = Interval.FromDoublePrecise(2);
            var res = lhs * rhs;

            var expRes = new IntervalMatrix(new[,] {
           {
                Interval.FromDoublePrecise(2),Interval.FromDoublePrecise(4),Interval.FromDoublePrecise(6)
            },
            {
                Interval.FromDoublePrecise(8),Interval.FromDoublePrecise(10),Interval.FromDoublePrecise(12)
            },
            {
                Interval.FromDoublePrecise(14),Interval.FromDoublePrecise(16),Interval.FromDoublePrecise(18)
            }
            });

            for (var row = 0; row < expRes.RowCount; row++)
            {
                for (var col = 0; col < expRes.RowCount; col++)
                {
                    Assert.AreEqual(expRes.Items[row, col].Infimum, res.Items[row, col].Infimum, 0.0001);
                    Assert.AreEqual(expRes.Items[row, col].Supremum, res.Items[row, col].Supremum, 0.0001);
                }
            }
        }

        [Test]
        public void TestIntervalMatrixDoubleVectorMutliplication()
        {
            var items = new[] { 1.0, 2, 3 };
            var rhs = Vector<double>.Build.DenseOfArray(items);

            var lhs = new IntervalMatrix(new[,] {
            {
                Interval.FromDoublePrecise(1),Interval.FromDoublePrecise(2),Interval.FromDoublePrecise(3)
            },
            {
                Interval.FromDoublePrecise(4),Interval.FromDoublePrecise(5),Interval.FromDoublePrecise(6)
            },
            {
                Interval.FromDoublePrecise(7),Interval.FromDoublePrecise(8),Interval.FromDoublePrecise(9)
            }
            });

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
        public void TestIntervalMatrixVectorMutliplication()
        {
            var items = new[] { Interval.FromDoublePrecise(1), Interval.FromDoublePrecise(2), Interval.FromDoublePrecise(3) };
            var rhs = new IntervalVector(items);

            var lhs = new IntervalMatrix(new[,] {
            {
                Interval.FromDoublePrecise(1),Interval.FromDoublePrecise(2),Interval.FromDoublePrecise(3)
            },
            {
                Interval.FromDoublePrecise(4),Interval.FromDoublePrecise(5),Interval.FromDoublePrecise(6)
            },
            {
                Interval.FromDoublePrecise(7),Interval.FromDoublePrecise(8),Interval.FromDoublePrecise(9)
            }
            });

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
        public void TestIsConcave()
        {
            var m = new IntervalMatrix(new[,] {
            {
                Interval.FromDoublePrecise(1),Interval.FromDoublePrecise(2),Interval.FromDoublePrecise(3)
            },
            {
                Interval.FromDoublePrecise(4),Interval.FromDoublePrecise(5),Interval.FromDoublePrecise(6)
            },
            {
                Interval.FromDoublePrecise(7),Interval.FromDoublePrecise(8),Interval.FromDoublePrecise(9)
            }
            });

            Assert.IsFalse(m.IsConcave());

            m = new IntervalMatrix(new[,] {
            {
                Interval.FromDoublePrecise(-1),Interval.FromDoublePrecise(-2),Interval.FromDoublePrecise(-3)
            },
            {
                Interval.FromDoublePrecise(-4),Interval.FromDoublePrecise(-5),Interval.FromDoublePrecise(-6)
            },
            {
                Interval.FromDoublePrecise(-7),Interval.FromDoublePrecise(-8),Interval.FromDoublePrecise(-9)
            }
            });
           
            Assert.IsTrue(m.IsConcave());

            m = new IntervalMatrix(new[,] {
            {
                Interval.FromDoublePrecise(-1),Interval.FromDoublePrecise(2),Interval.FromDoublePrecise(3)
            },
            {
                Interval.FromDoublePrecise(4),Interval.FromDoublePrecise(-5),Interval.FromDoublePrecise(6)
            },
            {
                Interval.FromDoublePrecise(7),Interval.FromDoublePrecise(8),Interval.FromDoublePrecise(-9)
            }
            });

            Assert.IsTrue(m.IsConcave());

        }

        [Test]
        public void TestInterior()
        {
            var i1 = Interval.FromInfSup(1, 2);
            var lhs = new IntervalMatrix(new [,]{{ i1, i1},{ i1 , i1}});

            var i2 = Interval.FromInfSup(1, 1.9);
            var rhs = new IntervalMatrix(new[,] { { i2, i2 }, { i2, i2 } });

            Assert.IsFalse(lhs.Interior(rhs));

            var i3 = Interval.FromInfSup(0.9, 2.1);
            rhs = new IntervalMatrix(new[,] { { i3, i3 }, { i3, i3 } });

            Assert.IsTrue(lhs.Interior(rhs));
        }
    }
}
