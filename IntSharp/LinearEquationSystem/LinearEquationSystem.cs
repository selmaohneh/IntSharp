using System;
using IntSharp.Types;

namespace IntSharp.LinearEquationSystem
{
    /// <summary>
    /// Class for solving a linear equation system of type A*x = b.
    /// </summary>
    public static class LinearEquationSystem
    {
        /// <summary>
        /// Starting point for solving any equation system. Checks the solvability and calls corresponding sub class.
        /// When using error weighting each line of the equation system gets scaled by a weight resulting from its accuracy.
        /// This requires two full equation system solvings. The first one to aquire the corresponding weights, the second one to calculate
        /// the results of the weighted system.
        /// The following paper demonstrates the importance of a weighted regression and makes clear, that it should be used
        /// as default:
        /// "Nachweis systematischer Fehler durch gewichtet Regression"
        /// Klaus Doerfell und Ralph Hebisch
        /// </summary>
        public static IntervalVector Solve(IntervalMatrix a, IntervalVector b, bool errorWeighting)
        {
            var solvability = CheckSolvability(a, b);

            if (solvability.Equals(Solvability.UnderDetermined))
                throw new Exception("Equation system could not be solved. The system matrix is underdetermined.");

            IntervalVector unweightedResult;
            Tuple<IntervalMatrix, IntervalVector> weightedSystem;
            
            if (solvability.Equals(Solvability.Determined))
            {
                if (!errorWeighting) return Determined.Solve(a, b);

                // First/unweighted solving.
                unweightedResult = Determined.Solve(a, b);

                // Second/weighted solving.
                weightedSystem = GetWeightedSystem(a, b, unweightedResult);
                a = weightedSystem.Item1;
                b = weightedSystem.Item2;
                return Determined.Solve(a, b);
            }

            // Overdetermined system. Least squares problem.
            if (!errorWeighting) return OverDetermined.Solve(a, b);

            // First/unweighted solving.
            unweightedResult = OverDetermined.Solve(a, b);

            // Second/weighted solving.
            weightedSystem = GetWeightedSystem(a, b, unweightedResult);
            a = weightedSystem.Item1;
            b = weightedSystem.Item2;
            return OverDetermined.Solve(a, b);
        }
        
        /// <summary>
        /// Scales the system matrix A and the right hand side b with weights according to their unweighted results.
        /// </summary>
        private static Tuple<IntervalMatrix, IntervalVector> GetWeightedSystem(
            IntervalMatrix a,
            IntervalVector b,
            IntervalVector unweightedResult)
        {
            var weightedAItems = new Interval[a.RowCount, a.ColumnCount];
            var weightedBItems = new Interval[b.RowCount];

            for (var row = 0; row < a.RowCount; row++)
            {
                // Weight = accuracy of calculated unweighted result.
                var weight = (a.GetRow(row) * unweightedResult).Rad();

                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (weight == 0) continue;

                // Weight the rhs.
                weightedBItems[row] = b.Items[row] / weight;

                // Weight the system matrix A.
                for (var col = 0; col < a.ColumnCount; col++)
                {
                    weightedAItems[row, col] = a.Items[row, col] / weight;
                }
            }

            return new Tuple<IntervalMatrix, IntervalVector>(
                new IntervalMatrix(weightedAItems),
                new IntervalVector(weightedBItems)
                );
        }

        /// <summary>
        /// Checks and returns the given equation system's solvability.
        /// </summary>
        private static Solvability CheckSolvability(IntervalMatrix a, IntervalVector b)
        {
            // Check if the system is undertermined, i.e. has no solution at all.
            var augmentedMatrix = a.Mid().Append(b.Mid().ToColumnMatrix());
            var augmentedRank = augmentedMatrix.Rank();
            if (augmentedRank < a.Mid().Rank() || augmentedRank < a.ColumnCount)
            {
                return Solvability.UnderDetermined;
            }

            // The system is determined, if the rank of the augmented matrix equals the number of unknowns.
            return augmentedRank == a.ColumnCount ? Solvability.Determined : Solvability.OverDetermined;
        }

        private enum Solvability
        {
            UnderDetermined,
            Determined,
            OverDetermined
        }
    }
}
