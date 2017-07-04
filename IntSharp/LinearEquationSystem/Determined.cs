﻿using System;
using IntSharp.Types;
using MathNet.Numerics.LinearAlgebra;

namespace IntSharp.LinearEquationSystem
{
    internal static class Determined
    {
        /// <summary>
        /// Algorithm 10.7 
        /// "Verification methods: Rigorous results using floating-point arithmetic"
        /// Prof. Rump 
        /// http://www.ti3.tuhh.de/paper/rump/Ru10.pdf
        /// </summary>
        public static IntervalVector Solve(IntervalMatrix a, IntervalVector b)
        {
            // Approximate inverse.
            var r = a.Mid().Inverse();

            // Approximate solution.
            var xs = r * b.Mid();

            if (xs.Exists(double.IsNaN))
                throw new Exception("Equation system could not be solved. Approximate solution contains NaN.");

            // Iteration matrix.
            var c = Matrix<double>.Build.DenseIdentity(a.RowCount) - r * a;

            var z = r * (b - a * xs);
            var x = z;
            
            var lastX = IntervalVector.Entire(1);

            // Todo: maybe add break criterium depending on number of iterations.
            while (true)
            {
                var y = x.EpsilonInflation();

                // Iteration does not converge.
                if (x.Equals(lastX))
                    throw new Exception("Equation system could not be solved. The interval iteration does not converge.");

                lastX = x;

                // Interval iteration.
                x = z + c * y;

                if (x.Interior(y)) return xs + x;
            }
        }
    }
}