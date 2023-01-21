#!/bin/bash

#  File: test.sh
#  Solution: PRL - Proj 2 - Assign preorder order to vertices
#  Date: 22.04.2022
#  Author: Jan Lorenc
#  Description: Testing script which builds, runs and cleans.

if [ $# -ne 1 ]
then
    echo "Argument error: Exactly 1 argument is expected and it's input string."
    exit 1
fi

input=$1;
input_len=${#input};
let num_of_procs=2*$input_len-2;
if [ $num_of_procs -eq 0 ]; then num_of_procs=1; fi

mpic++ --prefix /usr/local/share/OpenMPI -o pro pro.cpp
mpirun --prefix /usr/local/share/OpenMPI -oversubscribe -np $num_of_procs pro $input

rm -f pro 
