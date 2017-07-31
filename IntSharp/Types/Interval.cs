using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

// ReSharper disable CompareOfFloatsByEqualityOperator
// File-scope due to multiple operator overloadings

namespace IntSharp.Types
{
    /// <summary>
    /// Base type for the whole library
    /// </summary>
    public struct Interval : IEquatable<object>, IComparable<Interval>
    {
        /// <summary>
        /// Lower bound of the interval.
        /// </summary>
        public double Infimum { get; }

        /// <summary>
        /// Upper bound of the interval.
        /// </summary>
        public double Supremum { get; }

        /// <summary>
        /// DON'T CALL!
        /// Use factory methods instead.
        /// Private constructor since factory pattern is used.
        /// </summary>
        private Interval(double infimum, double supremum)
        {
            Infimum = infimum;
            Supremum = supremum;
        }

        // Todo: add inflation factories for FromInfSup/FromMidRad.
        /// <summary>
        /// Factory using lower and upper bound.
        /// </summary>
        public static Interval FromInfSup(double inf, double sup)
        {
            // Filter swapped inf/sup.
            if (inf > sup) throw new Exception("An infimum greater than the supremum is invalid.");

            // Filter [+inf,+inf] and [-inf,-inf].
            if (double.IsPositiveInfinity(inf) && double.IsPositiveInfinity(sup))
                throw new Exception("[ +inf , +inf ] is an invalid interval.");
            if (double.IsNegativeInfinity(inf) && double.IsNegativeInfinity(sup))
                throw new Exception("[ +inf , +inf ] is an invalid interval.");

            // Filter NaN.
            if (double.IsNaN(inf)) throw new Exception("double.NaN is an invalid infimum.");
            if (double.IsNaN(sup)) throw new Exception("double.NaN is an invalid supremum.");

            return new Interval(inf, sup);
        }

        /// <summary>
        /// Factory using mid and radius.
        /// </summary>
        public static Interval FromMidRad(double mid, double rad)
        {
            // Filter negativ radius.
            if (rad < 0) throw new Exception("A negative radius is invalid.");

            // Filter infinite mid.
            if (double.IsInfinity(mid)) throw new Exception("An infinite mid is invalid.");

            // Filter NaN.
            if (double.IsNaN(mid)) throw new Exception("double.NaN is an invalid mid.");
            if (double.IsNaN(rad)) throw new Exception("double.NaN is an invalid radius.");

            return new Interval(mid - rad, mid + rad);
        }

        /// <summary>
        /// Factory using a single double.
        /// If the desired number has more than 15 significant digits use "FromDoubleWithEpsilonInflation" instead to prevent
        /// a rounding error. This guarantees a rigorous enclosure.
        /// </summary>
        public static Interval FromDoublePrecise(double value)
        {
            // Filter infinity.
            if (double.IsPositiveInfinity(value)) throw new Exception("[ +inf , +inf ] is an invalid interval.");
            if (double.IsNegativeInfinity(value)) throw new Exception("[ -inf , -inf ] is an invalid interval.");

            // Filter NaN.
            if (double.IsNaN(value)) throw new Exception("[ double.NaN , double.NaN ] is an invalid interval.");

            return new Interval(value, value);
        }

        /// <summary>
        /// Factory using a single double.
        /// If the desired number has less than 15 significant digits use "FromDoublePrecise"
        /// instead to get a smaller enclosure. This method should only be used to prevent rounding errors due to 
        /// numbers containig more than 15 significant digits. 
        /// </summary>
        public static Interval FromDoubleWithEpsilonInflation(double value)
        {
            // Filter infinity.
            if (double.IsPositiveInfinity(value)) throw new Exception("[ +inf , +inf ] is an invalid interval.");
            if (double.IsNegativeInfinity(value)) throw new Exception("[ -inf , -inf ] is an invalid interval.");

            // Filter NaN.
            if (double.IsNaN(value)) throw new Exception("[ double.NaN , double.NaN ] is an invalid interval.");

            return FromDoublePrecise(value).EpsilonInflation();
        }

        /// <summary>
        /// Standard IComparable implementation.
        /// </summary>
        public int CompareTo(Interval other)
        {
            if (this < other) return -1;
            if (this > other) return 1;

            return 0;
        }

        public override string ToString()
        {
            return ToString(IntervalFormat.MidRad);
        }

