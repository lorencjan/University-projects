/*
 * File: generators.cpp
 * Solution: IMS - project
 *			 Implementation of discrete simulator with SHO support
 * Date: 14.11.2020
 * Author: Jan Lorenc & Vojtěch Staněk
 * Faculty: Faculty of information technologies VUT
 * Desription: Source file for generator functions.
 */

#include "generators.hpp"

int UniformGenerator(int min, int max)
{
    random_device rd;
    mt19937 generator(rd());
    uniform_int_distribution<int> distribution(min, max);
    return distribution(generator);
}

double ExponentialGenerator(double mean)
{
    random_device rd;
    mt19937 generator(rd());
    exponential_distribution<double> distribution(1/mean);
    return distribution(generator);
}
