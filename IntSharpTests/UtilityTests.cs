using IntSharp;
using IntSharp.Types;
using NUnit.Framework;

namespace IntSharpTests
{
    [TestFixture]
    public class UtilityTests
    {
        [Test]
        public void ContainsZeroTest()
        {
            var i = Interval.FromInfSup(-2, 2);
            Assert.IsTrue(i.ContainsZero());

            i = Interval.FromInfSup(0, 2);
            Assert.IsTrue(i.ContainsZero());

            i = Interval.Entire;
            Assert.IsTrue(i.ContainsZero());

            i = Interval.FromInfSup(1, 2);
            Assert.IsFalse(i.ContainsZero());
        }

        [Test]
        public void DisjointTest()
        {
            var lhs = Interval.FromInfSup(1, 2);

            var rhs = Interval.FromInfSup(2, 4);
            Assert.IsFalse(lhs.Disjoint(rhs));

            rhs = Interval.FromInfSup(-1, 2);
            Assert.IsFalse(lhs.Disjoint(rhs));

            rhs = Interval.FromInfSup(2.1, 4);
            Assert.IsTrue(lhs.Disjoint(rhs));

            rhs = Interval.FromInfSup(-1, 0.9);
            Assert.IsTrue(lhs.Disjoint(rhs));
        }

        [Test]
        public void InteriorTest()
        {
            var lhs = Interval.FromInfSup(1, 2);

            var rhs = Interval.FromInfSup(1, 2);
            Assert.IsFalse(lhs.Interior(rhs));

            rhs = Interval.FromInfSup(1, 1.9);
            Assert.IsFalse(lhs.Interior(rhs));

            rhs = Interval.FromInfSup(1, 2.1);
            Assert.IsFalse(lhs.Interior(rhs));

            rhs = Interval.FromInfSup(0.9, 2.1);
            Assert.IsTrue(lhs.Interior(rhs));
        }

        [Test]
        public void InTest()
        {
            var i = Interval.FromInfSup(-2, 2);
            Assert.IsTrue(i.In(0));
            Assert.IsTrue(i.In(-2));
            Assert.IsFalse(i.In(2.0001));

            i = Interval.Entire;
            Assert.IsTrue(i.In(0));
        }
        
        [Test]
        public void IsEntireTest()
        {
            var i = Interval.FromInfSup(double.NegativeInfinity, double.PositiveInfinity);
            Assert.IsTrue(i.IsEntire());

            i = Interval.FromInfSup(double.NegativeInfinity, double.MaxValue);
            Assert.IsFalse(i.IsEntire());

            i = Interval.FromInfSup(double.MinValue, double.PositiveInfinity);
            Assert.IsFalse(i.IsEntire());
        }

        [Test]
        public void IsNegativTest()
        {
            var i = Interval.FromInfSup(double.NegativeInfinity, 0);
            Assert.IsFalse(i.IsNegativ());

            i = Interval.FromInfSup(double.NegativeInfinity, -1);
            Assert.IsTrue(i.IsNegativ());

            i = Interval.FromInfSup(-2, -1);
            Assert.IsTrue(i.IsNegativ());

            i = Interval.FromInfSup(-2, 1);
            Assert.IsFalse(i.IsNegativ());

            i = Interval.FromInfSup(-0, -0);
            Assert.IsFalse(i.IsNegativ());
        }

        [Test]
        public void IsPositivTest()
        {
            var i = Interval.FromInfSup(1, double.PositiveInfinity);
            Assert.IsTrue(i.IsPositiv());

            i = Interval.FromInfSup(1, 2);
            Assert.IsTrue(i.IsPositiv());

            i = Interval.FromInfSup(0, double.PositiveInfinity);
            Assert.IsFalse(i.IsPositiv());

            i = Interval.FromInfSup(-1, 2);
            Assert.IsFalse(i.IsPositiv());

            i = Interval.FromInfSup(+0, +0);
            Assert.IsFalse(i.IsPositiv());
        }
        
