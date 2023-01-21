/*
 *  File: error.h
 *  Solution: ISA - project - Monitoring of SSL connections
 *  Date: 17.10.2020
 *  Author: Jan Lorenc
 *  Faculty: Faculty of information technologies VUT
 *  Description: Header file for error handling function
 */

#ifndef ERROR_H
#define ERROR_H

#include <iostream>
#include <string>

using namespace std;

/**
 * @brief Ends program with error 1
 * @param msg Custom error message
 */
void Error(string msg);

#endif