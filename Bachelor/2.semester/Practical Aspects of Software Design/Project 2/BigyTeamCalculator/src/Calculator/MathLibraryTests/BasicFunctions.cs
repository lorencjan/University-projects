/**
 * @file BasicFunctions.cs
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
     * @brief BasicFunctions class contains tests for basic mathematical
     * operations from CustomMath library
     */
    [TestClass]
    public class BasicFunctions
    {
        /**
         * @brief This test controls function 'Add' from CustomMath 
         */
        [TestMethod]
        public void Add()
        {
            Assert.AreEqual<double>(MathFunctions.Add(12.4, 14.5), 12.4 + 14.5);
            Assert.AreEqual<double>(MathFunctions.Add(34575, -84751), 34575 - 84751);
            Assert.AreEqual<double>(MathFunctions.Add(-456.4475, -174.556), -456.4475 + (-174.556));
        }

        /**
         * @brief This test controls function 'Subtract' from CustomMath 
         */
        [TestMethod]
        public void Subtract()
        {
            Assert.AreEqual<double>(MathFunctions.Subtract(152.3, 141.75), 152.3 - 141.75);
            Assert.AreEqual<double>(MathFunctions.Subtract(3.4575, -8.4751), 3.4575 + 8.4751);
            Assert.AreEqual<double>(MathFunctions.Subtract(-456.4475, -174.556), -456.4475 + 174.556);
        }

        /**
         * @brief This test controls function 'Multiply' from CustomMath 
         */
        [TestMethod]
        public void Multiply()
        {
            Assert.AreEqual<double>(MathFunctions.Multiply(15.3, 11.75), 15.3 * 11.75);
            Assert.AreEqual<double>(MathFunctions.Multiply(13.45, -8.4751), 13.45 * (-8.4751));
            Assert.AreEqual<double>(MathFunctions.Multiply(-6.4, -124.6), -6.4 * (-124.6));
        }

        /**
         * @brief This test controls function 'Divide' from CustomMath 
         */
        [TestMethod]
        public void Divide()
        {
            //expecting dividing by zero exception
            Assert.ThrowsException<DivideByZeroException>(() => (MathFunctions.Divide(14, 0)));
            //random values
            Assert.AreEqual<double>(MathFunctions.Divide(17.3, 11.5), 17.3 / 11.5);
            Assert.AreEqual<double>(MathFunctions.Divide(113.5, -8.4751), 113.5 / (-8.4751));
            Assert.AreEqual<double>(MathFunctions.Divide(-25648.4, -14.6), -25648.4 / (-14.6));
        }

        /**
         * @brief This test controls function 'Mod' from CustomMath 
         */
        [TestMethod]
        public void Mod()
        {
            //expecting dividing by zero exception
            Assert.ThrowsException<DivideByZeroException>(() => (MathFunctions.Divide(14, 0)));
            //random values
            Assert.AreEqual<double>(MathFunctions.Mod(16, 4), 0);
            Assert.AreEqual<double>(MathFunctions.Mod(17.7, 11.8), 17.7 % 11.8);
            Assert.AreEqual<double>(MathFunctions.Mod(11.6, -8.7), 11.6 % (-8.7));
            Assert.AreEqual<double>(MathFunctions.Mod(-5648.1, -74.47), -5648.1 % (-74.47));
        }
    }
}
