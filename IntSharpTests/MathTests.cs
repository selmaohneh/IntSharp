using System;
using IntSharp.Types;
using NUnit.Framework;
using Math = System.Math;

namespace IntSharpTests
{
    [TestFixture]
    public class MathTests
    {
        [Test]
        public void TestAbs()
        {
            var i = Interval.FromInfSup(-7, 3);
            var res = IntSharp.Math.Abs(i);
            Assert.AreEqual(res.Infimum, 0);
            Assert.AreEqual(res.Supremum, 7);

            i = Interval.FromInfSup(-7, -3);
            res = IntSharp.Math.Abs(i);
            Assert.AreEqual(res.Infimum, 3);
            Assert.AreEqual(res.Supremum, 7);

            i = Interval.FromInfSup(3, 7);
            res = IntSharp.Math.Abs(i);
            Assert.AreEqual(res.Infimum, 3);
            Assert.AreEqual(res.Supremum, 7);

            i = Interval.Entire;
            res = IntSharp.Math.Abs(i);
            Assert.AreEqual(res.Infimum, 0);
            Assert.AreEqual(res.Supremum, double.PositiveInfinity);
        }
        
        [Test]
        public void TestCos()
        {
            var i = Interval.FromInfSup(0, 1);
            var res = IntSharp.Math.Cos(i);
            Assert.AreEqual(res.Infimum, 0.5403, 0.0001);
            Assert.AreEqual(res.Supremum, 1, 0.0001);

            i = Interval.FromInfSup(0, 1.8);
            res = IntSharp.Math.Cos(i);
            Assert.AreEqual(res.Infimum, -0.2272, 0.0001);
            Assert.AreEqual(res.Supremum, 1, 0.0001);

            i = Interval.FromInfSup(1.8, 5);
            res = IntSharp.Math.Cos(i);
            Assert.AreEqual(res.Infimum, -1, 0.0001);
            Assert.AreEqual(res.Supremum, 0.2836, 0.0001);

            i = Interval.FromInfSup(5*Math.PI, 10*Math.PI);
            res = IntSharp.Math.Cos(i);
            Assert.AreEqual(res.Infimum, -1);
            Assert.AreEqual(res.Supremum, 1);

            i = Interval.FromInfSup(5*Math.PI, 6*Math.PI);
            Assert.Throws<Exception>(()=> IntSharp.Math.Cos(i));
        }

        [Test]
        public void TestExp()
        {
            var i = Interval.FromInfSup(-3, 7);
            var res = IntSharp.Math.Exp(i);
            Assert.AreEqual(0.0497, res.Infimum, 0.0001);
            Assert.AreEqual(1096.633,res.Supremum, 0.001);
        }

        [Test]
        public void TestLog()
        {
            var i = Interval.FromInfSup(3, 7);

            var res = IntSharp.Math.Log(i,2);
            Assert.AreEqual(1.584, res.Infimum, 0.001);
            Assert.AreEqual(2.807,res.Supremum, 0.001);

            res = IntSharp.Math.Log(i, 10);
            Assert.AreEqual(0.477,res.Infimum, 0.001);
            Assert.AreEqual(0.845,res.Supremum, 0.001);
        }
        
        [Test]
        public void TestPown()
        {
            var i = Interval.FromInfSup(-3, 7);

            var r = IntSharp.Math.Pown(i, 2);
            Assert.AreEqual(0, r.Infimum, 0.001);
            Assert.AreEqual(49, r.Supremum, 0.001);

            r = IntSharp.Math.Pown(i, 3);
            Assert.AreEqual(-27, r.Infimum, 0.001);
            Assert.AreEqual(343, r.Supremum, 0.001);
        }

        [Test]
        public void TestSin()
        {
            var i = Interval.FromInfSup(0, 1);
            var res = IntSharp.Math.Sin(i);
            Assert.AreEqual(res.Infimum, 0, 0.0001);
            Assert.AreEqual(res.Supremum, 0.8414, 0.0001);

            i = Interval.FromInfSup(0, 1.8);
            res = IntSharp.Math.Sin(i);
            Assert.AreEqual(res.Infimum, 0, 0.0001);
            Assert.AreEqual(res.Supremum, 1, 0.0001);

            i = Interval.FromInfSup(1.8, 5);
            res = IntSharp.Math.Sin(i);
            Assert.AreEqual(res.Infimum, -1, 0.0001);
            Assert.AreEqual(res.Supremum, 0.9738, 0.0001);

            i = Interval.FromInfSup(5 * Math.PI, 10 * Math.PI);
            res = IntSharp.Math.Sin(i);
            Assert.AreEqual(res.Infimum, -1);
            Assert.AreEqual(res.Supremum, 1);

            i = Interval.FromInfSup(5 * Math.PI, 6 * Math.PI);
            Assert.Throws<Exception>(() => IntSharp.Math.Sin(i));
        }

        [Test]
        public void SqrTest()
        {
            var i = Interval.FromInfSup(-3, 7);
            var res = IntSharp.Math.Sqr(i);
            Assert.AreEqual(0, res.Infimum, 0.001);
            Assert.AreEqual(49, res.Supremum, 0.001);

            i = Interval.FromInfSup(3, 7);
            res = IntSharp.Math.Sqr(i);
            Assert.AreEqual(9, res.Infimum, 0.001);
            Assert.AreEqual(49, res.Supremum, 0.001);
        }
        
        [Test]
        public void SqrtTest()
        {
            var i = Interval.FromInfSup(4, 9);
            var res = IntSharp.Math.Sqrt(i);
            Assert.AreEqual(2, res.Infimum, 0.0001);
            Assert.AreEqual(3, res.Supremum, 0.0001);

            i = Interval.FromInfSup(-2, 9);
            Assert.Throws<Exception>(() => IntSharp.Math.Sqrt(i));
        }
    }
}
