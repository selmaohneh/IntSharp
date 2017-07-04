using System;
using IntSharp.Types;
using MathNet.Numerics;

namespace IntSharp
{
    /// <summary>
    /// Class for finding roots of a function f.
    /// </summary>
    public static class RootFinding
    {
        /// <summary>
        /// Algorithm 2 
        /// "Solving Nonlinear Systems with Least Significant Bit Accuracy"
        /// Prof. Rump 
        /// http://www.ti3.tuhh.de/paper/rump/Ru82.pdf
        /// </summary>
        public static Interval FindRoot(Func<Interval,Interval> f, Func<Interval, Interval> df, Interval searchBounds)
        {
            // Approximate solution.
            // 15 due to .NET Double precision (should not be changed).
            var xs = FindRoots.OfFunction(x => 
                f(Interval.FromDoublePrecise(x)).Mid(), 
                searchBounds.Infimum, 
                searchBounds.Supremum, 
                1e-15);           

            // Inverse of the jacobian matrix.
            // Since the matrix has only a single item, the inverse is 1/item.
            var r = 1/df(Interval.FromDoublePrecise(xs)).Mid();

            var y = Interval.Zero;
            
            var lastY = Interval.Entire;

            var z = f(Interval.FromDoublePrecise(xs));
            z = -r*z;
            
            // Todo: maybe add stop criterium depening on number of iterations.
            while (true)
            {
                if (y.Equals(lastY)) throw new Exception("Root could not be found. The iteration does not converge.");
                lastY = y;

                y = y.EpsilonInflation();
                
                var x = y;

                // Interval iteration.
                var d = df(xs + x);
                y = z + (1 - r * d) * x;
               
                if (y.Interior(x)) return xs + y;
            }
        }
    }
}
