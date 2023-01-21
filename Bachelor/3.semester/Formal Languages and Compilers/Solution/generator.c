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
 * @file generator.c
 * @brief Implementation of generator for IFJcode19
 *
 */

#include "generator.h"

int currentLabelNumber = 0;       //used for numbering labels
int currentVariableNumber = 0;    //used for nubering variables

void printStart(){
    printf(".IFJcode19\n");
}

int printInputs(){
    printInstruction(READ_I, "TF@%retVal", "string", NULL);
    return STRING_T;
}

int printInputi(){
    printInstruction(READ_I, "TF@%retVal", "int", NULL);
    return INT_T;
}

int printInputf(){
    printInstruction(READ_I, "TF@%retVal", "float", NULL);
    return FLOAT_T;
}

int printPrint(List *params){
    listItem *item = params->first; 
    while(item)
    {
        char str[35]; //%a shortens whatever number to approximatelly same char length, sufficient reserver for even too huge doubles
        switch (item->value->type){
            case ID_T:;
                char * writeString = malloc(sizeof(char)*50); //let's say max length of ID name is 50 chars
                String *string = initString();
                symbolTable_t *ST = actSymtable ? localST : globalST;
                symbolTable_t *found = searchST(ST, item->value->stringVal->string);
                if(found && found->symInfo->definedInLoop)
                    appendString(string, "TF@");
                else if(found && actSymtable==LOCAL){
                    appendString(string, "LF@");
                }
                else
                {
                    //if I use global var in a local frame and this global is not shadowed by a local, make it used in local frame
                    if(actSymtable==LOCAL)
                    {
                        symbolTable_t *foundGlobal = searchST(globalST, item->value->stringVal->string);
                        if(foundGlobal)
                            foundGlobal->symInfo->globalUsedInLocal = true;
                    }
                    appendString(string, "GF@");
                }
                sprintf(writeString, "%s", item->value->stringVal->string);
                appendString(string, writeString);
                printInstruction(WRITE_I, string->string, NULL, NULL);
                free(writeString);
                delString(string);
                break;
            case STRING_T:;
                String *s = generatorString(item->value->stringVal->string);
                printInstruction(WRITE_I, s->string, NULL, NULL);
                delString(s);
                break;
            case INT_T:
                sprintf(str, "int@%d", item->value->intVal);
                printInstruction(WRITE_I, str, NULL, NULL);
                break;
            case FLOAT_T:
                sprintf(str, "float@%a", item->value->floatVal);
                printInstruction(WRITE_I, str, NULL, NULL);
                break;
            default: break;
        }
        if(item->next)
            printInstruction(WRITE_I, "string@\\032", NULL, NULL);
        item = item->next;
    }
    printInstruction(WRITE_I, "string@\\010", NULL, NULL);
    return 0;
}

int printLen(char *str, bool id){
    if(id) //if it's id, put there ifjcode19 var, which is in str
    {
        printInstruction(STRLEN_I, "TF@%retVal", str, NULL);
    }
    else //otherwise the string literal
    {
        String *s = generatorString(str);
        printInstruction(STRLEN_I, "TF@%retVal", s->string, NULL);
        delString(s);
    }
    return INT_T;
}

void printSubstr(char *str, char *index, char *n){
    //if(index < 0) ->nothing
    char *label = getLabel();
    char *nNotGreaterThanLenSubN = getLabel();
    char *substrLoopLabel = getLabel();
    printInstruction(DEFVAR_I, "TF@%conditionResult", NULL, NULL);
    printInstruction(LT_I, "TF@%conditionResult", index, "int@0");
    printInstruction(JUMPIFEQ_I, label, "bool@true", "TF@%conditionResult");
    //if(index > strlen(str)) ->nothing
    printInstruction(DEFVAR_I, "TF@%lenOfString", NULL, NULL);
    printInstruction(STRLEN_I, "TF@%lenOfString", str, NULL);
    printInstruction(GT_I, "TF@%conditionResult", index, "TF@%lenOfString");
    printInstruction(JUMPIFEQ_I, label, "bool@true", "TF@%conditionResult");
    //if(n < 0) ->nothing
    printInstruction(LT_I, "TF@%conditionResult", n, "int@0");
    printInstruction(JUMPIFEQ_I, label, "bool@true", "TF@%conditionResult");
    //if(n > len(str)−index) -> n = len(str) - i
    printInstruction(DEFVAR_I, "TF@%Nchars", NULL, NULL);
    printInstruction(MOVE_I, "TF@%Nchars", n, NULL);
    printInstruction(DEFVAR_I, "TF@%lenSubIndex", NULL, NULL);
    printInstruction(SUB_I, "TF@%lenSubIndex", "TF@%lenOfString", index);
    printInstruction(GT_I, "TF@%conditionResult", n, "TF@%lenSubIndex");
    printInstruction(JUMPIFNEQ_I, nNotGreaterThanLenSubN, "bool@true", "TF@%conditionResult");
    printInstruction(MOVE_I, "TF@%Nchars", "TF@%lenSubIndex", NULL);
    printInstruction(LABEL_I, nNotGreaterThanLenSubN, NULL, NULL);
    //substr
    printInstruction(DEFVAR_I, "TF@%currIndex", NULL, NULL);
    printInstruction(MOVE_I, "TF@%currIndex", index, NULL);
    printInstruction(DEFVAR_I, "TF@%currentChar", NULL, NULL);
    printInstruction(MOVE_I, "TF@%retVal", "string@", NULL);
    printInstruction(LABEL_I, substrLoopLabel, NULL, NULL);
    printInstruction(GETCHAR_I, "TF@%currentChar", str, "TF@%currIndex");
    printInstruction(CONCAT_I, "TF@%retVal", "TF@%retVal", "TF@%currentChar");
    printInstruction(ADD_I, "TF@%currIndex", "TF@%currIndex", "int@1");
    printInstruction(SUB_I, "TF@%Nchars", "TF@%Nchars", "int@1");
    printInstruction(GT_I, "TF@%conditionResult", "TF@%Nchars", "int@0");
    printInstruction(JUMPIFEQ_I, substrLoopLabel, "bool@true", "TF@%conditionResult");
    //ending error label
    printInstruction(LABEL_I, label, NULL, NULL);
    free(label);
    free(nNotGreaterThanLenSubN);
    free(substrLoopLabel);
}