        /// <summary>
        /// Plots the interval in the given format and the given number of decimal digits.
        /// Use decimal digits = null (default) if you do not want to cut any digit. This
        /// allows maximum precision.
        /// </summary>
        public string ToString(IntervalFormat format, int? decimalDigits = null)
        {
            // No inflation needed.
            if (decimalDigits == null)
            {
                switch (format)
                {
                    case IntervalFormat.InfSup:
                        return
                            $"[ {Infimum.ToString(CultureInfo.InvariantCulture)} , {Supremum.ToString(CultureInfo.InvariantCulture)} ]";

                    case IntervalFormat.MidRad:
                        return
                            $"< {this.Mid().ToString(CultureInfo.InvariantCulture)} , {this.Rad().ToString(CultureInfo.InvariantCulture)} >";

                    case IntervalFormat.ValDev:
                        return
                            $"{this.Mid().ToString(CultureInfo.InvariantCulture)} ± {this.Rad().ToString(CultureInfo.InvariantCulture)}";

                    default:
                        throw new Exception("Interval format does not exist.");
                }
            }

            var inflationValue = System.Math.Pow(10, -decimalDigits.Value);

            if (format == IntervalFormat.InfSup)
            {
                // Get the inflated values
                var inflatedInfimum = Infimum - inflationValue;
                var inflatedSupremum = Supremum + inflationValue;

                // Round to given decimal digits.
                var roundedInfimum = System.Math.Round(inflatedInfimum, decimalDigits.Value);
                var roundedSupremum = System.Math.Round(inflatedSupremum, decimalDigits.Value);

                return $"[ {roundedInfimum.ToString(CultureInfo.InvariantCulture)} , " +
                       $"{roundedSupremum.ToString(CultureInfo.InvariantCulture)} ]";
            }

            // Get the inflated values
            var inflatedRadius = this.Rad() + inflationValue;

            // Round to given decimal digits.
            var roundedRadius = System.Math.Round(inflatedRadius, decimalDigits.Value);
            var roundedMid = System.Math.Round(this.Mid(), decimalDigits.Value);

            if (format == IntervalFormat.MidRad)
            {
                return $"< {roundedMid.ToString(CultureInfo.InvariantCulture)} , " +
                       $"{roundedRadius.ToString(CultureInfo.InvariantCulture)} >";
            }

            return $"{roundedMid.ToString(CultureInfo.InvariantCulture)} ± " +
                          $"{roundedRadius.ToString(CultureInfo.InvariantCulture)}";
        }

        /// <summary>
        /// Two intervals are equal when they have the same infimum and supremum.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (!(obj is Interval)) return false;

            var i = (Interval)obj;

            return i.Infimum == Infimum && i.Supremum == Supremum;
        }

        /// <summary>
        /// The hash code is based on the interval's infimum and supremum.
        /// </summary>
        public override int GetHashCode()
        {
            return new { Inf = Infimum, Sup = Supremum }.GetHashCode();
        }


