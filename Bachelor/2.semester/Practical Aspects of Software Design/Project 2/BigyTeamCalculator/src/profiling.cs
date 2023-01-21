using CustomMath;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Profiling
{
    class Profiling
    {
        static void Main(string[] args)
        {
            double sum;
            List<double> numbers = new List<double>();

            string[] content = File.ReadAllLines("../../data.txt");
            foreach (string line in content)
            {
                double n = double.Parse(line);
                numbers.Add(n);
            }

            double xsum = 0;
            foreach (double x in numbers)
            {
                xsum = MathFunctions.Add(x, xsum);
            }
            double xstriped = MathFunctions.Multiply(MathFunctions.Divide(1, numbers.Count()), xsum);

            xsum = 0;
            foreach (double x in numbers)
            {
                xsum = MathFunctions.Add(xsum, MathFunctions.Power(x, 2));
            }
            xsum = MathFunctions.Subtract(xsum, MathFunctions.Multiply(numbers.Count(), MathFunctions.Power(xstriped, 2)));

            sum = MathFunctions.Root(MathFunctions.Multiply(MathFunctions.Divide(1, MathFunctions.Subtract(numbers.Count(), 1)), xsum), 2);

            Console.WriteLine(sum);
        }
    }
}