void printOrd(char *val, char *index){
    //if(index < 0) ->nothing
    char *label = getLabel();
    printInstruction(DEFVAR_I, "TF@%conditionResult", NULL, NULL);
    printInstruction(LT_I, "TF@%conditionResult", index, "int@0");
    printInstruction(JUMPIFEQ_I, label, "bool@true", "TF@%conditionResult");
    //if(index > strlen(str)) ->nothing
    printInstruction(DEFVAR_I, "TF@%lenOfString", NULL, NULL);
    printInstruction(STRLEN_I, "TF@%lenOfString", val, NULL);
    printInstruction(GT_I, "TF@%conditionResult", index, "TF@%lenOfString");
    printInstruction(JUMPIFEQ_I, label, "bool@true", "TF@%conditionResult");
    //Chr(val)
    printInstruction(STRI2INT_I, "TF@%retVal", val, index);
    //label to skip
    printInstruction(LABEL_I, label, NULL, NULL);
    free(label);
}

void printChr(char *val){
    //if(i >=0 && i < 256)
    char *label = getLabel();
    printInstruction(DEFVAR_I, "TF@%conditionResult", NULL, NULL);
    printInstruction(GT_I, "TF@%conditionResult", val, "int@255");
    printInstruction(JUMPIFEQ_I, label, "bool@true", "TF@%conditionResult");
    printInstruction(LT_I, "TF@%conditionResult", val, "int@0");
    printInstruction(JUMPIFEQ_I, label, "bool@true", "TF@%conditionResult");
    //Chr(val)
    printInstruction(INT2CHAR_I, "TF@%retVal", val, NULL);
    //label to skip
    printInstruction(LABEL_I, label, NULL, NULL);
    free(label);
}

int printFunctionDefinition(char *funcName, List *params, int paramsCount)
{
    skipLabel = getLabel(); //while executing code, we need to skip definitions execution
    printInstruction(JUMP_I, skipLabel, NULL, NULL);
    printInstruction(LABEL_I, funcName, NULL, NULL);
    printInstruction(PUSHFRAME_I, NULL, NULL, NULL);
    listItem *param = params->first;
    for(int i = 1; i <= paramsCount && param != NULL; i++)
    {
        //set it as defined in localST - in func it's always local
        char *s = param->value->stringVal->string;
        symbolTable_t *found = searchST(localST, s);
        if(!found)
        {
            found = searchST(globalST, s); //it might be already used as global ->error
            if(found && found->symInfo->globalUsedInLocal)
                return DEF_ERROR;
            insertSymbol(LOCAL, s, 0, true, true, inLoop, false, 0, 0);
        }
        else
            found->symInfo->definedInAssembly = true;
        //variable definition for the function parameter
        char *paramName = malloc(sizeof(strlen(s)) + 3*sizeof(char) + 1);
        sprintf(paramName, "LF@%s", s);
        printInstruction(DEFVAR_I, paramName, NULL, NULL);
        //parameter value assignment
        char paramVal[10];
        sprintf(paramVal, "LF@%%_%d", i);
        printInstruction(MOVE_I, paramName, paramVal, NULL);
        free(paramName);
        //next parameter
        param = param->next;
    }
    return 0;
}

