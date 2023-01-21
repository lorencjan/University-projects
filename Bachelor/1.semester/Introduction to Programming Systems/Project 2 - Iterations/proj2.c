/*********************************************/
/**      Projekt 2 - Iteracni vypocty       **/
/**              verze: 1                   **/
/**                                         **/
/**             Jan Lorenc                  **/
/**           login: xloren15               **/
/**             10. 11. 2018                **/
/*********************************************/

#include <stdio.h>
#include <stdlib.h>
#include <math.h>
#include <stdbool.h>
#include <string.h>

#define ARG_FUNC_LENGTH 6            // length of --pow/--log -> 5 chars + \0

//this struct contains all variables for functions from arguments
typedef struct operation
{
    char function[ARG_FUNC_LENGTH];  // variable for function --log/--pow
    unsigned int numOfIterations;    // variable for number of iterations in functions - N
    double base, exponent;           // base and exponent for pow function - X, Y
    double numerus;                  // variable for numerus=agrument of logarithm - X
} Toperation;

// asign variables to struct op
bool asignVariables(char *arguments[], Toperation *op)
{
    if(!strcmp(arguments[1], "--pow"))                             //if the function is pow
    {
        strcpy(op->function, arguments[1]);
        if( !sscanf(arguments[2], "%lg", &op->base) ||            //asign base - X
            !sscanf(arguments[3], "%lg", &op->exponent) ||        //asign exponent - Y
            !sscanf(arguments[4], "%ud", &op->numOfIterations) )  //asign number of iterations - N
        {                                                         //if some of the conversions failed, write error
            fprintf(stderr, "Some of the argument variables could not be converted!\n"
                            "Please enter numbers as variables X, Y, N.\n");
            return false;
        }
    }
    else                                                          //if not pow -> than log
    {
        strcpy(op->function, arguments[1]);
        if( !sscanf(arguments[2], "%lg", &op->numerus) ||         //asign numerus - X
            !sscanf(arguments[3], "%ud", &op->numOfIterations))   //asign number of iterations - N
        {                                                         //if some of the conversions failed, write error
            fprintf(stderr, "Some of the argument variables could not be converted!\n"
                            "Please enter numbers as variables X, N.\n");
            return false;
        }
    }
    return true;
}

// function takes program's arguments, controls them and if ok, asign them to struct op
bool getArguments(int argc, char *arguments[], Toperation *op)
{
    //controls minimum number of arguments
    if(argc < 4)
    {
        fprintf(stderr, "Program has not enough of arguments!\n");
        return false;
    }
    //controls if the arguments contain correct function
    if(strcmp(arguments[1], "--log") && strcmp(arguments[1], "--pow"))
    {
        fprintf(stderr, "Function in arguments is not valid!\nEnter --pow or --log!\n");
        return false;
    }
    //controls number of arguments for each function
    if( (!strcmp(arguments[1], "--pow") && argc!=5) || (!strcmp(arguments[1], "--log") && argc!=4))
    {
        fprintf(stderr, "Program has incorrect number of arguments!\n");
        return false;
    }
    //if all is ok, asign argument values to op struct
    if(!asignVariables(arguments, op))
        return false;
    //there should be at least 1 iteration
    if((signed)op->numOfIterations <= 0)
    {
        fprintf(stderr, "Given number of iterations N must be greater than 0!\n");
        return 0;
    }

    return true;     //if the function came all the way here, everything is ok, returns true
}

// function calculates natural logarithm using taylor polynom
double taylor_log(double x, unsigned int n)
{
   //controls, if numerus of logarithm is greater than 0
   if(x < 0)
      return NAN;
   if(x == 0)
      return -INFINITY;
   double result = 0;
   double poweredX = 1;      //contains actual value of powered x/expression in each iteration
   unsigned int count = 1;   //count must be unsigned - comparing with "n"
   if(x >= 1)                //if x is greater than 1 use following algorithm
       for(;count<=n; count++)
       {
          poweredX *= (x-1)/x;
          result += poweredX / count;
       }
   else                      //else use the second
   {
       double z = 1-x;
       for(;count<=n; count++)
       {
           poweredX *= z;
           result -= poweredX / count;
       }
   }
   return result;
}

// function calculates natural logarithm using fractional method
double cfrac_log(double x, unsigned int n)
{
    //controls, if numerus of logarithm is greater than 0
    if(x < 0)
       return NAN;
    if(x == 0)
       return -INFINITY;
    double z = (x-1)/(x+1);           //z expressed from equation
    double result = 1;                //result starts on 1, so that N iteration ends dividing by 1
    int count = 2*n - 1;              //value from which we take away fraction
    int nominator = n*n;              //value of the biggest nominator
    while(count >= 1)                 //algorithm ends with count = 1 ->the beginning
    {
        //expressed denominator of current fraction - always rising up
        result = count - (nominator*z*z)/result;
        nominator -= count;           //decreasing denominator by count
        count -= 2;                   //decreasing count by 2
    }
    return 2*z/result;                //our result is just intermediate, just dividing 2z in base equation
}

//returns x powered by y using natural logarithm gotten by taylor polynom
double taylor_pow(double x, double y, unsigned int n)
{
    //controls, if base (used in logarithm) is greater than 0
    if(x < 0)
    {
       printf("Negative base! Logarithm used in algorithm doesn't allow it!\n");
       return NAN;                        //using negative in logarithm -> nan
    }
    if(x == 0)
       return 0;
    double result = 1, intermediate = 1;  //initialize result and intermediate as 1
    double Ln = taylor_log(x, n);         //contain natural logarithm of x - better than calling the function each iteration
    for(unsigned int i=1; i<n; i++)       //loop for number of polynom members
    {
        intermediate *= y*Ln/i;           //the very algorithm equation
        result += intermediate;           //adding results of individual members = intermediates
    }
    return result;
}

//same as previous but using fractional method to get natural logarithm
double taylorcf_pow(double x, double y, unsigned int n)
{
    if(x < 0)
    {
       printf("Negative base! Logarithm used in algorithm doesn't allow it!\n");
       return NAN;
    }
    if(x == 0)
       return 0;
    double result = 1, intermediate = 1;
    double Ln = cfrac_log(x, n);
    for(unsigned int i=1; i<n; i++)
    {
        intermediate *= y*Ln/i;
        result += intermediate;
    }
    return result;
}

int main(int argc, char *argv[])
{
    Toperation op = {"", 0, 0, 0, 0};      // initialization of a struct containing variables for functions
    if(!getArguments(argc, argv, &op))     // controlling arguments and giving them to the struct
        return 0;

    if(!strcmp(op.function, "--log"))      // if given function is logarithm, execute logarithm functions
    {
        printf("%10s(%.4f) = %.12g\n", "log", op.numerus, log(op.numerus));
        printf("%10s(%.4f) = %.12g\n", "cfrac_log", op.numerus, cfrac_log(op.numerus, op.numOfIterations));
        printf("taylor_log(%.4f) = %.12g\n", op.numerus, taylor_log(op.numerus, op.numOfIterations));
    }
    else                                   // else the given function is power -> execute logarithm functions
    {
        printf("%12s(%.2f,%.1f) = %.12g\n", "pow", op.base, op.exponent, pow(op.base, op.exponent));
        printf("%12s(%.2f,%.1f) = %.12g\n", "taylor_pow", op.base, op.exponent, taylor_pow(op.base, op.exponent, op.numOfIterations));
        printf("taylorcf_pow(%.2f,%.1f) = %.12g\n", op.base, op.exponent, taylorcf_pow(op.base, op.exponent, op.numOfIterations));
    }
    return 0;
}
