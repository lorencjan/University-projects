/*
 * File: productStack.hpp
 * Solution: IMS - project
 *			 Implementation of discrete simulator with SHO support
 * Date: 14.11.2020
 * Author: Jan Lorenc & Vojtěch Staněk
 * Faculty: Faculty of information technologies VUT
 * Desription: Header file for ProductStack class.
 */

#ifndef PRODUCT_STACK_H
#define PRODUCT_STACK_H

#include <iostream>
#include <iomanip>
#include <string>

using namespace std;

/**
 * @brief Collection keeping track of process products. Basically just an advanced counter.
 */
class ProductStack
{
    private:
        /**
         * @brief Name of the structure that this instance is representing.
         */
        string name;

        /**
         * @brief Number of items in the stack.
         */
        int count = 0;

        /**
         * @brief Maximum number of items in the stack in the whole simulation.
         */
        int max = 0;

        /**
         * @brief Total amount of items stored. Basically sum of inputs.
         */
        int total = 0;

        /**
         * @brief Sum of items at the time of inputs. Enables us to calculate mean at the end.d
         */
        int sumOfCurrentItems = 0;

        /**
         * @brief Number of inputs to the stack.
         */
        int inputs = 0;

        /**
         * @brief Number of items with which we can work.
         */
        int required = 0;
    
    public:
        ProductStack(string name, int requiredItems);
        ~ProductStack();
        
        /**
         * @brief Adds specfied number of items to the stack
         * @param number Number by which to increment the stack
         */
        void Add(int number);

        /**
         * @return Outputs statistics about the stack (number of inputs, maximum amount of items at a time, mean)
         */
        void Output();

        /**
         * @brief Checks whether there's enough items to proceed and if so, removes them from the stack
         * @return True if the items were consumer, false if there's not enough of them
         */
        bool Consume();
};

#endif
