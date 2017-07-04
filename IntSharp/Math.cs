using System;
using System.Collections.Generic;
using System.Linq;
using IntSharp.Types;

namespace IntSharp
{
    /// <summary>
    /// Class that implements common mathematically functions
    /// </summary>
    public static class Math
    {
        // Todo: add argument reduction for sin/cos (binary calculations).
        // Todo: add more functions (tan, asin, ...)

        public static Interval Abs(Interval i)
        {
            var absInf = System.Math.Abs(i.Infimum);
            var absSup = System.Math.Abs(i.Supremum);

            return i.ContainsZero()
                ? Interval.FromInfSup(0, System.Math.Max(absInf, absSup))
                : Interval.FromInfSup(System.Math.Min(absInf, absSup), System.Math.Max(absInf, absSup));
        }

        /// <summary>
        /// Defined for -2Pi smaller/equal argument smaller/equal 2Pi.
        /// </summary>
        public static Interval Cos(Interval i)
        {
            if (i.Diam() > Interval.TwoPi) return Interval.FromInfSup(-1, 1);

            if (i > Interval.TwoPi || i.Infimum < -Interval.TwoPi)
                throw new Exception("Cosine only implemented for arguments between -2Pi and 2Pi.");

            var bounds = new List<double>
            {
                System.Math.Cos(i.Infimum).InflateDown(),
                System.Math.Cos(i.Supremum).InflateDown(),
                System.Math.Cos(i.Infimum).InflateUp(),
                System.Math.Cos(i.Supremum).InflateUp()
            };

            var infimum = bounds.Min();
            var supremum = bounds.Max();

            // Check critical points.
            if (i.ContainsZero() || Interval.TwoPi.Subset(i) || Interval.TwoPi.Subset(-i)) supremum = 1;
            if (Interval.Pi.Subset(i) || Interval.Pi.Subset(-i)) infimum = -1;

            return Interval.FromInfSup(infimum, supremum);
        }

        /// <summary>
        /// Defined for -2Pi smaller/equal argument smaller/equal 2Pi.
        /// </summary>
        public static IntervalVector Cos(IntervalVector i)
        {
            var results = new Interval[i.RowCount];
            for (var row = 0; row < i.RowCount; row++)
            {
                results[row] = Cos(i.Items[row]);
            }

            return new IntervalVector(results);
        }

        /// <summary>
        /// Defined for -2Pismaller/equal argument smaller/equal 2Pi.
        /// </summary>
        public static IntervalMatrix Cos(IntervalMatrix i)
        {
            var results = new Interval[i.RowCount,i.RowCount];
            for (var row = 0; row < i.RowCount; row++)
            {
                for (var col = 0; col < i.ColumnCount; col++)
                {
                    results[row,col] = Cos(i.Items[row,col]);
                }
                
            }

            return new IntervalMatrix(results);
        }
        
        public static Interval Exp(Interval i)
        {
            var inf = System.Math.Exp(i.Infimum).InflateDown();
            var sup = System.Math.Exp(i.Supremum).InflateUp();

            return Interval.FromInfSup(inf, sup);
        }

        /// <summary>
        /// Defined for positiv intervals and base >= 1.
        /// </summary>
        public static Interval Log(Interval i, double newBase)
        {
            // Filter negativ intervals.
            if (!i.IsPositiv()) throw new Exception("Negativ arguments are invalid.");

            // Filter too small base.
            if (newBase < 1) throw new Exception("Bases smaller than 1 are invalid.");

            var inf = System.Math.Log(i.Infimum, newBase).InflateDown();
            var sup = System.Math.Log(i.Supremum, newBase).InflateUp();
            return Interval.FromInfSup(inf, sup);
        }

        // Todo: improve pown to pow -> support any exponent, not just integer.
        public static Interval Pown(Interval i, int exponent)
        {
            double inf;
            double sup;

            // Odd exponent.
            if (exponent % 2 != 0)
            {
                inf = System.Math.Pow(i.Infimum, exponent).InflateDown();
                sup = System.Math.Pow(i.Supremum, exponent).InflateUp();
                return Interval.FromInfSup(inf, sup);
            }

            // Even exponent.
            if (i.Infimum >= 0)
            {
                inf = System.Math.Pow(i.Infimum, exponent).InflateDown();
                sup = System.Math.Pow(i.Supremum, exponent).InflateUp();
                return Interval.FromInfSup(inf, sup);
            }
            if (i.Supremum < 0)
            {
                inf = System.Math.Pow(i.Supremum, exponent).InflateDown();
                sup = System.Math.Pow(i.Infimum, exponent).InflateUp();
                return Interval.FromInfSup(inf, sup);
            }

            var powerInf = System.Math.Pow(i.Infimum, exponent).InflateUp();
            var powerSup = System.Math.Pow(i.Supremum, exponent).InflateUp();
            inf = 0;
            sup = System.Math.Max(powerInf, powerSup);
            return Interval.FromInfSup(inf, sup);
        }

        /// <summary>
        /// Defined for -2Pi smaller/equal argument smaller/equal 2Pi.
        /// </summary>
        public static Interval Sin(Interval i)
        {
            if (i.Diam() > Interval.TwoPi) return Interval.FromInfSup(-1, 1);

            if (i > Interval.TwoPi || i.Infimum < -Interval.TwoPi)
                throw new Exception("Sine only implemented for arguments between -2Pi and 2Pi.");

            var bounds = new List<double>
            {
                System.Math.Sin(i.Infimum).InflateDown(),
                System.Math.Sin(i.Supremum).InflateDown(),
                System.Math.Sin(i.Infimum).InflateUp(),
                System.Math.Sin(i.Supremum).InflateUp()
            };

            var infimum = bounds.Min();
            var supremum = bounds.Max();

            // Check critical points.
            if (Interval.HalfPi.Subset(i) || Interval.ThreeHalfPi.Subset(-i)) supremum = 1;
            if (Interval.ThreeHalfPi.Subset(i) || Interval.HalfPi.Subset(-i)) infimum = -1;

            return Interval.FromInfSup(infimum, supremum);
        }

        /// <summary>
        /// Note that interval*interval != interval².
        /// </summary>
        public static Interval Sqr(Interval i)
        {
            if (!i.ContainsZero()) return i * i;

            var absInf = System.Math.Abs(i.Infimum);
            var absSup = System.Math.Abs(i.Supremum);
            var absMax = System.Math.Max(absInf, absSup);

            var supremum = (absMax * absMax).InflateUp();
            return Interval.FromInfSup(0, supremum);
        }

        /// <summary>
        /// Defined for arguments >= 0.
        /// </summary>
        public static Interval Sqrt(Interval i)
        {
            if (i.Infimum < 0) throw new Exception("Negativ arguments are invalid.");

            var infimum = System.Math.Sqrt(i.Infimum).InflateDown();
            var supremum = System.Math.Sqrt(i.Supremum).InflateUp();

            return Interval.FromInfSup(infimum, supremum);
        }
    }
}
