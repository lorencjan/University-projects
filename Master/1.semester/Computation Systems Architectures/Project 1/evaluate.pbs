#!/bin/bash
#PBS -q qexp
#PBS -A OPEN-20-37
#PBS -l select=1:ncpus=36,vtune=2020_update3-GCC 
#PBS -l walltime=0:05:00
#PBS -m e
#PBS -N AVS-evaluate


cd $PBS_O_WORKDIR
ml CMake intel-compilers/2021.1.2


rm -rf build_evaluate
mkdir build_evaluate
cd build_evaluate

CC=icc CXX=icpc cmake ..
make


SHAPES=(512 1024 2048 4096)
ITERS=(100 1000)
CALCULATORS=("ref" "line" "batch")

    for calc in "${CALCULATORS[@]}"; do
        for run in `seq 3`; do
            (
                for iter in "${ITERS[@]}"; do
                    for shape in "${SHAPES[@]}"; do
                        ./mandelbrot -s $shape -i $iter -c $calc --batch > "tmp_${calc}_${run}_${iter}_${shape}"
                    done
                done
            ) &
            p=$!
            echo $p
            pids[${i}]=$p
            echo "pids" ${pids[*]}
        done
    done

# wait for all pids
for pid in ${pids[*]}; do
    echo "wait for $pid"
    wait $pid
done



 (
    echo "CALCULATOR;BASE;WIDTH;HEIGHT;ITERS;TIME"
    for calc in "${CALCULATORS[@]}"; do
        for run in `seq 3`; do
            for iter in "${ITERS[@]}"; do
                for shape in "${SHAPES[@]}"; do
                    cat "tmp_${calc}_${run}_${iter}_${shape}"
                done
            done &
        done
    done
 ) | tee ../datalog.csv