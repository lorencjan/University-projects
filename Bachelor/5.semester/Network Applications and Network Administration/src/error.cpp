/*
 *  File: error.cpp
 *  Solution: ISA - project - Monitoring of SSL connections
 *  Date: 17.10.2020
 *  Author: Jan Lorenc
 *  Faculty: Faculty of information technologies VUT
 *  Description: Source file for error handling function
 */

#include "error.h"

void Error(string msg)
{
    cerr << msg;
	exit(1);
}
