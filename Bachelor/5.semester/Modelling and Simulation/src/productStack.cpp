/*
 * File: productStack.cpp
 * Solution: IMS - project
 *			 Implementation of discrete simulator with SHO support
 * Date: 14.11.2020
 * Author: Jan Lorenc & Vojtěch Staněk
 * Faculty: Faculty of information technologies VUT
 * Desription: Source file for ProductStack class.
 */

#include "productStack.hpp"

ProductStack::ProductStack(string name, int requiredItems)
{
    this->name = name;
    required = requiredItems;
}

ProductStack::~ProductStack() {}

void ProductStack::Add(int number)
{
    count += number;
    total += number;
    sumOfCurrentItems += count;
    inputs++;
    if(count > max)
        max = count;
}

void ProductStack::Output()
{
    cout << "+----------------------------------------------------------+" << endl;
    cout << "| PRODUCT STACK " << name << setw(44 - name.length()) << "|" << endl;
    cout << "+----------------------------------------------------------+" << endl;
    cout << "|  Current count: " << count << setw(42 - to_string(count).length()) << "|" << endl;
    cout << "|  Total count: " << total << setw(44 - to_string(total).length()) << "|" << endl;
    cout << "|  Maximum count: " << max << setw(42 - to_string(max).length()) << "|" << endl;
    string mean = to_string(sumOfCurrentItems / (float)inputs);
    cout << "|  Mean: " << mean << setw(51 - mean.length()) << "|" << endl;
    cout << "|  Number of inputs: " << inputs << setw(39 - to_string(inputs).length()) << "|" << endl;
    cout << "+----------------------------------------------------------+" << endl;
    cout << endl;
}

bool ProductStack::Consume()
{
    if(count < required)
        return false;

    count -= required;
    return true;
}
