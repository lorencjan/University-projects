#  File: Makefile
#  Solution: IOS - project 2
#  Author: Jan Lorenc
#  Date: 12.4.2019
#  Faculty: Faculty of information technologies VUT
#  Desription: Makefile for the second IOS project

CC     = gcc
CFLAGS = -std=gnu99 -Wall -Wextra -Werror -pedantic -pthread

proj2: main.c rivercrossing.c rivercrossing.h
	$(CC) $(CFLAGS) -o $@ $^ -lrt
	