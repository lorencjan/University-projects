#!/bin/bash

gcc -std=c99 -Wall -Wextra -Werror proj1.c -o proj1

./proj1 i.txt <text.txt
echo $?

./proj1 ni.txt <text.txt 
echo $?

./proj1 a.txt <text.txt 
echo $?

./proj1 dni.txt <text.txt 
echo $?

./proj1 chyba1.txt <text.txt
echo $?

./proj1 chyba2.txt <text.txt
echo $?

#./proj1 cycle1.txt <text.txt 
#echo $?

#./proj1 cycle2.txt <text.txt  
#echo $?

#./proj1 cycle3.txt <text.txt 
#echo $?

#./proj1 notreallycycle.txt <text.txt 
#echo $?

