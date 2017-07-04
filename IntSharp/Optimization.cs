using System;
using System.Collections.Generic;
using System.Linq;
using IntSharp.Types;

namespace IntSharp
{
    /// <summary>
    /// Class for solving global optimization problems.
    /// </summary>
    public static class Optimization
    {
        // ToDo: Don't throw exception if search box causes DividedByZero, maybe just discard? Add test afterwards!

        /// <summary>
        /// Starts the global optimization algorithm.
        /// Very similar to "Algorithm 14.4" in 
        /// "C++ Toolbox for Verified Computing"
        /// by R. Hammer
        /// </summary>
        public static Tuple<List<Tuple<IntervalVector, double>>, Interval> Optimize(
            Func<IntervalVector, Interval> f,
            Func<IntervalVector, IntervalVector> gradientF, 
            Func<IntervalVector, IntervalMatrix> hessianF,  
            IntervalVector searchBox,
            double epsilon
            )
        {
            // Compute a first upper bound for the global minimum using the mid of the search box.
            var center = new IntervalVector(searchBox.Items.Select(item => Interval.FromDoublePrecise(item.Mid())).ToArray());
            var minimumSupremum = f(center).Supremum;

            // Initializations.
            var subBox = searchBox;
            var todoList = new List<Tuple<IntervalVector, double>>();
            var doneList = new List<Tuple<IntervalVector, double>>();
            bool bisectFurther;
            double subBoxInfimum;

            do
            {
                // Look for the direction with the biggest diameter.
                var k = FindIndexOfMaxDiam(subBox);

                // Bisect the box in that biggest direction.
                var subSubBoxes = Bisect(subBox, k);

                // Handle both bisected boxes.
                for (var i = 0; i < 2; i++)
                {
                    // Calculate the gradient.
                    var gradient = gradientF(subSubBoxes[i]);

                    // Monotonicity test.
                    // If the subSubBox is not interior of the whole searchBox it could contain the search box' bounds and
                    // so can't be discarded.
                    if (gradient.Items.Any(item => !item.ContainsZero()) && subSubBoxes[i].Interior(searchBox)) continue;

                    // Test with value of centered form.
                    var subSubBoxValue = (f(center) + gradient * (subSubBoxes[i] - center)).Intersection(f(subSubBoxes[i]));
                    if (minimumSupremum < subSubBoxValue.Infimum) continue;

                    // Calculate the hessian matrix.
                    var hessian = hessianF(subSubBoxes[i]);

                    // Concavity test.
                    // If the subSubBox is not interior of the whole searchBox it could contain the search box' bounds and
                    // so can't be discarded.
                    if(hessian.IsConcave() && subSubBoxes[i].Interior(searchBox)) continue;

                    // subSubBox is a candidate for a minimizer -> add to todoList
                    todoList.Add(new Tuple<IntervalVector, double>(subSubBoxes[i], subSubBoxValue.Infimum));
                    todoList = todoList.OrderBy(item => item.Item2).ToList();
                }

                bisectFurther = false;

                while (todoList.Any() && !bisectFurther)
                {
                    // Process (and remove) next sub box from the todoList.
                    // First element of the todoList is the one with smallest lower bound of the interval function evaluation.
                    subBox = todoList.First().Item1;
                    subBoxInfimum = todoList.First().Item2;
                    todoList = todoList.Skip(1).ToList();

                    // Midpoint test.
                    center = new IntervalVector(subBox.Items.Select(item => Interval.FromDoublePrecise(item.Mid())).ToArray());
                    if (f(center).Supremum < minimumSupremum) minimumSupremum = f(center).Supremum;
                    todoList = todoList.Where(item => item.Item2 <= minimumSupremum).ToList();

                    // Check tolerance criterion for current minimum.
                    var minimum = Interval.FromInfSup(subBoxInfimum, minimumSupremum);
                    if (minimum.Diam() <= epsilon || subBox.Items.All(item => item.Diam() <= epsilon) || todoList.Count == 0 && doneList.Count == 0)
                    {
                        doneList.Add(new Tuple<IntervalVector, double>(subBox, subBoxInfimum));
                        doneList = doneList.OrderBy(item => item.Item2).ToList();
                    }
                    else
                    {
                        bisectFurther = true;
                    }
                }
            } while (bisectFurther);
            
            subBoxInfimum = doneList.First().Item2;
            return new Tuple<List<Tuple<IntervalVector, double>>, Interval>(doneList, Interval.FromInfSup(subBoxInfimum, minimumSupremum));
        }

        /// <summary>
        /// Looks for the interval with the biggest diameter and returns the corresponding row index of
        /// the interval vector.
        /// </summary>
        private static int FindIndexOfMaxDiam(IntervalVector intervalVector)
        {
            var maxDiam = 0.0;
            var index = 0;
            for (var i = 0; i < intervalVector.RowCount; i++)
            {
                if (!(intervalVector.Items[i].Diam() > maxDiam)) continue;

                maxDiam = intervalVector.Items[i].Diam();
                index = i;
            }

            return index;
        }

        /// <summary>
        /// Bisects the box at the given index and returns both resulting interval vectors.
        /// </summary>
        private static IntervalVector[] Bisect(IntervalVector intervalVector, int bisectIndex)
        {
            var lItems = new Interval[intervalVector.RowCount];
            var rItems = new Interval[intervalVector.RowCount];

            for (var row = 0; row < intervalVector.RowCount; row++)
            {
                if (row == bisectIndex)
                {
                    var bisection = intervalVector.Items[row].Bisect();
                    lItems[row] = bisection.Item1;
                    rItems[row] = bisection.Item2;
                }
                else
                {
                    lItems[row] = intervalVector.Items[row];
                    rItems[row] = intervalVector.Items[row];
                }

            }

            return new[]
            {
                new IntervalVector(lItems),
                new IntervalVector(rItems)
            };
        }
    }
}