        // Todo: add missing operators.
        public static Interval operator +(Interval lhs, Interval rhs)
        {
            var inf = (lhs.Infimum + rhs.Infimum).InflateDown();
            var sup = (lhs.Supremum + rhs.Supremum).InflateUp();

            return FromInfSup(inf, sup);
        }
        public static Interval operator +(Interval lhs, double rhs)
        {
            var intervalRhs = FromDoublePrecise(rhs);

            return lhs + intervalRhs;
        }
        public static Interval operator +(double lhs, Interval rhs)
        {
            var intervalLhs = FromDoublePrecise(lhs);

            return intervalLhs + rhs;
        }
        public static Interval operator -(Interval lhs, Interval rhs)
        {
            var inf = (lhs.Infimum - rhs.Supremum).InflateDown();
            var sup = (lhs.Supremum - rhs.Infimum).InflateUp();

            return FromInfSup(inf, sup);
        }
        public static Interval operator -(Interval i)
        {
            return FromInfSup(-i.Supremum, -i.Infimum);
        }
        public static Interval operator -(Interval lhs, double rhs)
        {
            var rhsInterval = FromDoublePrecise(rhs);

            return lhs - rhsInterval;
        }
        public static Interval operator -(double lhs, Interval rhs)
        {
            var lhsInterval = FromDoublePrecise(lhs);

            return lhsInterval - rhs;
        }
        public static Interval operator *(Interval lhs, Interval rhs)
        {
            var products = new List<double>
            {
                (lhs.Infimum * rhs.Infimum).InflateDown(),
                (lhs.Infimum * rhs.Supremum).InflateDown(),
                (lhs.Supremum * rhs.Infimum).InflateDown(),
                (lhs.Supremum * rhs.Supremum).InflateDown(),

                (lhs.Infimum * rhs.Infimum).InflateUp(),
                (lhs.Infimum * rhs.Supremum).InflateUp(),
                (lhs.Supremum * rhs.Infimum).InflateUp(),
                (lhs.Supremum * rhs.Supremum).InflateUp()
            };

            return FromInfSup(products.Min(), products.Max());
        }
        public static IntervalVector operator *(Interval lhs, IntervalVector rhs)
        {
            var items = new Interval[rhs.RowCount];
            for (var row = 0; row < rhs.RowCount; row++)
            {
                items[row] = lhs * rhs.Items[row];
            }

            return new IntervalVector(items);
        }
        public static Interval operator *(Interval lhs, double rhs)
        {
            var rhsInterval = FromDoublePrecise(rhs);

            return lhs * rhsInterval;
        }
        public static Interval operator *(double lhs, Interval rhs)
        {
            var lhsInterval = FromDoublePrecise(lhs);

            return lhsInterval * rhs;
        }
        public static Interval operator /(Interval lhs, Interval rhs)
        {
            if (rhs.ContainsZero()) throw new DivideByZeroException();

            var infFactor = (1 / rhs.Supremum).InflateDown();
            var supFactor = (1 / rhs.Infimum).InflateUp();

            return lhs * FromInfSup(infFactor, supFactor);
        }
        public static Interval operator /(Interval lhs, double rhs)
        {
            var rhsInterval = FromDoublePrecise(rhs);

            return lhs / rhsInterval;
        }
        public static Interval operator /(double lhs, Interval rhs)
        {
            var lhsInterval = FromDoublePrecise(lhs);

            return lhsInterval / rhs;
        }
        public static bool operator <(Interval lhs, Interval rhs)
        {
            return lhs.Supremum < rhs.Infimum;
        }
        public static bool operator >(Interval lhs, Interval rhs)
        {
            return lhs.Infimum > rhs.Supremum;
        }
        public static bool operator <=(Interval lhs, Interval rhs)
        {
            return lhs.Supremum <= rhs.Infimum;
        }
        public static bool operator >=(Interval lhs, Interval rhs)
        {
            return lhs.Infimum >= rhs.Supremum;
        }
        public static bool operator <(Interval lhs, double rhs)
        {
            return lhs.Supremum < rhs;
        }
        public static bool operator >(Interval lhs, double rhs)
        {
            return lhs.Infimum > rhs;
        }
        public static bool operator <=(Interval lhs, double rhs)
        {
            return lhs.Supremum <= rhs;
        }
        public static bool operator >=(Interval lhs, double rhs)
        {
            return lhs.Infimum >= rhs;
        }
        public static bool operator <(double lhs, Interval rhs)
        {
            return lhs < rhs.Infimum;
        }
        public static bool operator >(double lhs, Interval rhs)
        {
            return lhs > rhs.Supremum;
        }
        public static bool operator <=(double lhs, Interval rhs)
        {
            return lhs <= rhs.Infimum;
        }
        public static bool operator >=(double lhs, Interval rhs)
        {
            return lhs >= rhs.Supremum;
        }
        public static bool operator ==(Interval lhs, Interval rhs)
        {
            return lhs.Equals(rhs);
        }
        public static bool operator !=(Interval lhs, Interval rhs)
        {
            return !lhs.Equals(rhs);
        }

        // Todo: add more constants.
        /// <summary>
        /// [ 0 , 0 ]
        /// </summary>
        public static Interval Zero => FromDoublePrecise(0);

        /// <summary>
        /// [ -infinity , +infinity ]
        /// </summary>
        public static Interval Entire => FromInfSup(double.NegativeInfinity, double.PositiveInfinity);

        /// <summary>
        /// Rigorous enclosure of PI.
        /// </summary>
        public static Interval Pi => FromInfSup(3.14159265358979, 3.1415926535898);

        /// <summary>
        /// Rigorous enclosure of 2*PI.
        /// </summary>
        public static Interval TwoPi => FromInfSup(6.28318530717958, 6.28318530717959);

        /// <summary>
        /// Rigorous enclosure of PI/2.
        /// </summary>
        public static Interval HalfPi => FromInfSup(1.57079632679489, 1.5707963267949);

        /// <summary>
        /// Rigorous enclosure of 3*PI/2.
        /// </summary>
        public static Interval ThreeHalfPi => FromInfSup(4.71238898038468, 4.71238898038469);
    }

    public enum IntervalFormat
    {
        /// <summary>
        /// [lower, upper]
        /// </summary>
        InfSup,
        /// <summary>
        /// &lt; mid, radius >
        /// </summary>
        MidRad,
        /// <summary>
        /// mid ± radius
        /// </summary>
        ValDev
    }
}