int printFunctionCall(char *name, List *params, int paramsCount, char *retVal)
{
    //definition of return variable
    char *returnName = NULL;
    if(retVal)
    {
        //check if the return value isn't already defined
        bool isDefined = false;
        int errCode = generateNameOfIdentifier(&returnName, retVal, &isDefined);
        if(errCode)
        {
            if(returnName) free(returnName);
            return errCode;
        }
        //define is it hasn't already been done
        if(!isDefined)
            printInstruction(DEFVAR_I, returnName, NULL, NULL);
    }
    //create temporary frame and prepare return value
    if(!inLoop)
    {
        printInstruction(CREATEFRAME_I, NULL, NULL, NULL);
        printInstruction(DEFVAR_I, "TF@%retVal", NULL, NULL);
    }
    printInstruction(MOVE_I, "TF@%retVal", "nil@nil", NULL);
    
    int returnType = -1;
    //check for a builtin and call it if it's so
    if(!strcmp(name, "print"))
    {
        int errCode = printPrint(params);
        if(errCode){
            if(returnName)
                free(returnName);
            return errCode;
        }
        returnType = NONE_T;
    }
    else if(!strcmp(name, "inputs"))
    {
        if(paramsCount != 0) return PARAM_ERROR;
        returnType = printInputs();
    }
    else if(!strcmp(name, "inputi"))
    {
        if(paramsCount != 0) return PARAM_ERROR;
        returnType = printInputi();
    }
    else if(!strcmp(name, "inputf"))
    {
        if(paramsCount != 0) return PARAM_ERROR;
        returnType = printInputf();
    }
    else if(!strcmp(name, "len"))
    {
        if(paramsCount != 1)
            return PARAM_ERROR;
        Token_t *t = params->first->value;
        if(t->type != STRING_T && t->type != ID_T)
            return TYPE_COMP_ERROR;
        if(t->type == ID_T) //it's a param in a function, we have this value in a var
        {
            char *str = NULL;
            int errCode = generateNameOfIdentifier(&str, t->stringVal->string, NULL);
            if(errCode)
            {
                if(str) free(str);
                return errCode;
            }
            returnType = printLen(str, true);
            free(str);
        }
        else
        {
            returnType = printLen(t->stringVal->string, false);
        }
    }
    else if(!strcmp(name, "substr"))
    {
        returnType = STRING_T;
        //check param count
        if(paramsCount != 3)
            return PARAM_ERROR;
        //check first param type
        listItem *i = params->first;
        if(i->value->type != STRING_T && i->value->type != ID_T)
            return TYPE_COMP_ERROR;
        //prepare it's ifjcode19 value
        char *str = i->value->stringVal->string;
        char *tmp = NULL;
        String *genString = NULL;
        if(i->value->type == ID_T)
        {
            int errCode = generateNameOfIdentifier(&tmp, str, NULL);
            if(errCode)
            {
                if(tmp) free(tmp);
                return errCode;
            }
        }
        else
        {
            genString = generatorString(str);
        }
        //check second param type
        i = i->next;
        if(i->value->type != INT_T && i->value->type != ID_T)
            return TYPE_COMP_ERROR;
        //get it's ifjcode value
        char *index = NULL;
        if(i->value->type == ID_T)
        {
            int errCode = generateNameOfIdentifier(&index, i->value->stringVal->string, NULL);
            if(errCode)
            {
                if(index) free(index);
                return errCode;
            }
        }
        else
        {
            index = malloc(sizeof(char)*20);
            if(!index) return INT_ERROR;
            sprintf(index, "int@%d", i->value->intVal);
        }
        //check third param type
        i = i->next;
        if(i->value->type != INT_T && i->value->type != ID_T)
            return TYPE_COMP_ERROR;
        //get it's ifjcode value
        char *n;
        if(i->value->type == ID_T)
        {
            int errCode = generateNameOfIdentifier(&n, i->value->stringVal->string, NULL);
            if(errCode)
            {
                if(n) free(n);
                return errCode;
            }
        }
        else
        {
            n = malloc(sizeof(char)*20);
            if(!n) return INT_ERROR;
            sprintf(n, "int@%d", i->value->intVal);
        }
        //call function generator
        str = (genString) ? genString->string : tmp;
        printSubstr(str, index, n);
        //free resources
        if(genString) delString(genString);
        if(tmp) free(tmp);
        free(index);
        free(n);
    }
    else if(!strcmp(name, "ord"))
    {
        returnType = INT_T;
        //check param count
        if(paramsCount != 2)
            return PARAM_ERROR;
        //check first param type
        listItem *i = params->first;
        if(i->value->type != STRING_T && i->value->type != ID_T)
            return TYPE_COMP_ERROR;
        //prepare it's ifjcode19 value
        char *str = i->value->stringVal->string;
        char *tmp = NULL;
        String *genString = NULL;
        if(i->value->type == ID_T)
        {
            int errCode = generateNameOfIdentifier(&tmp, str, NULL);
            if(errCode)
            {
                if(tmp) free(tmp);
                return errCode;
            }
        }
        else
        {
            genString = generatorString(str);
        }
        //check second param type
        i = i->next;
        if(i->value->type != INT_T && i->value->type != ID_T)
            return TYPE_COMP_ERROR;
        //get it's ifjcode value
        char *index = NULL;
        if(i->value->type == ID_T)
        {
            int errCode = generateNameOfIdentifier(&index, i->value->stringVal->string, NULL);
            if(errCode)
            {
                if(index) free(index);
                return errCode;
            }
        }
        else
        {
            index = malloc(sizeof(char)*20);
            sprintf(index, "int@%d", i->value->intVal);
        }
        //call the function
        if(genString)
            printOrd(genString->string, index);
        else
            printOrd(tmp, index);
        //free resources and resolve return type
        if(tmp) free(tmp);
        if(genString) delString(genString);
        free(index);
        if(i->value->intVal < 0  || i->value->intVal > (int)strlen(str))
            returnType = NONE_T;
    }
    else if(!strcmp(name, "chr"))
    {
        returnType = STRING_T;
        if(paramsCount != 1)
            return PARAM_ERROR;
        Token_t *t = params->first->value;
        if(t->type != INT_T && t->type != ID_T)
            return TYPE_COMP_ERROR;
        
        if(t->type == ID_T) //it's a param in a function, we have this value in a var
        {
            char *str = NULL;
            int errCode = generateNameOfIdentifier(&str, t->stringVal->string, NULL);
            if(errCode)
            {
                if(str) free(str);
                return errCode;
            }
            printChr(str);
            free(str);
        }
        else
        {
            char tmp[20];
            sprintf(tmp, "int@%d", t->intVal);
            printChr(tmp);
            if(t->intVal>255 || t->intVal<0)
                returnType = NONE_T;
        }
    }
    else  //it's not a built-in, call user function
    {
        //define all of the parameters and set their values
        listItem *param = params->first;
        for(int i = 1; i <= paramsCount && param != NULL; i++)
        {
            char paramName[10];
            sprintf(paramName, "TF@%%_%d", i);
            printInstruction(DEFVAR_I, paramName, NULL, NULL);
            char paramVal[25];
            switch (param->value->type)
            {
                case INT_T:
                    sprintf(paramVal, "int@%d", param->value->intVal);
                    printInstruction(MOVE_I, paramName, paramVal, NULL);
                    break;
                case FLOAT_T:
                    sprintf(paramVal, "float@%a", param->value->floatVal);
                    printInstruction(MOVE_I, paramName, paramVal, NULL);
                    break;
                case ID_T:;
                    char id[200];
                    if(actSymtable==GLOBAL)
                        sprintf(id, "GF@%s", param->value->stringVal->string);
                    else
                        sprintf(id, "LF@%s", param->value->stringVal->string);
                    printInstruction(MOVE_I, paramName, id, NULL);
                    break;
                default:;
                    String *s = generatorString(param->value->stringVal->string);
                    printInstruction(MOVE_I, paramName, s->string, NULL);
                    delString(s);
                    break;
            }
            param = param->next;
        }
        //ready to call the function, the temporary frame will be pushed inside
        printInstruction(CALL_I, name, NULL, NULL);
    }
    //if there is an identifier to assign the value to, assign, otherwise non-expression call, retVall not needed
    if(retVal)
    {
        //assign result type of builtin
        if(returnType != -1)
        {
            symbolTable_t *ST = (actSymtable) ? localST : globalST;
            symbolTable_t *found = searchST(ST, retVal); //now we know it's there 'cause we already inserted it higher
            found->symInfo->varType = returnType;            
        }
        printInstruction(MOVE_I, returnName, "TF@%retVal", NULL);
        free(returnName);
    }

    return 0;
}

