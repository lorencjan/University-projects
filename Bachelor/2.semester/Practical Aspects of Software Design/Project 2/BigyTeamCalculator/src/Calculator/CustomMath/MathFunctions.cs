/**
 * @file MathFunctions.cs
 * @brief Custom math library
 *
 * This source file is a custom written mathematical library
 * Copyright (C) 2019 BigyTeam
 * Permission is granted to copy, distribute and/or modify this document under the
 * terms of the GNU Free Documentation License as published by the Free Software Foundation,
 * either version 3 of the License, or (at your option) any later version.
 **/

using System;

/// <summary>
/// Custom math library
/// </summary>

/**
 * @brief Library consists of one class containing several mathematical functions
 **/
namespace CustomMath
{
    /**
     * @brief MathFunctions contains both basic function +,-,*,/,mod
     * as well as general root, natural logarithm, power and factorial
     **/
    public static class MathFunctions
    {
        /**
         * @name Basic functions
         *
         * Functions of basic arithmetical operations
         *
         * @{
         **/

        /**
         * @brief Function takes two values, adds them and returns the result
         * @param a First number to be added
         * @param b Second number to be added
         * @return Returns result of addition of "a" and "b"
         **/
        public static double Add(double a, double b)
        {
            return a + b;
        }
        /**
         * @brief Function takes two values, subtract them and returns the result
         * @param a Number from which the second will be taken off
         * @param b Number the function subtract
         * @return Returns result of subtraction of "a" and "b"
         **/
        public static double Subtract(double a, double b)
        {
            return a - b;
        }

        /**
         * @brief Function takes two values, multiplies one by the other
         * @param a Number to be multiplied
         * @param b Number that multiplies
         * @return Returns "a" multiplied by "b"
         **/
        public static double Multiply(double a, double b)
        {
            return a * b;
        }

        /**
         * @brief Function takes two values, divides one by the other
         * @param a Number to be divided
         * @param b Number that divides
         * @return Returns "a" divided by "b"
         **/
        public static double Divide(double a, double b)
        {
            if (b == 0)
                throw new DivideByZeroException();
            return a / b;
        }

        /**
         * @brief Function takes two values, divides one by the other in order to get remainder
         * @param a Number to be divided
         * @param b Number that divides
         * @return Returns the remainder of "a" after being divided by "b"
         **/
        public static double Mod(double a, double b)
        {
            if (b == 0)
                throw new DivideByZeroException();
            return a % b;
        }

        /** @} */

        /**
         * @name Advanced functions
         *
         * More advanced functions
         *
         * @{
         **/

        /**
         * @brief Function takes a value and gets its factorial
         * @param a Nonnegative number
         * @return Returns the factorial of "a"
         **/
        public static double Factorial(int a)
        {
            //although negative factorial exists thanks to gamma functions, it is not standard
            if (a < 0)
                throw new Exception("Attempt to calculate negative factorial!");

            //too big a number to represent
            if (a > 170)
                throw new Exception("Too big a number to represent!");

            //factorial of 0 is 1
            if (a == 0)
                return 1;

            if (a ==1)
                return 1;
            //factorial algorithm
            double result = a;
            for (int i = a - 1; i != 1; i--)
                result *= i;

            return result;
        }

        /**
         * @brief Function powers a number by its exponent
         * @param a Base
         * @param x Exponent
         * @return Returns "a" powered by "x"
         **/
        public static double Power(double a, int x)
        {
            //any number power by 0 is 1
            if (x == 0)
                return 1;

            //powering a
            double result;
            for (result = 1; x > 0; x--)
                result *= a;

            //returns rounded to 6 decimals
            return Math.Round(result * 1000000) / 1000000;
        }

        /**
         * @brief Function calculates natural logarithm of the given number using Taylor algorithms
         * @param a Numerus of the logarithm
         * @return Returns natural logarithm of "a"
         **/
        public static double Ln(double a)
        {
            //controls, if the numerus of the logarithm is greater than 0
            if (a <= 0)
                throw new Exception("Numerus of a logarithm must be a positive number!");

            //setting accuracy to 0.000 000 1
            const double eps = 1e-7;
            //defining variables needed for the algorithms
            double poweredA = 1, result = 0, lastResult, difference = 1000;
            int counter = 1;
            //if numerus is greater than 1, use following algorithm
            if (a > 1)
            {
                while (Math.Abs(difference) > eps)
                {
                    lastResult = result;
                    poweredA *= (a - 1) / a;
                    result += poweredA / counter;
                    counter++;
                    difference = lastResult - result;
                }
            }
            else    //else use a second
            {
                double z = 1 - a;
                while (Math.Abs(difference) > eps)
                {
                    lastResult = result;
                    poweredA *= z;
                    result -= poweredA / counter;
                    counter++;
                    difference = lastResult - result;
                }
            }
            //returns rounded to 6 decimals
            return Math.Round(result * 1000000) / 1000000;
        }

        /**
         * @brief Function calculates any positive root of a number using Newtons method
         * @param a Number to be rooted
         * @param exponent Exponent of the root
         * @return Returns Nth (exponent) root of "a" rounded on 3 decimals
         **/
        public static double Root(double a, double exponent)
        {
            //checking for math errors
            if (exponent == 0)
                throw new Exception("Math error - attempt to zero root!");
            if (exponent < 0)
                throw new Exception("This function cannot calculate with negative root!");
            if (exponent % 2 == 0 && a < 0)
                throw new Exception("Math error - even root cannot have negative base!");

            //any root of a zero is zero (with algorithm would have some decimal values)
            if (a == 0)
                return 0;

            //selecting random number from 0-9 ... so we pick 5 later it will serve as
            //the result from last step for calculating the difference between steps
            double randomNumber = 5;

            //setting accuracy to 0.001 and initializing difference
            const double eps = 1e-3;
            double difference = 1000;

            double result = 0;
            //calculating by Newton's algorithm till we reach set difference
            while (difference > eps)
            {
                result = ((exponent - 1.0) * randomNumber +
                        a / Math.Pow(randomNumber, exponent - 1)) / (double)exponent;
                difference = Math.Abs(result - randomNumber);
                randomNumber = result;
            }
            //returns rounded to 6 decimals
            return Math.Round(result*1000000) / 1000000;
        }
    }
}
/** @} **/
