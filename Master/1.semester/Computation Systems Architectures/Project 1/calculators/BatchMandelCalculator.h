/**
 * @file BatchMandelCalculator.h
 * @author Jan Lorenc <xloren15@stud.fit.vutbr.cz>
 * @brief Implementation of Mandelbrot calculator that uses SIMD paralelization over small batches
 * @date 25.10.2021
 */
#ifndef BATCHMANDELCALCULATOR_H
#define BATCHMANDELCALCULATOR_H

#include <BaseMandelCalculator.h>

class BatchMandelCalculator : public BaseMandelCalculator
{
public:
    BatchMandelCalculator(unsigned matrixBaseSize, unsigned limit);
    ~BatchMandelCalculator();
    int * calculateMandelbrot();

private:
    const int batchSize = 32;
    int *data;
    float *xBase;
    float *yBase;
    float *xMem;
    float *yMem;
};

#endif