int generateNameOfIdentifier(char **ifjCodeName, char *idName, bool *isDefined)
{
    //create it's ifjcode variable
    *ifjCodeName = malloc(sizeof(char)*(strlen(idName)+3+1));
    if(!*ifjCodeName) return INT_ERROR;
    if(actSymtable==GLOBAL)
        sprintf(*ifjCodeName, "GF@%s", idName);
    else
        sprintf(*ifjCodeName, "LF@%s", idName);
    //test if it's ok with definitions
    symbolTable_t *ST = (actSymtable) ? localST : globalST;
    symbolTable_t *found = searchST(ST, idName);
    if(found && found->symInfo->definedInAssembly)
    {
        if(isDefined != NULL)
            *isDefined = true;
    }
    else if(found)
        found->symInfo->definedInAssembly = true;
    else
    {
        found = searchST(globalST, idName); //it might be already used as global ->error
        if(found && found->symInfo->globalUsedInLocal)
            return DEF_ERROR;
        insertSymbol(actSymtable, idName, 0, true, true, inLoop, false, 0, 0);
        //create it's ifjcode variable
        if(inLoop)
            sprintf(*ifjCodeName, "TF@%s", idName);
    }
    return 0;
}

int printInstruction(int instruction, char *op1, char *op2, char *op3){

    //instructions without operands
    if(op1==NULL && op2==NULL && op3==NULL){
        switch (instruction){
        case CREATEFRAME_I:
            printf("CREATEFRAME\n");
            break;
        case PUSHFRAME_I:
            printf("PUSHFRAME\n");
            break;
        case POPFRAME_I:
            printf("POPFRAME\n");
            break;
        case RETURN_I:
            printf("RETURN\n");
            break;
        case CLEARS_I:
            printf("CLEARS\n");
            break;
        case ADDS_I:
            printf("ADDS\n");
            break;
        case SUBS_I:
            printf("SUBS\n");
            break;
        case MULS_I:
            printf("MULS\n");
            break;
        case DIVS_I:
            printf("DIVS\n");
            break;
        case IDIVS_I:
            printf("IDIVS\n");
            break;
        case LTS_I:
            printf("LTS\n");
            break;
        case GTS_I:
            printf("GTS\n");
            break;
        case EQS_I:
            printf("EQS\n");
            break;
        case ANDS_I:
            printf("ANDS\n");
            break;
        case ORS_I:
            printf("ORS\n");
            break;
        case NOTS_I:
            printf("NOTS\n");
            break;
        case INT2FLOATS_I:
            printf("INT2FLOATS\n");
            break;
        case FLOAT2INTS_I:
            printf("FLOAT2INTS\n");
            break;
        case INT2CHARS_I:
            printf("INT2CHARS\n");
            break;
        case STRI2INTS_I:
            printf("STRI2INTS\n");
            break;
        case BREAK_I:
            printf("BREAK\n");
            break;
        default:
            //Non existing instruction
            return INT_ERROR;
        }
    }
    //instructions with one operand
    else if(op2==NULL && op3==NULL){
        switch(instruction){
            case DEFVAR_I:
                printf("DEFVAR %s\n", op1);
                break;
            case CALL_I:
                printf("CALL %s\n", op1);
                break;
            case PUSHS_I:
                printf("PUSHS %s\n", op1);
                break;
            case POPS_I:
                printf("POPS %s\n", op1);
                break;
            case WRITE_I:
                printf("WRITE %s\n", op1);
                break;
            case LABEL_I:
                printf("LABEL %s\n", op1);
                break;
            case JUMP_I:
                printf("JUMP %s\n", op1);
                break;
            case EXIT_I:
                printf("EXIT %s\n", op1);
                break;
            case DPRINT_I:
                printf("DPRINT %s\n", op1);
                break;
            case JUMPIFEQS_I:
                printf("JUMPIFEQS %s\n", op1);
                break;
            case JUMPIFNEQS_I:
                printf("JUMPIFNEQS %s\n", op1);
                break;
            default:
                //Non existing instruction
                return INT_ERROR;
        }
    }
    //instructions with two operands
    else if(op3==NULL){
        switch(instruction){
            case MOVE_I:
                printf("MOVE %s %s\n", op1, op2);
                break;
            case INT2FLOAT_I:
                printf("INT2FLOAT %s %s\n", op1, op2);
                break;
            case FLOAT2INT_I:
                printf("FLOAT2INT %s %s\n", op1, op2);
                break;
            case INT2CHAR_I:
                printf("INT2CHAR %s %s\n", op1, op2);
                break;
            case READ_I:
                printf("READ %s %s\n", op1, op2);
                break;
            case STRLEN_I:
                printf("STRLEN %s %s\n", op1, op2);
                break;
            case TYPE_I:
                printf("TYPE %s %s\n", op1, op2);
                break;
            default:
                //Non existing instruction
                return INT_ERROR;
        }
    }
    //instructions with all three operands
    else{
        switch(instruction){
            case ADD_I:
                printf("ADD %s %s %s\n", op1, op2, op3);
                break;
            case SUB_I:
                printf("SUB %s %s %s\n", op1, op2, op3);
                break;
            case MUL_I:
                printf("MUL %s %s %s\n", op1, op2, op3);
                break;
            case DIV_I:
                printf("DIV %s %s %s\n", op1, op2, op3);
                break;
            case IDIV_I:
                printf("IDIV %s %s %s\n", op1, op2, op3);
                break;
            case LT_I:
                printf("LT %s %s %s\n", op1, op2, op3);
                break;
            case GT_I:
                printf("GT %s %s %s\n", op1, op2, op3);
                break;
            case EQ_I:
                printf("EQ %s %s %s\n", op1, op2, op3);
                break;
            case AND_I:
                printf("AND %s %s %s\n", op1, op2, op3);
                break;
            case OR_I:
                printf("OR %s %s %s\n", op1, op2, op3);
                break;
            case NOT_I:
                printf("NOT %s %s %s\n", op1, op2, op3);
                break;
            case STRI2INT_I:
                printf("STRI2INT %s %s %s\n", op1, op2, op3);
                break;
            case CONCAT_I:
                printf("CONCAT %s %s %s\n", op1, op2, op3);
                break;
            case GETCHAR_I:
                printf("GETCHAR %s %s %s\n", op1, op2, op3);
                break;
            case SETCHAR_I:
                printf("SETCHAR %s %s %s\n", op1, op2, op3);
                break;
            case JUMPIFEQ_I:
                printf("JUMPIFEQ %s %s %s\n", op1, op2, op3);
                break;
            case JUMPIFNEQ_I:
                printf("JUMPIFNEQ %s %s %s\n", op1, op2, op3);
                break;
            default:
                //Non existing instruction
                return INT_ERROR;
        }
    }
    return 0;
}

