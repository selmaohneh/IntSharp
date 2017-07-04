using IntSharp;
using IntSharp.Types;
using MathNet.Numerics;
using NUnit.Framework;

namespace IntSharpTests
{
    [TestFixture]
    public class RootFindingTests
    {
        private static Interval F(Interval x)
        {
            return 4 * x * Math.Exp(-2 * 0.0001247 * Math.Sqr(72.8 - x)) 
                - 72.8 * Math.Sqr(1 + Math.Cos(Interval.FromMidRad(Trig.DegreeToRadian(110), Trig.DegreeToRadian(2))));
        }

        private static Interval Df(Interval x)
        {
            return -4 * (4 * 0.0001247 * x * (x - 72.8) - 1) * Math.Exp(-2 * 0.0001247 * Math.Sqr(x - 72.8));
        }
        
        [Test]
        public void FindRootTest()
        {
            var root = RootFinding.FindRoot(F, Df, Interval.FromInfSup(0,100));
            Assert.IsTrue(root.In(17.1));
        }
    }
}
