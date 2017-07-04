using IntSharp.Types;

namespace IntSharp
{
    /// <summary>
    /// Inflation is needed after every interval arithmetic operation. 
    /// If for example an operation's result would be 0.99999999999999999999 it will be rounded to 1 due to an double overflow. 
    /// Using the inflation algorithm below, the value gets enclosed rigorously! 
    /// The need of an inflation after each arithmetic operation is inevitable since C#/.NET has no option
    /// to switch the rounding mode dynamically (like C has). 
    /// </summary>
    public static class Inflation
    {
        /// <summary>
        /// Use the inflation algorithm to get the next greater representable double.
        /// </summary>
        public static double InflateUp(this double c)
        {
            return double.IsInfinity(c) ? c : GetInflatedValue(c, InflationDirection.Up);
        }

        /// <summary>
        /// Use the inflation algorithm to get the next smaller representable double.
        /// </summary>
        public static double InflateDown(this double c)
        {
            return double.IsInfinity(c) ? c : GetInflatedValue(c, InflationDirection.Down);
        }

        /// <summary>
        /// Performs an epsilon inflation. The intervals bounds get outward-bounded, meaning the new infimum will
        /// be a little smaller and the new supremum will be a little bigger.
        /// </summary>
        public static Interval EpsilonInflation(this Interval i)
        {
            var infimum = InflateDown(i.Infimum);
            var supremum = InflateUp(i.Supremum);

            return Interval.FromInfSup(infimum, supremum);
        }

        /// <summary>
        /// Performs an item-wise epsilon inflation.
        /// </summary>
        public static IntervalVector EpsilonInflation(this IntervalVector i)
        {
            var inflatedItems = new Interval[i.RowCount];
            for (var row = 0; row < i.RowCount; row++)
            {
                inflatedItems[row] = i.Items[row].EpsilonInflation();
            }

            return new IntervalVector(inflatedItems);
        }

        /// <summary>
        /// Algorithm 1
        /// "Interval operations in rounding to nearest"
        /// Prof. Rump
        /// http://www.ti3.tuhh.de/paper/rump/RuZiBoMe09.pdf
        /// </summary>
        private static double GetInflatedValue(double c, InflationDirection direction)
        {
            // The base.
            const int beta = 10;

            // 15 digits of precision.
            // Smaller values give poorer/less precise results.
            // Greater values are not possible due to .NETs Double precision.
            // This is why this value should not be changed.
            const int p = 15;
            
            var u = System.Math.Pow(beta, 1 - p) / 2;
            var phi = u * (1 + 4.0 / beta * u);
            const double eta = double.Epsilon;

            // The calculation of the paper is split in two parts whilst the first part
            // is temporary stored in a variable to force the compiler to round to nearest.
            // This is represented as the fl(.) operator in the paper.
            var tmp = phi * System.Math.Abs(c);
            var e = tmp + eta;

            if (direction.Equals(InflationDirection.Up)) return c + e;

            return c - e;
        }

        private enum InflationDirection
        {
            Up,
            Down
        }
    }
}