char* getLabel(){
    char* labelName = malloc(sizeof(char)*20);
    strcpy(labelName, "label");
    char numberString[20];
    sprintf(numberString, "%d", currentLabelNumber);
    strcat(labelName, numberString);
    currentLabelNumber++;
    return labelName;
}

char* getVariableName(int location){
    char* varName = malloc(sizeof(char)*50);
    switch(location){
        case 1:
            strcpy(varName, "GF@");
            break;
        case 2:
            strcpy(varName, "TF@");
            break;
        default:
            strcpy(varName, "LF@");
    }
    char numberString[20];
    sprintf(numberString, "%%%d", currentVariableNumber);
    strcat(varName, numberString);
    currentVariableNumber++;
    return varName;
}


String *generatorString(char *str)
{
    if(!str) return NULL;

    String *s = initString();
    appendString(s, "string@");
    int i = 0;
    while(str[i] != '\0')
    {
        char c[2];
        c[0] = str[i++];
        c[1] = '\0';
        switch(c[0])
        {
            case ' ': appendString(s, "\\032"); break;
            case '\\': appendString(s, "\\092"); break;
            case '\n': appendString(s, "\\010"); break;
            case '\t': appendString(s, "\\009"); break;
            case '#': appendString(s, "\\035"); break;
            default: appendString(s, c); break;
        }
    }
    return s;
}

