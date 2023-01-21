/**
 * @file AdvancedFunctions.cs
 *
 * This source file contains tests for mathematical library 'CustomMath'
 * Copyright (C) 2019 BigyTeam
 * Permission is granted to copy, distribute and/or modify this document under the
 * terms of the GNU Free Documentation License as published by the Free Software Foundation,
 * either version 3 of the License, or (at your option) any later version.
 */

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CustomMath;

/// <summary>
/// Tests for custom math library
/// </summary>

namespace MathLibraryTests
{
    /**
     * @brief AdvancedFunctions class contains tests for basic mathematical
     * operations from CustomMath library
     */
    [TestClass]
    public class AdvancedFunctions
    {
        /**
         * @brief This test controls function 'Factorial' from CustomMath 
         */
        [TestMethod]
        public void Factorial()
        {
            //function should throw exception in following cases
            //though negative factorials exist (gamma function), function
            //calculates only standard defined factorial which cannot be negative
            Assert.ThrowsException<Exception>(() => (MathFunctions.Factorial(-5)));
            //factorial greater than 170 cannot be represented in C#
            Assert.ThrowsException<Exception>(() => (MathFunctions.Factorial(171)));

            //factorial of 0 is 1
            Assert.AreEqual<double>(MathFunctions.Factorial(0), 1);

            //random values
            Assert.AreEqual<double>(MathFunctions.Factorial(5), 120);
            Assert.AreEqual<double>(MathFunctions.Factorial(10), 3628800);
            Assert.AreEqual<double>(MathFunctions.Factorial(14), 8.71782912e10);
        }

        /**
         * @brief This test controls function 'Power' from CustomMath 
         */
        [TestMethod]
        public void Power()
        {
            //special values
            Assert.AreEqual<double>(MathFunctions.Power(0, 5), 0);
            Assert.AreEqual<double>(MathFunctions.Power(5, 0), 1);

            //random values
            Assert.AreEqual<double>(MathFunctions.Power(5, 2), 25);
            Assert.AreEqual<double>(MathFunctions.Power(14, 7), Math.Pow(14, 7));
            Assert.AreEqual<double>(MathFunctions.Power(8.7, 9), Math.Round(Math.Pow(8.7, 9) * 1000000) / 1000000);
            Assert.AreEqual<double>(MathFunctions.Power(-4.14, 5), Math.Round(Math.Pow(-4.14, 5) * 1000000) / 1000000);
        }

        /**
         * @brief This test controls function 'Ln' from CustomMath 
         */
        [TestMethod]
        public void Ln()
        {
            //controls exception when nonpositive logarithm
            Assert.ThrowsException<Exception>(() => (MathFunctions.Ln(0)));
            Assert.ThrowsException<Exception>(() => (MathFunctions.Ln(-5)));

            //**random calculations
            //three decimal numbers match is sufficient -> we cut
            double first = MathFunctions.Ln(14.5786);
            double second = Math.Log(14.5786);
            first = Math.Truncate(first * 1000) / 1000;
            second = Math.Truncate(second * 1000) / 1000;
            Assert.AreEqual<double>(first, second);

            first = MathFunctions.Ln(274.364);
            second = Math.Log(274.364);
            first = Math.Truncate(first * 1000) / 1000;
            second = Math.Truncate(second * 1000) / 1000;
            Assert.AreEqual<double>(first, second);

            first = MathFunctions.Ln(7.946125);
            second = Math.Log(7.946125);
            first = Math.Truncate(first * 1000) / 1000;
            second = Math.Truncate(second * 1000) / 1000;
            Assert.AreEqual<double>(first, second);

            first = MathFunctions.Ln(0.7654);
            second = Math.Log(0.7654);
            first = Math.Truncate(first * 1000) / 1000;
            second = Math.Truncate(second * 1000) / 1000;
            Assert.AreEqual<double>(first, second);
        }

        /**
         * @brief This test controls function 'Root' from CustomMath 
         */
        [TestMethod]
        public void Root()
        {
            //**exception checking
            //zero exponent
            Assert.ThrowsException<Exception>(() => (MathFunctions.Root(5, 0)));
            //negative root
            Assert.ThrowsException<Exception>(() => (MathFunctions.Root(5, -5)));
            //negative base when while even root
            Assert.ThrowsException<Exception>(() => (MathFunctions.Root(-4, 2)));

            //**random calculations
            //three decimal numbers match is sufficient -> we cut
            double first = MathFunctions.Root(2546.78, 2);
            double second = Math.Sqrt(2546.78);
            first = Math.Truncate(first * 1000) / 1000;
            second = Math.Truncate(second * 1000) / 1000;
            Assert.AreEqual<double>(first, second);

            first = MathFunctions.Root(1486, 7);
            first = Math.Truncate(first * 1000) / 1000;
            Assert.AreEqual<double>(first, 2.838);

            first = MathFunctions.Root(742.965, 4.876);
            first = Math.Truncate(first * 1000) / 1000;
            Assert.AreEqual<double>(first, 3.879);

            first = MathFunctions.Root(12.8, 9.6);
            first = Math.Truncate(first * 1000) / 1000;
            Assert.AreEqual<double>(first, 1.304);
        }
    }
}
