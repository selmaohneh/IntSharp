using System;
using MathNet.Numerics.LinearAlgebra;

namespace IntSharp.Types
{
    /// <summary>
    /// Common matrix implementation for the interval type
    /// </summary>
    public struct IntervalMatrix
    {
        // Todo: add constants (zero, entire, ...).

        /// <summary>
        /// The matrix' contents.
        /// </summary>
        public Interval[,] Items { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="items"></param>
        public IntervalMatrix(Interval[,] items)
        {
            Items = items;
        }

        public int RowCount => Items.GetLength(0);
        public int ColumnCount => Items.GetLength(1);

        /// <summary>
        /// Checks if the matrix is concave.
        /// A matrix is concave if every item of the hessian's (2. derivative) main diagonal is smaller than zero.
        /// If any of those items is bigger or equal to 0, the matrix is not concave.
        /// </summary>
        public bool IsConcave()
        {
            for (var row = 0; row < RowCount; row++)
            {
                for (var col = 0; col < ColumnCount; col++)
                {
                    // Continue if not on main diagonal.
                    if(row != col) continue;
                    if (Items[row,col] >= 0) return false;
                }
            }

            return true;
        }

        public IntervalVector GetColumn(int columnIndex)
        {
            if (columnIndex > ColumnCount) throw new Exception($"Matrix has no column with index {columnIndex}.");

            var column = new Interval[RowCount];
            for (var row = 0; row < RowCount; row++)
            {
                column[row] = Items[row, columnIndex];
            }

            return new IntervalVector(column);
        }
        public IntervalVector GetRow(int rowIndex)
        {
            if (rowIndex > RowCount) throw new Exception($"Matrix has no row with index {rowIndex}.");

            var row = new Interval[ColumnCount];
            for (var col = 0; col < ColumnCount; col++)
            {
                row[col] = Items[rowIndex, col];
            }

            return new IntervalVector(row);
        }

        /// <summary>
        /// Flips the matrix' items over its diagonal. 
        /// </summary>
        public IntervalMatrix Transpose()
        {
            var items = new Interval[ColumnCount, RowCount];
            for (var row = 0; row < RowCount; row++)
            {
                for (var col = 0; col < ColumnCount; col++)
                {
                    items[col, row] = Items[row,col];
                }
            }

            return new IntervalMatrix(items);
        }

        // Todo: add missing operators.
        public static IntervalMatrix operator +(IntervalMatrix lhs, IntervalMatrix rhs)
        {
            if (lhs.RowCount != rhs.RowCount || lhs.ColumnCount != rhs.ColumnCount)
                throw new Exception("Matrix dimensions do not match.");

            var items = new Interval[lhs.RowCount, lhs.ColumnCount];
            for (var row = 0; row < lhs.RowCount; row++)
            {
                for (var col = 0; col < lhs.ColumnCount; col++)
                {
                    items[row, col] = lhs.Items[row,col] + rhs.Items[row,col];
                }
            }

            return new IntervalMatrix(items);
        }
        public static IntervalMatrix operator +(IntervalMatrix lhs, Interval rhs)
        {
            var items = new Interval[lhs.RowCount, lhs.ColumnCount];
            for (var row = 0; row < lhs.RowCount; row++)
            {
                for (var col = 0; col < lhs.ColumnCount; col++)
                {
                    items[row, col] = lhs.Items[row,col] + rhs;
                }
            }

            return new IntervalMatrix(items);
        }
        public static IntervalMatrix operator +(Matrix<double> lhs, IntervalMatrix rhs)
        {
            if (lhs.RowCount != rhs.RowCount || lhs.ColumnCount != rhs.ColumnCount)
                throw new Exception("Matrix dimensions do not match.");

            var items = new Interval[lhs.RowCount, lhs.ColumnCount];
            for (var row = 0; row < lhs.RowCount; row++)
            {
                for (var col = 0; col < lhs.ColumnCount; col++)
                {
                    items[row, col] = lhs.At(row, col) + rhs.Items[row,col];
                }
            }

            return new IntervalMatrix(items);
        }
        public static IntervalMatrix operator -(IntervalMatrix lhs, IntervalMatrix rhs)
        {
            if (lhs.RowCount != rhs.RowCount || lhs.ColumnCount != rhs.ColumnCount)
                throw new Exception("Matrix dimensions do not match.");

            var items = new Interval[lhs.RowCount, lhs.ColumnCount];
            for (var row = 0; row < lhs.RowCount; row++)
            {
                for (var col = 0; col < lhs.ColumnCount; col++)
                {
                    items[row, col] = lhs.Items[row,col] - rhs.Items[row,col];
                }
            }

            return new IntervalMatrix(items);
        }
        public static IntervalMatrix operator -(Matrix<double> lhs, IntervalMatrix rhs)
        {
            if (lhs.RowCount != rhs.RowCount || lhs.ColumnCount != rhs.ColumnCount)
                throw new Exception("Matrix dimensions do not match.");

            var items = new Interval[lhs.RowCount, lhs.ColumnCount];
            for (var row = 0; row < lhs.RowCount; row++)
            {
                for (var col = 0; col < lhs.ColumnCount; col++)
                {
                    items[row, col] = lhs.At(row, col) - rhs.Items[row,col];
                }
            }

            return new IntervalMatrix(items);
        }
        public static IntervalMatrix operator *(IntervalMatrix lhs, IntervalMatrix rhs)
        {
            if (lhs.ColumnCount != rhs.RowCount) throw new Exception("Matrix dimensions do not match.");

            var items = new Interval[lhs.RowCount, rhs.ColumnCount];
            for (var row = 0; row < items.GetLength(0); row++)
            {
                for (var col = 0; col < items.GetLength(1); col++)
                {
                    items[row, col] = Interval.FromDoublePrecise(0);
                    for (var i = 0; i < lhs.ColumnCount; i++)
                    {
                        items[row, col] += lhs.Items[row, i] * rhs.Items[i,col];
                    }
                }
            }

            return new IntervalMatrix(items);
        }
        public static IntervalMatrix operator *(Matrix<double> lhs, IntervalMatrix rhs)
        {
            if (lhs.ColumnCount != rhs.RowCount) throw new Exception("Matrix dimensions do not match.");

            var items = new Interval[lhs.RowCount, rhs.ColumnCount];
            for (var row = 0; row < items.GetLength(0); row++)
            {
                for (var col = 0; col < items.GetLength(1); col++)
                {
                    items[row, col] = Interval.FromDoublePrecise(0);
                    for (var i = 0; i < lhs.ColumnCount; i++)
                    {
                        items[row, col] += lhs.At(row, i) * rhs.Items[i,col];
                    }
                }
            }

            return new IntervalMatrix(items);
        }
        public static IntervalMatrix operator *(IntervalMatrix lhs, Matrix<double> rhs)
        {
            if (lhs.ColumnCount != rhs.RowCount) throw new Exception("Matrix dimensions do not match.");

            var items = new Interval[lhs.RowCount, rhs.ColumnCount];
            for (var row = 0; row < items.GetLength(0); row++)
            {
                for (var col = 0; col < items.GetLength(1); col++)
                {
                    items[row, col] = Interval.FromDoublePrecise(0);
                    for (var i = 0; i < lhs.ColumnCount; i++)
                    {
                        items[row, col] += lhs.Items[row,i] * rhs.At(i, col);
                    }
                }
            }

            return new IntervalMatrix(items);
        }
        public static IntervalVector operator *(IntervalMatrix lhs, Vector<double> rhs)
        {
            if (lhs.ColumnCount != rhs.Count) throw new Exception("Matrix and vector dimension do not match.");

            var items = new Interval[lhs.RowCount];
            for (var row = 0; row < items.GetLength(0); row++)
            {
                items[row] = Interval.FromDoublePrecise(0);
                for (var i = 0; i < lhs.ColumnCount; i++)
                {
                    items[row] += lhs.Items[row,i] * rhs.At(i);
                }
            }

            return new IntervalVector(items);
        }
        public static IntervalMatrix operator *(IntervalMatrix lhs, Interval rhs)
        {
            var items = new Interval[lhs.RowCount, lhs.ColumnCount];
            for (var row = 0; row < items.GetLength(0); row++)
            {
                for (var col = 0; col < items.GetLength(1); col++)
                {
                    items[row, col] = lhs.Items[row,col] * rhs;
                }
            }

            return new IntervalMatrix(items);
        }
        public static IntervalVector operator *(IntervalMatrix lhs, IntervalVector rhs)
        {
            if (lhs.ColumnCount != rhs.RowCount) throw new Exception("Matrix and vector dimension do not match.");

            var items = new Interval[lhs.RowCount];
            for (var row = 0; row < items.GetLength(0); row++)
            {
                items[row] = Interval.FromDoublePrecise(0);
                for (var i = 0; i < lhs.ColumnCount; i++)
                {
                    items[row] += lhs.Items[row,i] * rhs.Items[i];
                }
            }

            return new IntervalVector(items);
        }
    }
}
