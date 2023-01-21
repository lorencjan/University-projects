/*
 * File: generators.hpp
 * Solution: IMS - project
 *			 Implementation of discrete simulator with SHO support
 * Date: 14.11.2020
 * Author: Jan Lorenc & Vojtěch Staněk
 * Faculty: Faculty of information technologies VUT
 * Desription: Header file for generator functions.
 */

#ifndef GENERATORS_H
#define GENERATORS_H

#include <random>

using namespace std;
        
/**
 * @brief Generates a pseudorandom number from uniform distibution with given boundaries.
 * @param min Start of the interval from which to generate the random number
 * @param max End of the interval from which to generate the random number
 */
int UniformGenerator(int min, int max);

/**
 * @brief Generates a pseudorandom number from exponential distibution with given mean.
 * @param mean Mean of the exponential distribution
 */
double ExponentialGenerator(double mean);

#endif
