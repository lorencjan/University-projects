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
 * @file symtable.h
 * @brief Header file for symbol table structure
 *
 */
#ifndef SYMTABLE_H
#define SYMTABLE_H

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <stdbool.h>

/**
 * Enum of symbol types
 */
typedef enum symbolType
{
    VAR,
    FUNC
} symbolType;

/**
 * @struct symbolInfo
 * @brief Struct to hold info about symbol
 */
typedef struct symbolInfo
{
    symbolType symType;
    int varType;
    int parameters;
    bool defined;
    bool definedInAssembly;
    bool definedInLoop;
    bool globalUsedInLocal;
} symbolInfo_t;

/**
 * Enum for deciding whether table is global or local
 */
enum
{
    GLOBAL,
    LOCAL
};

/**
 * @struct symbolTable
 * @brief Struct reprezenting table of symbols
 *
 * Symbol table structure. Since it's implemented in the way of a binary
 * search tree, it holds two pointers (to left and right child) in
 * addition to its name and pointer to its struct info.
 */
typedef struct symbolTable
{
    struct symbolTable *leftPtr;
    struct symbolTable *rightPtr;
    char *name;
    symbolInfo_t *symInfo;
} symbolTable_t;

/**
 * @brief Initialize symbolInfo - constructor
 *
 * @return Pointer to initialized node
 */
symbolInfo_t *initSymbolInfo();

/**
 * @brief Delete symbolInfo - destructor
 *
 * @param node Pointer of node to delete
 */
void delSymbolInfo(symbolInfo_t *node);

//

/**
 * @brief Initialize symbol table - constructor
 *
 * @return Pointer to initialized tree
 */
symbolTable_t *initST();

/**
 * @brief Insert new node to symbol table tree
 *
 * @param root Pointer to root of symbol table tree
 * @param name Name of the node
 * @param info Symbol info
 *
 * @return non-zero value if error occured, zero if all ok
 */
int insertNodeST(symbolTable_t *root, char *name, symbolInfo_t *info);

/**
 * @brief Search symbol table tree for node with matching name
 *
 * @param root Pointer to root of symbol table tree
 * @param name Name of the node
 *
 * @return pointer to matching symbolTable_t node
 */
symbolTable_t *searchST(symbolTable_t *root, char *name);

/**
 * @brief Delete and free symbol table tree - destructor
 *
 * @param root Pointer to root of symbol table tree to delete
 */
void delST(symbolTable_t *root);

/**
 * @brief Set globalUsedInLocal to false for all members
 *
 * @param root Pointer to root of symbol table tree to modify
 */
void detachGlobalUsedInLocal(symbolTable_t *root);

/**
 * @brief Insert a specific symbol into symbol table
 *
 * @param tableType           Into which table to insert (GLOBAL/LOCAL)
 * @param name                Symbol name
 * @param symType             Type of symbol (VAR_T/FUNC_T) of symbolInfo
 * @param defined             Informs us whether the variable has been defined
 * @param definedInAssembly   Informs us whether the variable has been defined in IFJcode19 (printed)
 * @param definedInLoop       Informs us whether the variable has been defined inside loop - special kind of local (TF)
 * @param globalUsedInLocal   Informs us whether the variable is global and has been used in local frame before bein shadowed by local var of the same name
 * @param params              Parameters of symbolInfo
 * @param label               Label of symbolInfo
 *
 * @return non-zero value if error occured, 0 if everything ok
 */
int insertSymbol(int tableType, char *name, int symType, bool defined, bool definedInAssembly, bool definedInLoop, bool globalUsedInLocal, int params, int varType);

/**
 * @brief Check symtable for undefined functions and variables
 * 
 * @param symtable Symbol table to check
 * 
 * @return 0 if all symbols are defined, non-zero when at least one is undefined
 */
int checkUndefined(symbolTable_t * symtable);

// Declare global and local symbol tables
symbolTable_t *globalST;
symbolTable_t *localST;

#endif
