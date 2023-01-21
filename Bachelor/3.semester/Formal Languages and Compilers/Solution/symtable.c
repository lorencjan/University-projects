/**
 * University: University of technology (VUT) Brno
 * Faculty: Faculty of information technologies
 * 
 * Project: Implementation of a compiler for imperative language IFJ19
 * Subject: IFJ & IAL
 * Team: 007
 * Variant: 1
 * 
 * Authors: Dominik Kaláb   (xkalab11)
 *          Vojtěch Staněk  (xstane45)
 *          Jan Lorenc      (xloren15)
 *          Matěj Pokorný   (xpokor78)
 * 
 * @file symtable.c
 * @brief Implementation of symbol table structure and its methods
 *
 */

#include "symtable.h"

symbolInfo_t *initSymbolInfo()
{
    symbolInfo_t *node = malloc(sizeof(symbolInfo_t));
    if (node == NULL)
        return NULL;

    node->varType = -1;
    node->definedInAssembly = false;
    node->definedInLoop = false;
    return node;
}

void delSymbolInfo(symbolInfo_t *node)
{
    free(node);
}

symbolTable_t *initST()
{
    symbolTable_t *root = malloc(sizeof(symbolTable_t));
    if (root == NULL)
        return NULL;

    root->name = NULL;
    root->leftPtr = NULL;
    root->rightPtr = NULL;
    root->symInfo = NULL;

    return root;
}

void delST(symbolTable_t *root)
{
    if (root == NULL) return;

    delST(root->leftPtr);
    delST(root->rightPtr);
    if(root->name) free(root->name);
    if(root->symInfo) free(root->symInfo);
    free(root);
    root = NULL;
}

void detachGlobalUsedInLocal(symbolTable_t *root)
{
    if (root == NULL) return;

    detachGlobalUsedInLocal(root->leftPtr);
    detachGlobalUsedInLocal(root->rightPtr);
    root->symInfo->globalUsedInLocal = false;
}

int insertNodeST(symbolTable_t *root, char *name, symbolInfo_t *info)
{
    if (name == NULL || info == NULL)
        return EXIT_FAILURE;

    while (root->name != NULL)
    {
        int cmp = strcmp(root->name, name);

        if (cmp == 0)
            return 1;
        else if (cmp > 0)
        {
            if (root->leftPtr == NULL)
                root->leftPtr = initST();
            root = root->leftPtr;
        }
        else
        {
            if (root->rightPtr == NULL)
                root->rightPtr = initST();
            root = root->rightPtr;
        }
    }
    root->name = malloc((strlen(name)+1)*sizeof(char));
    strcpy(root->name, name);
    root->symInfo = info;
    return 0;
}

symbolTable_t *searchST(symbolTable_t *root, char *name)
{
    if (root == NULL || root->name == NULL || name == NULL){
        return NULL;
    }
    
    int cmp = strcmp(root->name, name);

    if (cmp == 0){
        return root;
    }
    else if (cmp > 0)
    {
        return searchST(root->leftPtr, name);
    }
    else
    {
        return searchST(root->rightPtr, name);
    }
}

int insertSymbol(int tableType, char *name, int symType, bool defined, bool definedInAssebly, bool definedInLoop, bool globalUsedInLocal, int params, int varType)
{
    symbolTable_t *STPtr;

    if (tableType == GLOBAL)
        STPtr = globalST;
    else if (tableType == LOCAL)
        STPtr = localST;
    else
        return EXIT_FAILURE;

    symbolInfo_t *symInfo = initSymbolInfo();
    if (symInfo == NULL)
        return EXIT_FAILURE; //init and check on one line

    symInfo->varType = varType;
    symInfo->parameters = params;
    symInfo->defined = defined;
    symInfo->definedInAssembly = definedInAssebly;
    symInfo->definedInLoop = definedInLoop;
    symInfo->globalUsedInLocal = globalUsedInLocal;
    symInfo->symType = symType;

    return insertNodeST(STPtr, name, symInfo);
}

int checkUndefined(symbolTable_t * symtable){
    if(symtable == NULL) return 0; //End of tree
    if(symtable->name == NULL) return 0; //tree allocated, but not yet used
    if(symtable->symInfo == NULL) return 99; //INT_ERROR
    
    if(symtable->symInfo->defined == false) 
    {
        
        return 2; //SYN_ERROR
    }
    else {
        return (checkUndefined(symtable->leftPtr) || checkUndefined(symtable->rightPtr));
    }
}