void printExpression(List *postfix){
    if(!postfix->first){
        //If list is empty, that means there is no return value, so just push nil@nil
        printInstruction(PUSHS_I, "nil@nil", NULL, NULL);
    }
    
    for(postfix->current = postfix->first; postfix->current != NULL; postfix->current = postfix->current->next){    
        switch(postfix->current->value->type){
            //ARITHMETIC OPERATORS
            case PLUS_OP_T: {
                checkNone();
                char * operand1 = getVariableName(2);
                char * operand2 = getVariableName(2);
                char * type = getVariableName(2);
                char * result = getVariableName(2);
                char * concatLabel = getLabel();
                char * endLabel = getLabel();
                if(!inLoop)
                   printInstruction(CREATEFRAME_I, NULL, NULL, NULL);
                printInstruction(DEFVAR_I, operand2, NULL, NULL);
                printInstruction(DEFVAR_I, operand1, NULL, NULL);
                printInstruction(DEFVAR_I, type, NULL, NULL);
                printInstruction(DEFVAR_I, result, NULL, NULL);
                printInstruction(POPS_I, operand1, NULL, NULL);
                printInstruction(POPS_I, operand2, NULL, NULL);
                printInstruction(TYPE_I, type, operand1, NULL);
                printInstruction(JUMPIFEQ_I, concatLabel, type, "string@string");
                printInstruction(PUSHS_I, operand1, NULL, NULL);
                printInstruction(PUSHS_I, operand2, NULL, NULL);
                if(!inLoop)
                   printInstruction(PUSHFRAME_I, NULL, NULL, NULL);
                ints2Floats();
                if(!inLoop)
                   printInstruction(POPFRAME_I, NULL, NULL, NULL);
                printInstruction(POPS_I, operand1, NULL, NULL);
                printInstruction(POPS_I, operand2, NULL, NULL);
                printInstruction(ADD_I, result, operand1, operand2);
                printInstruction(PUSHS_I, result, NULL, NULL);
                printInstruction(JUMP_I, endLabel, NULL, NULL);
                printInstruction(LABEL_I, concatLabel, NULL, NULL);
                printInstruction(CONCAT_I, result, operand2, operand1);
                printInstruction(PUSHS_I, result, NULL, NULL);
                printInstruction(LABEL_I, endLabel, NULL, NULL);
                free(operand1);
                free(operand2);
                free(type);
                free(concatLabel);
                free(endLabel);
                free(result);
                break;
            }
            case MINUS_OP_T:{
                checkNone();
                ints2Floats();
                printInstruction(SUBS_I, NULL, NULL, NULL);
                break;
            }
            case MUL_OP_T:{
                checkNone();
                ints2Floats();
                printInstruction(MULS_I, NULL, NULL, NULL);
                break;
            }
            case DIV_OP_T:{
                checkNone();
                char * zeroDivisionLabel = getLabel();
                char * continueLabel = getLabel();
                char * operand1 = getVariableName(2);
                char * operand2 = getVariableName(2);

                //Convert both operands to floats
                if(!inLoop)
                    printInstruction(CREATEFRAME_I, NULL, NULL, NULL);
    
                //Pop values from stack and save them in temp. frame
                char * val1 = getVariableName(2);
                char * val2 = getVariableName(2);
                printInstruction(DEFVAR_I, val1, NULL, NULL);
                printInstruction(DEFVAR_I, val2, NULL, NULL);
                printInstruction(POPS_I, val2, NULL, NULL);
                printInstruction(POPS_I, val1, NULL, NULL);
                
                //Get types of those values
                char * type1 = getVariableName(2);
                char * type2 = getVariableName(2);
                printInstruction(DEFVAR_I, type1, NULL, NULL);
                printInstruction(DEFVAR_I, type2, NULL, NULL);
                printInstruction(TYPE_I, type1, val1, NULL);
                printInstruction(TYPE_I, type2, val2, NULL);
                
                //Create variables to store results of conversion
                char * res1 = getVariableName(2);
                char * res2 = getVariableName(2);
                printInstruction(DEFVAR_I, res1, NULL, NULL);
                printInstruction(DEFVAR_I, res2, NULL, NULL);
                
                //Now check first operand if it is int
                char * firstNotIntLabel = getLabel();
                char * afterFirstLabel = getLabel();
                printInstruction(JUMPIFNEQ_I, firstNotIntLabel, type1, "string@int");
                //Convert int to float
                printInstruction(INT2FLOAT_I, res1, val1, NULL);
                printInstruction(JUMP_I, afterFirstLabel, NULL, NULL);
                //Do not convert, just move value from val to res
                printInstruction(LABEL_I, firstNotIntLabel, NULL, NULL);
                printInstruction(MOVE_I, res1, val1, NULL);
                //Continue
                printInstruction(LABEL_I, afterFirstLabel, NULL, NULL);
                
                //Now check second operand if it is int
                char * secondNotIntLabel = getLabel();
                char * afterSecondLabel = getLabel();
                printInstruction(JUMPIFNEQ_I, secondNotIntLabel, type2, "string@int");
                //Convert int to float
                printInstruction(INT2FLOAT_I, res2, val2, NULL);
                printInstruction(JUMP_I, afterSecondLabel, NULL, NULL);
                //Do not convert, just move value from val to res
                printInstruction(LABEL_I, secondNotIntLabel, NULL, NULL);
                printInstruction(MOVE_I, res2, val2, NULL);
                //Continue
                printInstruction(LABEL_I, afterSecondLabel, NULL, NULL);

                //Push converted values back to stack
                printInstruction(PUSHS_I, res1, NULL, NULL);
                printInstruction(PUSHS_I, res2, NULL, NULL);
                
                //// DIVISION ITSELF ////
                
                if(!inLoop)
                    printInstruction(CREATEFRAME_I, NULL, NULL, NULL);
                printInstruction(DEFVAR_I, operand1, NULL, NULL);
                printInstruction(DEFVAR_I, operand2, NULL, NULL);
                printInstruction(MOVE_I, operand1, "float@0x0.0p+0", NULL);
                printInstruction(POPS_I, operand2, NULL, NULL);
                printInstruction(JUMPIFEQ_I, zeroDivisionLabel, operand1, operand2);
                printInstruction(POPS_I, operand1, NULL, NULL);
                printInstruction(PUSHS_I, operand1, NULL, NULL);
                printInstruction(PUSHS_I, operand2, NULL, NULL);
                printInstruction(DIVS_I, NULL, NULL, NULL);
                printInstruction(JUMP_I, continueLabel, NULL, NULL);
                printInstruction(LABEL_I, zeroDivisionLabel, NULL, NULL);
                printInstruction(EXIT_I, "int@9", NULL, NULL);
                printInstruction(LABEL_I, continueLabel, NULL, NULL);
                
                free(val1);
                free(val2);
                free(type1);
                free(type2);
                free(res1);
                free(res2);
                free(firstNotIntLabel);
                free(afterFirstLabel);
                free(secondNotIntLabel);
                free(afterSecondLabel);
                free(zeroDivisionLabel);
                free(continueLabel);
                free(operand1);
                free(operand2);
                break;
            }
            case INT_DIV_OP_T:{
                checkNone();
                char * zeroDivisionLabel = getLabel();
                char * continueLabel = getLabel();
                char * operand1 = getVariableName(2);
                char * operand2 = getVariableName(2);
                if(!inLoop)
                    printInstruction(CREATEFRAME_I, NULL, NULL, NULL);
                printInstruction(DEFVAR_I, operand1, NULL, NULL);
                printInstruction(DEFVAR_I, operand2, NULL, NULL);
                printInstruction(MOVE_I, operand1, "int@0", NULL);
                printInstruction(POPS_I, operand2, NULL, NULL);
                printInstruction(JUMPIFEQ_I, zeroDivisionLabel, operand1, operand2);
                printInstruction(PUSHS_I, operand2, NULL, NULL);
                printInstruction(IDIVS_I, NULL, NULL, NULL);
                printInstruction(JUMP_I, continueLabel, NULL, NULL);
                printInstruction(LABEL_I, zeroDivisionLabel, NULL, NULL);
                printInstruction(EXIT_I, "int@9", NULL, NULL);
                printInstruction(LABEL_I, continueLabel, NULL, NULL);
                
                free(zeroDivisionLabel);
                free(continueLabel);
                free(operand1);
                free(operand2);
                break;
            }
            //RELATION OPERATORS
            case EQUAL_T:{
                ints2Floats();
                printInstruction(EQS_I, NULL, NULL, NULL);
                break;
            }
            case NOT_EQUAL_T:{
                ints2Floats();
                printInstruction(EQS_I, NULL, NULL, NULL);
                printInstruction(NOTS_I, NULL, NULL, NULL);
                break;
            }
            case GREATER_EQUAL_T:{
                ints2Floats();

                char * operand1 = getVariableName(2);
                char * operand2 = getVariableName(2);
                char * result1 = getVariableName(2);
                char * result2 = getVariableName(2);
                char * resultFin = getVariableName(2);
                
                if(!inLoop)
                    printInstruction(CREATEFRAME_I, NULL, NULL, NULL);
                printInstruction(DEFVAR_I, operand1, NULL, NULL);
                printInstruction(DEFVAR_I, operand2, NULL, NULL);
                printInstruction(DEFVAR_I, result1, NULL, NULL);
                printInstruction(DEFVAR_I, result2, NULL, NULL);
                printInstruction(DEFVAR_I, resultFin, NULL, NULL);
                printInstruction(POPS_I, operand2, NULL, NULL);
                printInstruction(POPS_I, operand1, NULL, NULL);
                printInstruction(GT_I, result1, operand1, operand2);
                printInstruction(EQ_I, result2, operand1, operand2);
                printInstruction(OR_I, resultFin, result1, result2);
                printInstruction(PUSHS_I, resultFin, NULL, NULL);
                free(operand1);
                free(operand2);
                free(result1);
                free(result2);
                free(resultFin);
                break;
            }
            case LESS_EQUAL_T:{
                ints2Floats();

                char * operand1 = getVariableName(2);
                char * operand2 = getVariableName(2);
                char * result1 = getVariableName(2);
                char * result2 = getVariableName(2);
                char * resultFin = getVariableName(2);

                if(!inLoop)
                    printInstruction(CREATEFRAME_I, NULL, NULL, NULL);
                printInstruction(DEFVAR_I, operand1, NULL, NULL);
                printInstruction(DEFVAR_I, operand2, NULL, NULL);
                printInstruction(DEFVAR_I, result1, NULL, NULL);
                printInstruction(DEFVAR_I, result2, NULL, NULL);
                printInstruction(DEFVAR_I, resultFin, NULL, NULL);
                printInstruction(POPS_I, operand2, NULL, NULL);
                printInstruction(POPS_I, operand1, NULL, NULL);
                printInstruction(LT_I, result1, operand1, operand2);
                printInstruction(EQ_I, result2, operand1, operand2);
                printInstruction(OR_I, resultFin, result1, result2);
                printInstruction(PUSHS_I, resultFin, NULL, NULL);
                free(operand1);
                free(operand2);
                free(result1);
                free(result2);
                free(resultFin);
                break;
            }
            case GREATER_THAN_T:{
                ints2Floats();
                printInstruction(GTS_I, NULL, NULL, NULL);
                break;
            }
            case LESS_THAN_T:{
                ints2Floats();
                printInstruction(LTS_I, NULL, NULL, NULL);
                break;
            }
            //OPERANDS
            default:;
                char stringVal[150];
                switch(postfix->current->value->type){
                    case INT_T:
                        sprintf(stringVal, "int@%d", postfix->current->value->intVal);
                        printInstruction(PUSHS_I, stringVal, NULL, NULL);
                        break;
                    case FLOAT_T:
                        sprintf(stringVal, "float@%a", postfix->current->value->floatVal);
                        printInstruction(PUSHS_I, stringVal, NULL, NULL);
                        break;
                    case STRING_T:;
                        String *str = generatorString(postfix->current->value->stringVal->string);
                        sprintf(stringVal, "%s", str->string);
                        printInstruction(PUSHS_I, stringVal, NULL, NULL);
                        delString(str);
                        break;
                    case NONE_T:
                        printInstruction(PUSHS_I, "nil@nil", NULL, NULL);
                        break;
                    default:;
                        char *name = postfix->current->value->stringVal->string;
                        symbolTable_t *ST = actSymtable ? localST : globalST;
                        symbolTable_t *found = searchST(ST, name);
                        if(found && found->symInfo->definedInLoop)
                            sprintf(stringVal, "TF@%s", name);
                        else if(found && actSymtable==LOCAL)
                            sprintf(stringVal, "LF@%s", name);
                        else
                        {
                            //if I use global var in a local frame and this global is not shadowed by a local, make it used in local frame
                            if(actSymtable==LOCAL)
                            {
                                symbolTable_t *foundGlobal = searchST(globalST, name);
                                if(foundGlobal)
                                    foundGlobal->symInfo->globalUsedInLocal = true;
                            }
                            sprintf(stringVal, "GF@%s", name);
                        }
                        printInstruction(PUSHS_I, stringVal, NULL, NULL);
                        break;
                }
                break;
        }
    }
}

