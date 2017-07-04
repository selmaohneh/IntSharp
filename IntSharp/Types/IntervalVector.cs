using System;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;

namespace IntSharp.Types
{
    /// <summary>
    /// Common vector implementation for the interval type
    /// </summary>
    public struct IntervalVector
    {
        /// <summary>
        /// The vector's contents.
        /// </summary>
        public Interval[] Items { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="items"></param>
        public IntervalVector(Interval[] items)
        {
            Items = items;
        }

        public int RowCount => Items.Length;

        /// <summary>
        /// Rigorously sums all intervals the vector contains.
        /// </summary>
        public Interval Sum
        {
            get
            {
                // No loop if only a single item. ReSharper disabled due to readability.
                // ReSharper disable once ConvertIfStatementToReturnStatement
                if (RowCount == 1) return Items.First();

                return Items.Aggregate(Interval.Zero, (current, item) => current + item);
            }
        }

        /// <summary>
        /// Rigorously calculates the mean of all intervals the vector contains.
        /// </summary>
        public Interval Mean
        {
            get
            {
                // No division neccessary if only a single item.
                if (RowCount == 1) return Items.First();

                return Sum / RowCount;
            }
        }

        // Todo: add missing operators.
        public static IntervalVector operator -(IntervalVector lhs, IntervalVector rhs)
        {
            if (lhs.RowCount != rhs.RowCount) throw new Exception("Vector dimensions do not match.");

            var items = new Interval[lhs.RowCount];
            for (var row = 0; row < lhs.RowCount; row++)
            {
                items[row] = lhs.Items[row] - rhs.Items[row];
            }

            return new IntervalVector(items);
        }
        public static IntervalVector operator +(IntervalVector lhs, Interval rhs)
        {
            var items = new Interval[lhs.RowCount];
            for (var row = 0; row < lhs.RowCount; row++)
            {
                items[row] = lhs.Items[row] + rhs;
            }

            return new IntervalVector(items);
        }
        public static IntervalVector operator *(Matrix<double> lhs, IntervalVector rhs)
        {
            if (lhs.ColumnCount != rhs.RowCount) throw new Exception("Matrix and vector dimension do not match.");

            var items = new Interval[lhs.RowCount];
            for (var row = 0; row < items.GetLength(0); row++)
            {
                items[row] = Interval.FromDoublePrecise(0);
                for (var i = 0; i < lhs.ColumnCount; i++)
                {
                    items[row] += lhs.At(row, i) * rhs.Items[i];
                }
            }

            return new IntervalVector(items);
        }
        public static IntervalVector operator *(IntervalVector lhs, Interval rhs)
        {
            var items = new Interval[lhs.RowCount];
            for (var row = 0; row < items.GetLength(0); row++)
            {
                items[row] = lhs.Items[row] * rhs;
            }

            return new IntervalVector(items);
        }
        public static Interval operator *(IntervalVector lhs, IntervalVector rhs)
        {
            if (lhs.RowCount != rhs.RowCount) throw new Exception("Vector dimensions do not match.");

            var sum = Interval.Zero;
            for (var row = 0; row < lhs.RowCount; row++)
            {
                sum += lhs.Items[row] * rhs.Items[row];
            }

            return sum;
        }   
        public static IntervalVector operator +(IntervalVector lhs, IntervalVector rhs)
        {
            if (lhs.RowCount != rhs.RowCount) throw new Exception("Vector dimensions do not match.");

            var items = new Interval[lhs.RowCount];
            for (var row = 0; row < lhs.RowCount; row++)
            {
                items[row] = lhs.Items[row] + rhs.Items[row];
            }

            return new IntervalVector(items);
        }
        public static IntervalVector operator +(Vector<double> lhs, IntervalVector rhs)
        {
            if (lhs.Count != rhs.RowCount) throw new Exception("Vector dimensions do not match.");

            var items = new Interval[rhs.RowCount];
            for (var row = 0; row < rhs.RowCount; row++)
            {
                items[row] = lhs.At(row) + rhs.Items[row];
            }

            return new IntervalVector(items);
        }

        // Todo: add more constants.

        /// <summary>
        /// Returns a vector with the given number of Interval.Entire items.
        /// </summary>
        public static IntervalVector Entire(int rows)
        {
            var items = new Interval[rows];
            for (var row = 0; row < rows; row++)
            {
                items[row] = Interval.Entire;
            }

            return new IntervalVector(items);
        }

        /// <summary>
        /// Returns a vector with the given number of Interval.Zero items.
        /// </summary>
        public static IntervalVector Zero(int rows)
        {
            var items = new Interval[rows];
            for (var row = 0; row < rows; row++)
            {
                items[row] = Interval.Zero;
            }

            return new IntervalVector(items);
        }
    }
}
