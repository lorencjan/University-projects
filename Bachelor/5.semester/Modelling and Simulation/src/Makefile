#  File: Makefile
#  Solution: IMS - project
#			 Implementation of discrete simulator with SHO support
#  Date: 14.11.2020
#  Author: Jan Lorenc & Vojtěch Staněk
#  Faculty: Faculty of information technologies VUT
#  Desription: Makefile for SHO simulator

CC=g++
CFLAGS=-std=c++17 -Wall -Wextra -Werror -pedantic

simulator: main.cpp args.hpp args.cpp system.hpp system.cpp productStack.hpp productStack.cpp appleCollecting.hpp appleCollecting.cpp grinding.hpp grinding.cpp pressing.hpp pressing.cpp nextProcess.hpp nextProcess.cpp generators.hpp generators.cpp
	$(CC) $(CFLAGS) -o $@ $^ -lsimlib -lm

run:
	./simulator