void checkNone(){
    //Check when doing arithmetic operation, that none of the operands is None
    if(!inLoop)
        printInstruction(CREATEFRAME_I, NULL, NULL, NULL);
    
    //Pop values from stack and save them in temp. frame
    char * val1 = getVariableName(2);
    char * val2 = getVariableName(2);
    printInstruction(DEFVAR_I, val1, NULL, NULL);
    printInstruction(DEFVAR_I, val2, NULL, NULL);
    printInstruction(POPS_I, val2, NULL, NULL);
    printInstruction(POPS_I, val1, NULL, NULL);
    
    //Get types of those values
    char * type1 = getVariableName(2);
    char * type2 = getVariableName(2);
    printInstruction(DEFVAR_I, type1, NULL, NULL);
    printInstruction(DEFVAR_I, type2, NULL, NULL);
    printInstruction(TYPE_I, type1, val1, NULL);
    printInstruction(TYPE_I, type2, val2, NULL);
    
    char * errLabel = getLabel();
    char * continueLabel = getLabel();
    
    printInstruction(JUMPIFEQ_I, errLabel, type1, "string@nil");
    printInstruction(JUMPIFEQ_I, errLabel, type2, "string@nil");
    printInstruction(JUMP_I, continueLabel, NULL, NULL);
    //Arithmetic operation with None -> error
    printInstruction(LABEL_I, errLabel, NULL, NULL);
    printInstruction(EXIT_I, "int@4", NULL, NULL);
    
    printInstruction(LABEL_I, continueLabel, NULL, NULL);
    printInstruction(PUSHS_I, val1, NULL, NULL);
    printInstruction(PUSHS_I, val2, NULL, NULL);

    free(val1);
    free(val2);
    free(type1);
    free(type2);
    free(errLabel);
    free(continueLabel);
}

