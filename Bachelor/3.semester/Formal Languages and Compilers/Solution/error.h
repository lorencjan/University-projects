/**
 * University: University of technology (VUT) Brno
 * Faculty: Faculty of information technologies
 * 
 * Project: Implementation of compiler for imperative language IFJ19
 * Subject: IFJ & IAL
 * Team: 007
 * Variant: 1
 * 
 * Authors: Dominik Kaláb   (xkalab11)
 *          Vojtěch Staněk  (xstane45)
 *          Jan Lorenc      (xloren15)
 *          Matěj Pokorný   (xpokor78)
 * 
 * @file error.h
 * @brief Header file containing definitions for runtime errrors.
 *
 */

#ifndef ERROR_H
#define ERROR_H


#define LEX_ERROR 1             //lexical analysis error
#define SYN_ERROR 2             //syntactic analysis error
#define DEF_ERROR 3             //error while trying to define or redifine functions/variables
#define TYPE_COMP_ERROR 4       //error in type compatibility
#define PARAM_ERROR 5           //wrong number of parameters
#define SEM_ERROR 6             //other semantic error
#define ZERO_DIV_ERROR 9        //zero devision error
#define INT_ERROR 99            //other internal error
#endif


