using IntSharp.Types;

namespace IntSharp.LinearEquationSystem
{
    /// <summary>
    /// Class for solving an overdetermined linear equation system of type A*x=b
    /// </summary>
    internal static class OverDetermined
    {
        /// <summary>
        /// Equation (4.2)
        /// "Verified bounds for Least Squares Problems and Underdetermined Linear Systems"
        /// Prof. Rump 
        /// http://www.ti3.tu-harburg.de/paper/rump/Ru11balt3.pdf
        /// </summary>
        public static IntervalVector Solve(IntervalMatrix a, IntervalVector b)
        {
            // Convert to determined system and solve.
            var leastSquaresResult = Determined.Solve(GetA(a), GetB(a, b));

            // Cut away the zero solution Aw = 0.
            var resultItems = new Interval[a.ColumnCount];
            for (var row = 0; row < a.ColumnCount; row++)
            {
                resultItems[row] = leastSquaresResult.Items[row];
            }

            return new IntervalVector(resultItems);
        }

        /// <summary>
        /// Converts the current matrix to a new matrix as shown in Equation (4.2).
        /// </summary>
        private static IntervalMatrix GetA(IntervalMatrix a)
        {
            var size = a.RowCount + a.ColumnCount;
            var items = new Interval[size, size];
            for (var row = 0; row < size; row++)
            {
                for (var col = 0; col < size; col++)
                {
                    // Include A.
                    if (row < a.RowCount && col < a.ColumnCount) items[row, col] = a.Items[row, col];

                    // Include A^T.
                    else if (row >= a.RowCount && col >= a.ColumnCount) items[row, col] = a.Items[col - a.ColumnCount, row - a.RowCount];

                    // Include -I.
                    else if (row < a.RowCount && col >= a.ColumnCount && row == col - a.ColumnCount) items[row, col] = Interval.FromDoublePrecise(-1);

                    // Fill other cells with zero.
                    else items[row, col] = Interval.Zero;
                }
            }

            return new IntervalMatrix(items);
        }

        /// <summary>
        /// Converts the current right hand side to a new right hand side as shown in Equation (4.2).
        /// </summary>
        private static IntervalVector GetB(IntervalMatrix a, IntervalVector b)
        {
            var size = a.RowCount + a.ColumnCount;
            var items = new Interval[size];
            for (var row = 0; row < size; row++)
            {
                // Include b.
                if (row < a.RowCount) items[row] = b.Items[row];

                // Fill other cells with zero.
                else items[row] = Interval.Zero;
            }

            return new IntervalVector(items);
        }    
    }
}