void ints2Floats(){
    //Two values (that we care about) are already on stack, now lets check them out
    
    //Create new frame and save the current one
    //~ printInstruction(PUSHFRAME_I, NULL, NULL, NULL);
    if(!inLoop)
        printInstruction(CREATEFRAME_I, NULL, NULL, NULL);
    
    //Pop values from stack and save them in temp. frame
    char * val1 = getVariableName(2);
    char * val2 = getVariableName(2);
    printInstruction(DEFVAR_I, val1, NULL, NULL);
    printInstruction(DEFVAR_I, val2, NULL, NULL);
    printInstruction(POPS_I, val2, NULL, NULL);
    printInstruction(POPS_I, val1, NULL, NULL);
    
    //Get types of those values
    char * type1 = getVariableName(2);
    char * type2 = getVariableName(2);
    printInstruction(DEFVAR_I, type1, NULL, NULL);
    printInstruction(DEFVAR_I, type2, NULL, NULL);
    printInstruction(TYPE_I, type1, val1, NULL);
    printInstruction(TYPE_I, type2, val2, NULL);
    
    //Create variables to store results of conversion
    char * res1 = getVariableName(2);
    char * res2 = getVariableName(2);
    printInstruction(DEFVAR_I, res1, NULL, NULL);
    printInstruction(DEFVAR_I, res2, NULL, NULL);

    
    //First compare, if those two are the same type - if yes, then no type converting is needed
    char * noConversionLabel = getLabel();
    char * continueLabel = getLabel(); //Have to move vals to res to continue
    printInstruction(JUMPIFEQ_I, noConversionLabel, type1, type2);
    //Or do not convert if one of them is None - type nil@nil
    printInstruction(JUMPIFEQ_I, noConversionLabel, type1, "string@nil");
    printInstruction(JUMPIFEQ_I, noConversionLabel, type2, "string@nil");
    
    //Now check first operand if it is int
    char * firstNotIntLabel = getLabel();
    char * afterFirstLabel = getLabel();
    printInstruction(JUMPIFNEQ_I, firstNotIntLabel, type1, "string@int");
    //Convert int to float
    printInstruction(INT2FLOAT_I, res1, val1, NULL);
    printInstruction(JUMP_I, afterFirstLabel, NULL, NULL);
    //Do not convert, just move value from val to res
    printInstruction(LABEL_I, firstNotIntLabel, NULL, NULL);
    printInstruction(MOVE_I, res1, val1, NULL);
    //Continue
    printInstruction(LABEL_I, afterFirstLabel, NULL, NULL);
    
    //Now check second operand if it is int
    char * secondNotIntLabel = getLabel();
    char * afterSecondLabel = getLabel();
    printInstruction(JUMPIFNEQ_I, secondNotIntLabel, type2, "string@int");
    //Convert int to float
    printInstruction(INT2FLOAT_I, res2, val2, NULL);
    printInstruction(JUMP_I, afterSecondLabel, NULL, NULL);
    //Do not convert, just move value from val to res
    printInstruction(LABEL_I, secondNotIntLabel, NULL, NULL);
    printInstruction(MOVE_I, res2, val2, NULL);
    //Continue
    printInstruction(LABEL_I, afterSecondLabel, NULL, NULL);
    printInstruction(JUMP_I, continueLabel, NULL, NULL);
    
    printInstruction(LABEL_I, noConversionLabel, NULL, NULL);
    printInstruction(MOVE_I, res1, val1, NULL);
    printInstruction(MOVE_I, res2, val2, NULL);
    
    //Push OK or converted values back to stack
    printInstruction(LABEL_I, continueLabel, NULL, NULL);
    printInstruction(PUSHS_I, res1, NULL, NULL);
    printInstruction(PUSHS_I, res2, NULL, NULL);
    
    free(val1);
    free(val2);
    free(type1);
    free(type2);
    free(res1);
    free(res2);
    free(noConversionLabel);
    free(continueLabel);
    free(firstNotIntLabel);
    free(afterFirstLabel);
    free(secondNotIntLabel);
    free(afterSecondLabel);
}

//gets two ints or two floats on top of stack
//~ void getComparableStack(){
                //~ char * operand1 = getVariableName(2);
                //~ char * operand2 = getVariableName(2);
                //~ char * type1 = getVariableName(2);
                //~ char * type2 = getVariableName(2);
                //~ char * result = getVariableName(2);
                //~ char * result1 = getVariableName(2);
                //~ char * result2 = getVariableName(2);
                //~ char * continueLabel = getLabel();
                //~ char * convertFirstLabel = getLabel();
                //~ char * convertSecondLabel = getLabel();
                //~ char * defaultLabel = getLabel();
                //~ printInstruction(CREATEFRAME_I, NULL, NULL, NULL);
                //~ printInstruction(DEFVAR_I, operand1, NULL, NULL);
                //~ printInstruction(DEFVAR_I, operand2, NULL, NULL);
                //~ printInstruction(DEFVAR_I, type1, NULL, NULL);
                //~ printInstruction(DEFVAR_I, type2, NULL, NULL);
                //~ printInstruction(DEFVAR_I, result, NULL, NULL);
                //~ printInstruction(DEFVAR_I, result1, NULL, NULL);
                //~ printInstruction(DEFVAR_I, result2, NULL, NULL);
                //~ printInstruction(POPS_I, operand2, NULL, NULL);
                //~ printInstruction(POPS_I, operand1, NULL, NULL);
                //~ printInstruction(TYPE_I, type1, operand1, NULL);
                //~ printInstruction(TYPE_I, type2, operand2, NULL);
                //~ //compare operands
                //~ printInstruction(JUMPIFEQ_I, defaultLabel, type1, type2);
                //~ printInstruction(JUMPIFEQ_I, defaultLabel, type1, "string@string");
                //~ printInstruction(JUMPIFEQ_I, defaultLabel, type2, "string@string");
                //~ printInstruction(JUMPIFEQ_I, convertFirstLabel, type2, "string@float");
                //~ printInstruction(JUMPIFEQ_I, convertSecondLabel, type1, "string@float");
                //~ //convert first operand
                //~ printInstruction(LABEL_I, convertFirstLabel, NULL, NULL);
                //~ printInstruction(FLOAT2INT_I, result1, operand1, NULL);
                //~ printInstruction(MOVE_I, result2, operand2, NULL);
                //~ printInstruction(JUMP_I, continueLabel, NULL, NULL);
                //~ //convert second operand
                //~ printInstruction(LABEL_I, convertSecondLabel, NULL, NULL);
                //~ printInstruction(FLOAT2INT_I, result2, operand2, NULL);
                //~ printInstruction(MOVE_I, result1, operand1, NULL);
                //~ printInstruction(JUMP_I, continueLabel, NULL, NULL);
                //~ //types were the same or are string
                //~ printInstruction(LABEL_I, defaultLabel, NULL, NULL);
                //~ printInstruction(MOVE_I, result1, operand1, NULL);
                //~ printInstruction(MOVE_I, result2, operand2, NULL);
                //~ //push results back to stack
                //~ printInstruction(LABEL_I, continueLabel, NULL, NULL);
                //~ printInstruction(PUSHS_I, result1, NULL, NULL);
                //~ printInstruction(PUSHS_I, result2, NULL, NULL);
                //~ free(operand1);
                //~ free(operand2);
                //~ free(type1);
                //~ free(type2);
                //~ free(result);
                //~ free(result1);
                //~ free(result2);
                //~ free(continueLabel);
                //~ free(convertFirstLabel);
                //~ free(convertSecondLabel);
                //~ free(defaultLabel);
//~ }