        [Test]
        public void IsZeroTest()
        {
            var i = Interval.FromInfSup(0, 0);
            Assert.IsTrue(i.IsZero());

            i = Interval.FromInfSup(-0, -0);
            Assert.IsTrue(i.IsZero());

            i = Interval.FromInfSup(0,0.00000000001);
            Assert.IsFalse(i.IsZero());
        }
        
        [Test]
        public void MagTest()
        {
            var i = Interval.FromInfSup(-7, 5);
            Assert.AreEqual(7, i.Mag());

            i = Interval.FromInfSup(-3, 5);
            Assert.AreEqual(5, i.Mag());

            i = Interval.Entire;
            Assert.AreEqual(double.PositiveInfinity, i.Mag());
        }
        
        [Test]
        public void MidTest()
        {
            var i = Interval.Zero;
            Assert.AreEqual(0, i.Mid());

            i = Interval.FromInfSup(-1,1);
            Assert.AreEqual(0, i.Mid());

            i = Interval.FromInfSup(2, 4);
            Assert.AreEqual(3, i.Mid());
        }
        
        [Test]
        public void MigTest()
        {
            var i = Interval.FromInfSup(-7, 5);
            Assert.AreEqual(0, i.Mig());

            i = Interval.FromInfSup(3, 7);
            Assert.AreEqual(3, i.Mig());

            i = Interval.FromInfSup(-7, -3);
            Assert.AreEqual(3, i.Mig());

            i = Interval.Entire;
            Assert.AreEqual(0, i.Mig());
        }

        [Test]
        public void RadTest()
        {
            var i = Interval.Zero;
            Assert.AreEqual(0, i.Rad());

            i = Interval.FromInfSup(-1, 1);
            Assert.AreEqual(1, i.Rad());

            i = Interval.FromInfSup(2, 4);
            Assert.AreEqual(1, i.Rad());
        }

        [Test]
        public void SubsetTest()
        {
            var lhs = Interval.FromInfSup(1, 2);

            var rhs = Interval.FromInfSup(1, 2);
            Assert.IsTrue(lhs.Subset(rhs));

            rhs = Interval.FromInfSup(1, 2.1);
            Assert.IsTrue(lhs.Subset(rhs));

            rhs = Interval.FromInfSup(1, 1.9);
            Assert.IsFalse(lhs.Subset(rhs));

            lhs = Interval.FromInfSup(1, 2.1);
            rhs = Interval.FromInfSup(1, 2);
            Assert.IsFalse(lhs.Subset(rhs));
        }

        [Test]
        public void DiamTest()
        {
            var i = Interval.Zero;
            Assert.AreEqual(0, i.Diam());

            i = Interval.FromInfSup(-1, 1);
            Assert.AreEqual(2, i.Diam());

            i = Interval.FromInfSup(2, 4);
            Assert.AreEqual(2, i.Diam());

            i = Interval.Entire;
            Assert.AreEqual(double.PositiveInfinity, i.Diam());
        }

        [Test]
        public void TruncateTest()
        {
            var interval = Interval.FromInfSup(-5, 5);

            Assert.AreEqual(Interval.FromInfSup(-5, 2), interval.TruncateSup(2));
            Assert.AreEqual(Interval.FromInfSup(-5, -2), interval.TruncateSup(-2));

            Assert.AreEqual(Interval.FromInfSup(-2, 5), interval.TruncateInf(-2));
            Assert.AreEqual(Interval.FromInfSup(2, 5), interval.TruncateInf(2));
        }

        [Test]
        public void IntersectionTest()
        {
            var a = Interval.FromInfSup(10, 30);
            var b = Interval.FromInfSup(20, 40);

            var expectedIntersection = Interval.FromInfSup(20, 30);

            Assert.AreEqual(expectedIntersection,a.Intersection(b));
            Assert.AreEqual(expectedIntersection, b.Intersection(a));
        }
    }
}
