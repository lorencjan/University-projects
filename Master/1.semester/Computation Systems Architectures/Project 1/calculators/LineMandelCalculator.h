/**
 * @file LineMandelCalculator.h
 * @author Jan Lorenc <xloren15@stud.fit.vutbr.cz>
 * @brief Implementation of Mandelbrot calculator that uses SIMD paralelization over lines
 * @date 25.10.2021
 */

#include <BaseMandelCalculator.h>

class LineMandelCalculator : public BaseMandelCalculator
{
public:
    LineMandelCalculator(unsigned matrixBaseSize, unsigned limit);
    ~LineMandelCalculator();
    int *calculateMandelbrot();

private:
    int *data;
    float *xBase;
    float *yBase;
    float *xMem;
    float *yMem;
};