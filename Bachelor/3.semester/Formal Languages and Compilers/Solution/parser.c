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
 * @file parser.c
 * @brief Source file for the parser, which covers the functionality of the syntactic analysis
 *
 */


int err;        //will be used to hold return values

#include "parser.h"
#define loadtoken if(token!=NULL) freeToken(token); token = loadToken(); if(token->type==LEXICAL_ERROR_T){return LEX_ERROR;} if(token==NULL){return INT_ERROR;}
bool actSymtable=GLOBAL;   //when in function actSymtable will be LOCAL otherwise it will be GLOBAL
Token_t * token = NULL;
Token_t * tokenHolder = NULL;
symbolTable_t * found=NULL;
char *funcRetVal = NULL;
bool inLoop = false;
extern Stack indentStack;
extern char *skipLabel;
extern char *skipLabel;

int analyse(){
    globalST = initST();
    printStart();
    loadtoken   //loads first token

    err = prog();

    freeToken(token);
    delST(globalST);
    if(funcRetVal) free(funcRetVal);
    free((int *)top(&indentStack)->data);
    delStack(&indentStack);
    if(err!=0)
        return err;
    return 0;
}

int prog(){
    switch (token->type)
    {
    case ID_T:
    case INT_T:
    case FLOAT_T:
    case STRING_T:
    case PASS_T:
    case WHILE_T:
    case IF_T:
    case INPUTS_T:
    case INPUTI_T:
    case INPUTF_T:
    case PRINT_T:
    case EOL_T:       //1	<prog>	->	<statement> <prog>
        err = statement();
        if(err!=0)
            return err;

        return prog();
    case EOF_T:         //2	<prog>	->	EOF
        if(checkUndefined(globalST) != 0)
            return DEF_ERROR;
        return 0;
    case DEF_T:          //3	<prog>	->	def id<params> :  <eols> indent <statement> dedent <prog>
        actSymtable = LOCAL;    //we are now in function
        localST = initST();     //init symtable

        loadtoken
        if(token->type!=ID_T){
            delST(localST);
            return SYN_ERROR;
        }
        char * funcName= malloc((strlen(token->stringVal->string)+1)*sizeof(char));
        if(funcName==NULL){
            delST(localST);
            return INT_ERROR;
        }
        strcpy(funcName,token->stringVal->string);     //save string val for creating the function

        loadtoken
        err = params(funcName, true, NULL);             //<params>
        if(err!=0){
            free(funcName);
            delST(localST);
            return err;
        }

        free(funcName);

        if(token->type!=COLON_T){
            delST(localST);
            return SYN_ERROR;
        }

        loadtoken        //EOL

        err = eols();     //<eols>

        if(err!=0){
            delST(localST);
            return err;
        }


        err = statement();          //<statement>
        if(err!=0){
            delST(localST);
            return err;
        }
        while(token->type == EOL_T){

            loadtoken
        }

        if(token->type!=DEDENT_T){
            delST(localST);
            return SYN_ERROR;
        }
        //this is the point we got out of function definition, generate end of definition IFJcode19
        printInstruction(POPFRAME_I, NULL, NULL, NULL);
        printInstruction(RETURN_I, NULL, NULL, NULL);
        printInstruction(LABEL_I, skipLabel, NULL, NULL);
        free(skipLabel);
        //alse need to clear the globalInLocalUsed, so that contexts wouldn't cross each other
        detachGlobalUsedInLocal(globalST);
        delST(localST);             //delete symtable
        actSymtable = GLOBAL;

        loadtoken

        return prog();              //<prog>

    default:
        return SYN_ERROR;
    }
}

int params(char *funcName, bool definition, char *assignId)
{

    switch (token->type)
    {
    case LEFT_BRACKET_T:    //4	<params>	->	(<param>
        loadtoken
        int paramCount=0;
        List *paramList = initList();
        err = param(paramList, &paramCount, definition);
        if(err!=0)
        {
            delList(paramList);
            return err;
        }
        //checking builtin - those have their own semantic check as they are not in symtable
        for(int i = 0; i < NUM_OF_BUILTINS; i++)
            if(!strcmp(funcName, builtins[i]))
                goto callFunc;
        found = NULL;
        if(definition){

            found = searchST(globalST, funcName);      //look for func in GT
            if(found != NULL){              //func is already in GT
                if(found->symInfo->defined==true){       //if found defined function then we're trying to redifine function which means DEF_ERROR

                    delList(paramList);
                    return DEF_ERROR;
                }
                else if(found->symInfo->defined==false && found->symInfo->parameters == paramCount){  //if found undefined function with right amount of args then we'll define the function

                    found->symInfo->defined = true;
                }
                else {     //if the undifined function has wrong amount of parameteres return param error

                    delList(paramList);
                    return PARAM_ERROR;
                }
            }

            if(found==NULL){
                //printf("Func name %s was not found. Iserting it into GT\n",funcName);
                err = insertSymbol(GLOBAL, funcName, FUNC, true, false, inLoop, false, paramCount, 0);


            }

            //semantics is ok, let's generate the function definition
            int errCode = printFunctionDefinition(funcName, paramList, paramCount);
            if(errCode) return errCode;
        }
        else{


            if(actSymtable==LOCAL){

                found = searchST(localST, funcName);
            }

            if(found==NULL){        //if something is found in LT it is already doomed to error

                found = searchST(globalST, funcName);     //search global symtable always
            }

            if(found==NULL){        //if function was not found, create new undefined symbol

                insertSymbol(GLOBAL, funcName, FUNC, false, false, inLoop, false, paramCount, 0);
            }       //trying to call undefined function
            else if(found->symInfo->symType==VAR){        //which will be if found in LT

                delList(paramList);
                return DEF_ERROR;    //id již má proměnná
            }
            else if(paramCount!=found->symInfo->parameters)
            {
                delList(paramList);
                return PARAM_ERROR;
            }

            //semantics successful, let's generate it, assignId will be null if there's no var to assign the function value
            callFunc:;
            int errCode = printFunctionCall(funcName, paramList, paramCount, assignId);
            free(funcRetVal);
            funcRetVal = NULL;
            if(errCode)
            {
                delList(paramList);
                return errCode;
            }
        }
        delList(paramList);
        return 0;
    default:
        return SYN_ERROR;
    }
}

int param(List *paramList, int *paramCount, bool definition){

    switch (token->type)
    {
    case ID_T:
    case STRING_T:
    case INT_T:
    case FLOAT_T:   //5	<param>	->	<item> <param>
        if(definition){

            if(token->type!=ID_T){      //def args can be only ID's

                return DEF_ERROR;
            }

            insertSymbol(LOCAL, token->stringVal->string, VAR, true, false, inLoop, false, 0, 0);     //parameters are just vars in local symtable type has
        }
        //have no idea what to do when !definition



        (*paramCount)++;
        appendList(paramList, deepCopyToken(token));


        err = item();    //<item>
        if(err!=0)
            return err;
        return param(paramList, paramCount, definition);  //<param>
    case COMA_T:    //6	<param>	->	,<item> <param>
        loadtoken;

        (*paramCount)++;
        appendList(paramList, deepCopyToken(token));


        if(definition==true){

            found = searchST(globalST,token->stringVal->string);
            //check if param is not globaly defined function or variable that has been used, so cannot be shadowed
            if(found!=NULL && (found->symInfo->symType==FUNC || found->symInfo->globalUsedInLocal)){

                return DEF_ERROR;
            }
            if(token->type!=ID_T){      //in definition params can only be ID's

                return DEF_ERROR;
            }

            insertSymbol(LOCAL, token->stringVal->string, VAR, true, false, inLoop, false, 0, 0);        //
        }

        err = item();    //<item>
        if(err!=0)
            return err;
        return param(paramList, paramCount, definition); //<param>
    case RIGHT_BRACKET_T: //7	<param>	->	)
        loadtoken
        return 0;
    default:
        return SYN_ERROR;
    }
}

int statement(){

    int resType = 0;
    switch (token->type)
    {
    case WHILE_T:;   //8	<statement>	->	while <expr> : <eols> indent <statement> dedent <statement>
        char * labelName1 = getLabel();
        char * labelName2 = getLabel();

        inLoop = true;
        printInstruction(LABEL_I, labelName1, NULL, NULL);
        printInstruction(CREATEFRAME_I, NULL, NULL, NULL);

        loadtoken;
        err = expr(&resType);
        if(err!=0){
            free(labelName1);
            free(labelName2);
            return err;
        }

        char * returnVar2 = getVariableName(2);
        printInstruction(DEFVAR_I, "TF@%retVal", NULL, NULL); //for any function in it
        printInstruction(DEFVAR_I, returnVar2, NULL, NULL);
        printInstruction(POPS_I, returnVar2, NULL, NULL);
        printInstruction(JUMPIFEQ_I, labelName2, "bool@false", returnVar2);


        if(token->type!=COLON_T){
            free(labelName1);
            free(labelName2);
            free(returnVar2);
            return SYN_ERROR;
        }

        loadtoken;       //EOL

        err = eols();     //<eols>

        if(err!=0){
            free(labelName1);
            free(labelName2);
            free(returnVar2);
            return err;
        }

        err = statement();          //<statement>
        if(err!=0){
            free(labelName1);
            free(labelName2);
            free(returnVar2);
            return err;
        }

        printInstruction(JUMP_I, labelName1, NULL, NULL);
        printInstruction(LABEL_I, labelName2, NULL, NULL);
        inLoop = false;

        if(token->type!=DEDENT_T){
            free(labelName1);
            free(labelName2);
            free(returnVar2);
            return SYN_ERROR;
        }
        loadtoken;

        free(labelName1);
        free(labelName2);
        free(returnVar2);
        return statement();

    case IF_T:      //9	<statement>	->	if <expr>: <eols> INDENT  <statement> dedent else: EOL indent <statement> dedent <statement>
        loadtoken;
        err = expr(&resType);
        if(err!=0)
            return err;

        char * labelName3 = getLabel();
        char * labelName4 = getLabel();
        char * returnVar;
        if(inLoop)
            returnVar = getVariableName(2);
        else if(actSymtable==GLOBAL)
            returnVar = getVariableName(1);
        else
            returnVar = getVariableName(0);
        printInstruction(DEFVAR_I, returnVar, NULL, NULL);
        printInstruction(POPS_I, returnVar, NULL, NULL);
        printInstruction(JUMPIFEQ_I, labelName3, "bool@false", returnVar);


        if(token->type!=COLON_T){
            free(labelName3);
            free(labelName4);
            free(returnVar);
            return SYN_ERROR;
        }

        loadtoken;       //EOL

        err = eols();     //<eols>

        if(err!=0){
            free(labelName3);
            free(labelName4);
            free(returnVar);
            return err;
        }


        err = statement();          //<statement>
        if(err!=0){
            free(labelName3);
            free(labelName4);
            free(returnVar);
            return err;
        }


        if(token->type!=DEDENT_T){
            free(labelName3);
            free(labelName4);
            free(returnVar);
            return SYN_ERROR;
        }

        printInstruction(JUMP_I, labelName4, NULL, NULL);


        loadtoken;       //else

        if(token->type!=ELSE_T){
            free(labelName3);
            free(labelName4);
            free(returnVar);
            return SYN_ERROR;
        }


        printInstruction(LABEL_I, labelName3, NULL, NULL);

        loadtoken;       //:

        if(token->type!=COLON_T){
            free(labelName3);
            free(labelName4);
            free(returnVar);
            return SYN_ERROR;
        }

        loadtoken;       //EOL

        err = eols();     //<eols>

        if(err!=0){
            free(labelName3);
            free(labelName4);
            free(returnVar);
            return err;
        }


        err = statement();          //<statement>
        if(err!=0){
            free(labelName3);
            free(labelName4);
            return err;
        }
        if(token->type!=DEDENT_T){
            free(labelName3);
            free(labelName4);
            free(returnVar);
            return SYN_ERROR;
        }
        loadtoken;

        printInstruction(LABEL_I, labelName4, NULL, NULL);

        free(labelName3);
        free(labelName4);
        free(returnVar);
        return statement();         //statement

    case ID_T:;      //10	<statement>	->	id <assignOrFunc>  <statement>
        char * IdName =  malloc((strlen(token->stringVal->string)+1)*sizeof(char));
        if(!IdName) return INT_ERROR;

        strcpy(IdName, token->stringVal->string);
        tokenHolder = deepCopyToken(token);

        loadtoken

        err = assignOrFunc(IdName);
        free(IdName);
        if(err!=0)
            return err;
        return statement();

    case INPUTS_T:
    case INPUTI_T:
    case INPUTF_T:
    case PRINT_T:
    case LEN_T:
    case SUBSTR_T:
    case ORD_T:
    case CHR_T:;     //11	<statement>	->	<builtin> <params> <statement>
        char *builtinName =  malloc((strlen(token->stringVal->string)+1)*sizeof(char));
        if(!builtinName) return INT_ERROR;
        strcpy(builtinName, token->stringVal->string);

        err = builtIn();  //<builtin>
        if(err!=0)
            return err;

        err = params(builtinName, false, funcRetVal);   //<params>
        free(builtinName);
        if(err!=0)
            return err;
        return statement();
    //TODO: do we need generating code here?
    case RETURN_T:         //12	<statement>	->	return <expr> EOL <statement>
        if(actSymtable!=LOCAL){
            return SYN_ERROR;
        }
        loadtoken;
        err = expr(&resType);
        if(err!=0)
            return err;
        if(token->type!=EOL_T){
            return SYN_ERROR;
        }
        //here we return from function -> asign return value
        printInstruction(POPS_I, "LF@%retVal", NULL, NULL);
        printInstruction(POPFRAME_I, NULL, NULL, NULL);
        printInstruction(RETURN_I, NULL, NULL, NULL);
        loadtoken;
        return statement();

    //TODO: do we need generating code here?
    case PASS_T:            //13	<statement>	->	pass EOL <statement>
        loadtoken;
        return statement();     //<statement>

    case EOL_T:     //14	<statement>	->	EOL <statement>

        loadtoken;
        return statement(); //<statement>
    case STRING_T:
    case FLOAT_T:
    case INT_T:
        return expr(NULL);   // 31	<statement>	->	<expression>
    default:        //15	<statement>	->	ε

        return 0;
    }
}

int assignOrFunc(char * IdName){
    switch (token->type)
    {
    case ASSIGNMENT_T:  //16	<assignOrFunc>	->	= <expression> <statement>
        //Id name should be var
        if(actSymtable==LOCAL)      //if actSymtable==LOCAL search in local symtable first
            found = searchST(localST, IdName);
        if(found==NULL||actSymtable==GLOBAL)          //if symbol is not in local symtable or actsymtable is global (we're not in function) search in global symtable
            found = searchST(globalST, IdName);
        if(found!=NULL&&found->symInfo->symType==FUNC){          //if symbol is found but is function throw DEF_ERROR

            return DEF_ERROR;       //trying to use func as variable
        }

        loadtoken

        //retVal from function
        if(token->type == ID_T){
            symbolTable_t *func = searchST(globalST, token->stringVal->string);
            symbolTable_t *locVar = NULL;
            if(actSymtable==LOCAL)
                locVar = searchST(localST, token->stringVal->string);
            if(locVar == NULL && (func == NULL || func->symInfo->symType == FUNC)){ //if symbol with matching name is found
                if(funcRetVal){
                    free(funcRetVal);
                    funcRetVal = NULL;
                }
                funcRetVal = malloc(sizeof(char) * (strlen(IdName)+1));
                strcpy(funcRetVal, IdName);
                char *funcName = malloc(sizeof(char) * (strlen(token->stringVal->string)+1));
                strcpy(funcName, token->stringVal->string);
                loadtoken
                int errCode = params(funcName, false, IdName); //Call params and add IdName as a pointer to asign to
                free(funcName);
                return errCode;
            }
        }
        //retval from builtin
        int t = token->type;
        if(t == PRINT_T || t == INPUTI_T || t == INPUTS_T || t == INPUTF_T || t == SUBSTR_T || t == CHR_T || t == ORD_T || t == LEN_T){
            if(funcRetVal){
                free(funcRetVal);
                funcRetVal = NULL;
            }
            funcRetVal = malloc(sizeof(char) * (strlen(IdName)+1));
            strcpy(funcRetVal, IdName);
            char *funcName = malloc(sizeof(char) * (strlen(token->stringVal->string)+1));
            strcpy(funcName, token->stringVal->string);
            loadtoken
            int errCode = params(funcName, false, IdName); //Call params and add IdName as a pointer to asign to
            free(funcName);
            return errCode;
        }

        ////// token after assignment is either: /not ID/, /symbol doesn't exist/ or /symbol does exist and is a variable/
        int resType=0;
        err = expr(&resType);//expr
        if(err!=0)
            return err;

        //create variable name acording to local/global
        char * varName = malloc(strlen(IdName)+3*(sizeof(char))+1);
        if(inLoop)
            sprintf(varName, "TF@%s", IdName);
        else if(actSymtable==LOCAL)
            sprintf(varName, "LF@%s", IdName);
        else
            sprintf(varName, "GF@%s", IdName);
        if(found==NULL){        //if symbol is not found insert it into actSymtable

            symbolTable_t *foundInGlobal = NULL;
            if(actSymtable == LOCAL)
                foundInGlobal = searchST(GLOBAL, IdName);
            if(foundInGlobal && foundInGlobal->symInfo->globalUsedInLocal){
                free(varName);
                return DEF_ERROR;
            }
            printInstruction(DEFVAR_I, varName, NULL, NULL);
            printInstruction(POPS_I, varName, NULL, NULL);
            insertSymbol(actSymtable, IdName, VAR, true, true, inLoop, false, 0, resType);     //not sure if label should be different then the name and about the number of arguments

        }
        else
        {
            found->symInfo->varType=resType;
            if(!found->symInfo->definedInLoop) //if it wasnt defined in loop, it's not TF
            {
                if(actSymtable==LOCAL)
                {
                    //it might have been found only in global
                    symbolTable_t *foundInLocal = searchST(localST, IdName);
                    if(!foundInLocal && found->symInfo->globalUsedInLocal){ //if it's been used as global in the same scope
                        free(varName);
                        return DEF_ERROR;                                  //definition error
                    }
                    if(!foundInLocal) //it could have been foun in global, so we might still need to define it
                    {
                        sprintf(varName, "LF@%s", IdName);
                        printInstruction(DEFVAR_I, varName, NULL, NULL);
                        insertSymbol(actSymtable, IdName, VAR, true, true, inLoop, false, 0, resType);
                    }
                    if(foundInLocal && !foundInLocal->symInfo->definedInLoop) //or it was found, so we rewrite it in case of being in a loop
                        sprintf(varName, "LF@%s", IdName);
                }
                else
                    sprintf(varName, "GF@%s", IdName);
            }

            //need to check if it's not first use of a local var which isnt defined
            printInstruction(POPS_I, varName, NULL, NULL);
        }
        free(varName);

        return 0;
     case LEFT_BRACKET_T:;    //17	<assignOrFunc>	->	<params>
        int errCode = params(IdName, false, funcRetVal);
        if(funcRetVal)
        {
            free(funcRetVal);
            funcRetVal = NULL;
        }
        return errCode;    //<params>
    case EQUAL_T:
    case NOT_EQUAL_T:
    case GREATER_EQUAL_T:
    case LESS_EQUAL_T:
    case GREATER_THAN_T:
    case LESS_THAN_T:
    case PLUS_OP_T:
    case MINUS_OP_T:
    case MUL_OP_T:
    case DIV_OP_T:
    case INT_DIV_OP_T:          //rule 32 for dead code <assignOrFunc>	->	<expression>
        return expr(NULL);      //dead code
    case EOL_T:                 //18	<assignOrFunc>	->	EOL
        loadtoken
        return statement();
    default:
        return SYN_ERROR;
    }
}

int item(){

    switch (token->type)
    {
    case ID_T:      //19	<item>	->	id

        loadtoken;
        return 0;
    case STRING_T:  //20	<item>	->	string

        loadtoken;
        return 0;
    case INT_T:     //21	<item>	->	int
        loadtoken;
        return 0;
    case FLOAT_T:   //22	<item>	->	float
        loadtoken;
        return 0;
    default:
        return SYN_ERROR;
    }
}

int builtIn(){
    switch (token->type)
    {
        case INPUTS_T:
        case INPUTI_T:
        case INPUTF_T:
        case PRINT_T:
        case LEN_T:
        case SUBSTR_T:
        case ORD_T:
        case CHR_T:     //rules 23 to 30 for bultins

            loadtoken;
            return 0;
        default:
            return SYN_ERROR;
    }
}

int eols(){
    switch (token->type)
    {
    case EOL_T: //33	<eols>	->	EOL <eols>
        loadtoken
        return eols();
    case INDENT_T:  //34	<eols>	->	indent
        loadtoken
        return 0;
    default:
        return SYN_ERROR;
    }
}
////////////////////////////////////////////////////////////////////////////////
///////////////////////         Expression          ////////////////////////////

//  Using infix to postfix helper functions from IAL homework 1

//  Empty stack until left bracket
int untilLeftPar (Stack *s, List *postfix) {
    if(s == NULL || postfix == NULL) return 0;

    if(isStackEmpty(s)) return 0;

    for(Token_t *tok = top(s)->data; !isStackEmpty(s); tok = top(s)->data){
        //Check if left par
        if(tok->type == LEFT_BRACKET_T){
            pop(s);
            //End cycle if yes
            break;
        }

        //Save token in list
        Token_t *op = malloc(sizeof(Token_t));
        if(op == NULL) return 0;
        op->type = tok->type;
        op->stringVal = initString();
        appendString(op->stringVal, tok->stringVal->string);
        appendList(postfix, op);

        //Pop the saved item
        pop(s);

        if(top(s) == NULL){
            //No left bracket to close
            return SYN_ERROR;
        }
    }
    //Should never get here but for compilers sake
    return 0;
}

//  Process an operator (with correct precedence etc.)
void doOperation (Stack* s, Token_t *operator, List* postfix) {
    if( isStackEmpty(s) ){
        //Can insert
        push(s, operator);
        return;
    }
    if ( ((Token_t*)(top(s)->data))->type == LEFT_BRACKET_T ){
        push(s, operator);
        return;
    }

    Token_t *op = malloc(sizeof(Token_t));
    if(op == NULL) return;
    op->stringVal = NULL;

    //Whats on top of stack
    Token_t *stackTop = (Token_t*)(top(s)->data);
    switch (stackTop->type) {
        case PLUS_OP_T:
        case MINUS_OP_T:
            //What operator?
            if( operator->type == DIV_OP_T || operator->type == INT_DIV_OP_T || operator->type == MUL_OP_T ){
                push(s, operator);
            }
            else {
                // + or -
                //Add operator from stack to output
                op->type = stackTop->type;
                appendList(postfix, op);
                pop(s);
                //Process another token on stack
                doOperation(s, operator, postfix);
            }
            break;
        case MUL_OP_T:
        case DIV_OP_T:
        case INT_DIV_OP_T:
            //Always has higher precendence

            //From stack to output
            op->type = stackTop->type;
            appendList(postfix, op);
            pop(s);
            //Next
            doOperation(s, operator, postfix);
            break;
        case EQUAL_T:
        case NOT_EQUAL_T:
        case GREATER_EQUAL_T:
        case LESS_EQUAL_T:
        case GREATER_THAN_T:
        case LESS_THAN_T:
            if( operator->type == DIV_OP_T || operator->type == INT_DIV_OP_T || operator->type == MUL_OP_T || operator->type == PLUS_OP_T || operator->type == MINUS_OP_T){
                push(s, operator);
            }
            else{
                //Relation operator on top of stack and the one we're processing is also relational
                //Add operator from stack to output
                op->type = stackTop->type;
                appendList(postfix, op);
                pop(s);
                //Process another token on stack
                doOperation(s, operator, postfix);
            }
            break;
        default:
            freeToken(op);
            break;
    }
}

//  Convert expression to postfix notation
int toPostfix (List *infix, List *postFix) {
    if(infix == NULL || postFix == NULL) return INT_ERROR;

    Stack *s = malloc(sizeof(Stack));
    if(s == NULL) return INT_ERROR;
    s->top = NULL;

    //Cycle through infix expression
    for(listItem *iterator = infix->first; iterator != NULL; iterator = iterator->next){
        int err = 0;
        symbolTable_t *id = NULL;
        Token_t * newtok;

        switch (iterator->value->type){
            case LEFT_BRACKET_T:
                if(iterator->next == NULL || iterator->next->value->type == RIGHT_BRACKET_T){
                    //Empty brackets
                    delStack(s);
                    return SYN_ERROR;
                }
                push(s, iterator->value);
                break;

            case PLUS_OP_T:
            case MINUS_OP_T:
            case MUL_OP_T:
            case DIV_OP_T:
            case INT_DIV_OP_T:
            case EQUAL_T:
            case NOT_EQUAL_T:
            case GREATER_EQUAL_T:
            case LESS_EQUAL_T:
            case GREATER_THAN_T:
            case LESS_THAN_T:
                if(iterator->next == NULL || iterator->next->value->type == RIGHT_BRACKET_T){
                    //Operator has no right side
                    delStack(s);
                    return SYN_ERROR;
                }
                doOperation(s, iterator->value, postFix);
                break;

            case RIGHT_BRACKET_T:
                err = untilLeftPar(s, postFix);
                if(err == SYN_ERROR){
                    delStack(s);
                    return SYN_ERROR;
                }
                break;

            case ID_T:
                //Check if defined
                if(actSymtable == LOCAL)
                    id = searchST(localST, iterator->value->stringVal->string);
                if(actSymtable == GLOBAL || id == NULL)
                    id = searchST(globalST, iterator->value->stringVal->string);

                if(id == NULL || id->symInfo->symType == FUNC){
                    delStack(s);
                    return SYN_ERROR; //id not found or Function in expression
                }

                newtok = malloc(sizeof(Token_t));
                if(newtok == NULL){
                    delStack(s);
                    return INT_ERROR;
                }

                newtok->type = ID_T;
                newtok->stringVal = initString();
                if(!newtok->stringVal) return INT_ERROR;
                if(appendString(newtok->stringVal, id->name) == -1)
                {
                   freeToken(newtok);
                   delList(postFix);
                   delStack(s);
                   return INT_ERROR;
                }

                appendList(postFix, newtok);
                break;

            default:
                //Int, float or a string, or 'None'
                newtok = malloc(sizeof(Token_t));
                if(newtok == NULL){
                    delStack(s);
                    return INT_ERROR;
                }

                newtok->type = iterator->value->type;
                newtok->stringVal = NULL;
                switch(newtok->type){
                    case INT_T:
                        newtok->intVal = iterator->value->intVal;
                        break;
                    case FLOAT_T:
                        newtok->floatVal = iterator->value->floatVal;
                        break;
                    case STRING_T:
                        newtok->stringVal = initString();
                        if(!newtok->stringVal)
                            return INT_ERROR;
                        if(appendString(newtok->stringVal, iterator->value->stringVal->string) == -1)
                            return INT_ERROR;
                        break;
                    case NONE_T:
                        break;
                    default:
                        delStack(s);
                        return INT_ERROR;
                }

                appendList(postFix, newtok);
                break;
        }
    }

    while(!isStackEmpty(s)){
        //Empty the rest of the operators
        Token_t *op = malloc(sizeof(Token_t));
        if(op == NULL) return INT_ERROR;
        Token_t *stackTop = (Token_t*)top(s)->data;
        op->type = stackTop->type;
        op->stringVal = NULL;

        if(op->type == LEFT_BRACKET_T){
            //Unclosed parenthesis (left bracket still on stack)
            delStack(s);
            free(op);
            return SYN_ERROR;
        }

        appendList(postFix, op);
        pop(s);
    }

    delStack(s);
    free(s);

    return 0;
}

int semantics(int *resType, List *postfix){
    Stack *s = malloc(sizeof(Stack)); //Memory for values
    if(s == NULL) return INT_ERROR;
    s->top = NULL;
    Token_t *token = NULL;

    for(postfix->current = postfix->first; postfix->current != NULL; postfix->current = postfix->current->next){
        symbolTable_t *id = NULL;

        switch(postfix->current->value->type){
            //Check the type of result of operation
            //Do the operation (type-wise) and push the result type

            // ARTITHMETIC OPERATORS
            case PLUS_OP_T:
                //Different behaviour when operating with strings
                if(s->top == NULL || s->top->data == NULL || s->top->next == NULL || s->top->next->data == NULL){
                    freeTokenStack(s);
                    return INT_ERROR;
                }

                if(((Token_t *)(top(s)->next->data))->type == ID_T || ((Token_t *)(top(s)->data))->type == ID_T){
                    *resType = ID_T;
                    break;
                }

                //Both int, it's int
                if(((Token_t *)(top(s)->next->data))->type == INT_T && ((Token_t *)(top(s)->data))->type == INT_T){
                    *resType = INT_T;
                }

                //Both string, it's string
                else if(((Token_t *)(top(s)->next->data))->type == STRING_T && ((Token_t *)(top(s)->data))->type == STRING_T){
                    *resType = STRING_T;
                }

                //If one is float and the other is not string, it's float
                else if(((Token_t *)(top(s)->next->data))->type == FLOAT_T){
                    if(((Token_t *)(top(s)->data))->type != STRING_T){
                        *resType = FLOAT_T;
                    }
                    else{
                        //Type compatibility error
                        freeTokenStack(s);
                        return TYPE_COMP_ERROR;
                    }
                }
                else if(((Token_t *)(top(s)->data))->type == FLOAT_T){
                    if(((Token_t *)(top(s)->next->data))->type != STRING_T){
                        *resType = FLOAT_T;
                    }
                    else{
                        //Type compatibility error
                        freeTokenStack(s);
                        return TYPE_COMP_ERROR;
                    }
                }
                else {
                    //Type compatibility error
                    freeTokenStack(s);
                    return TYPE_COMP_ERROR;
                }
                break;

            case MINUS_OP_T:
            case MUL_OP_T:
                if(s->top == NULL || s->top->data == NULL || s->top->next == NULL || s->top->next->data == NULL){
                    freeTokenStack(s);
                    return INT_ERROR;
                }

                if(((Token_t *)(top(s)->next->data))->type == ID_T || ((Token_t *)(top(s)->data))->type == ID_T){
                    *resType = ID_T;
                    break;
                }

                //Both int, it's int
                if(((Token_t *)(top(s)->next->data))->type == INT_T && ((Token_t *)(top(s)->data))->type == INT_T){
                    *resType = INT_T;
                }

                //If one is float and the other is not string, it's float
                else if(((Token_t *)(top(s)->next->data))->type == FLOAT_T){
                    if(((Token_t *)(top(s)->data))->type != STRING_T){
                        *resType = FLOAT_T;
                    }
                }
                else if(((Token_t *)(top(s)->data))->type == FLOAT_T){
                    if(((Token_t *)(top(s)->next->data))->type != STRING_T){
                        *resType = FLOAT_T;
                    }
                }

                else {
                    //Type compatibility error
                    freeTokenStack(s);
                    return TYPE_COMP_ERROR;
                }
                break;

            case DIV_OP_T:
                if(s->top == NULL || s->top->data == NULL || s->top->next == NULL || s->top->next->data == NULL){
                    freeTokenStack(s);
                    return INT_ERROR;
                }

                if(((Token_t *)(top(s)->next->data))->type == ID_T || ((Token_t *)(top(s)->data))->type == ID_T){
                    *resType = FLOAT_T;
                    break;
                }

                //Both must not be a string and result is always float
                if(((Token_t *)(top(s)->next->data))->type != STRING_T && ((Token_t *)(top(s)->data))->type != STRING_T){
                    if(((Token_t *)(top(s)->data))->type == INT_T){

                        if(((Token_t *)(top(s)->data))->intVal == 0){
                            freeTokenStack(s);
                            return ZERO_DIV_ERROR;
                        }
                    }
                    else if(((Token_t *)(top(s)->data))->type == FLOAT_T){

                        if(((Token_t *)(top(s)->data))->floatVal == 0.0){
                            freeTokenStack(s);
                            return ZERO_DIV_ERROR;
                        }
                    }
                    else {
                        *resType = FLOAT_T;
                    }
                }
                else {
                    freeTokenStack(s);
                    return TYPE_COMP_ERROR;
                }
                break;

            case INT_DIV_OP_T:
                if(s->top == NULL || s->top->data == NULL || s->top->next == NULL || s->top->next->data == NULL){
                    freeTokenStack(s);
                    return INT_ERROR;
                }

                if(((Token_t *)(top(s)->next->data))->type == ID_T || ((Token_t *)(top(s)->data))->type == ID_T){
                    *resType = INT_T;
                    break;
                }

                //Both must be int and result is also int
                if(((Token_t *)(top(s)->next->data))->type == INT_T && ((Token_t *)(top(s)->data))->type == INT_T){
                    if(((Token_t *)(top(s)->data))->intVal == 0){
                        freeTokenStack(s);
                        return ZERO_DIV_ERROR;
                    }
                    else {
                        *resType = FLOAT_T;
                    }
                }
                else {
                    freeTokenStack(s);
                    return TYPE_COMP_ERROR;
                }
                break;

            // RELATION OPERATORS

            case EQUAL_T:
            case NOT_EQUAL_T:
                if(s->top == NULL || s->top->data == NULL || s->top->next == NULL || s->top->next->data == NULL){
                    freeTokenStack(s);
                    return INT_ERROR;
                }
                *resType = INT_T;

                if(((Token_t *)(top(s)->next->data))->type == STRING_T){
                    if(((Token_t *)(top(s)->data))->type == STRING_T || ((Token_t *)(top(s)->data))->type == NONE_T){
                        *resType = INT_T;
                    }
                    else{
                        //Type compatibility error
                        freeTokenStack(s);
                        return TYPE_COMP_ERROR;
                    }
                }
                else if(((Token_t *)(top(s)->data))->type == STRING_T){
                    if(((Token_t *)(top(s)->next->data))->type == STRING_T || ((Token_t *)(top(s)->next->data))->type == NONE_T){
                        *resType = INT_T;
                    }
                    else{
                        //Type compatibility error
                        freeTokenStack(s);
                        return TYPE_COMP_ERROR;
                    }
                }

                break;
                //~ if(((Token_t *)(top(s)->next->data))->type == ID_T || ((Token_t *)(top(s)->data))->type == ID_T){
                    //~ *resType = INT_T;
                    //~ break;
                //~ }
                //~ else if(((Token_t *)(top(s)->data))->type == NONE_T || ((Token_t *)(top(s)->next->data))->type == NONE_T){
                    //~ *resType = INT_T;
                    //~ break;
                //~ }
                //~ else {
                    //~ //Type compatibility error
                    //~ freeTokenStack(s);
                    //~ return TYPE_COMP_ERROR;
                //~ }
                //~ break;

            case GREATER_EQUAL_T:
            case LESS_EQUAL_T:
            case GREATER_THAN_T:
            case LESS_THAN_T:
                if(s->top == NULL || s->top->data == NULL || s->top->next == NULL || s->top->next->data == NULL){
                    freeTokenStack(s);
                    return INT_ERROR;
                }

                if(((Token_t *)(top(s)->next->data))->type == ID_T || ((Token_t *)(top(s)->data))->type == ID_T){
                    *resType = INT_T;
                    break;
                }

                //If same type
                if(((Token_t *)(top(s)->next->data))->type == ((Token_t *)(top(s)->data))->type){
                    *resType = INT_T;
                }
                //If one is float and the other is int, it's also ok
                else if(((Token_t *)(top(s)->next->data))->type == FLOAT_T){
                    if(((Token_t *)(top(s)->data))->type == INT_T){
                        *resType = INT_T;
                    }
                    else{
                        //Type compatibility error
                        freeTokenStack(s);
                        return TYPE_COMP_ERROR;
                    }
                }
                else if(((Token_t *)(top(s)->data))->type == FLOAT_T){
                    if(((Token_t *)(top(s)->next->data))->type == INT_T){
                        *resType = INT_T;
                    }
                    else{
                        //Type compatibility error
                        freeTokenStack(s);
                        return TYPE_COMP_ERROR;
                    }
                }
                else {
                    //Type compatibility error
                    freeTokenStack(s);
                    return TYPE_COMP_ERROR;
                }
                break;

            case ID_T:
                if(actSymtable == LOCAL)
                    id = searchST(localST, postfix->current->value->stringVal->string);
                if(actSymtable == GLOBAL || id == NULL)
                    id = searchST(globalST, postfix->current->value->stringVal->string);

                if(id == NULL){
                    if(actSymtable == LOCAL) //in function it can be a param -> let it slide
                    {
                        goto generateExp;
                    }
                    else
                    {
                        freeTokenStack(s);
                        return SYN_ERROR;
                    }
                }
                //ID exists and 'id' is a pointer to its symbolInfo

                //Check if id is a parameter of function
                //If yes, the type check must happen at runtime and not now
                if(id->symInfo->varType == 0){ //0 is ID, and parameters are inserted into symtable with this value
                    //Possibly generate ifjcode to typecheck the expression
                    goto generateExp;
                }

                /*
                 * Now, create a new token to push to stack (it's a stack of Token_t)
                 * and push the correct type so type check can be done
                 */
                token = deepCopyToken(postfix->current->value);
                if(!token)//Or do not convert if one of them is None - type nil@nil
                {
                    freeTokenStack(s);
                    return INT_ERROR;
                }
                token->type = id->symInfo->varType;
                if(token->type == INT_T || token->type == FLOAT_T)
                {
                    delString(token->stringVal); //because of the union I lose access to it ... need to deallocate it now
                    token->stringVal = NULL;
                }
                push(s, token);
                break;

            default:
                token = deepCopyToken(postfix->current->value); //SEGFAULT with NONE_T
                if(!token)
                {
                    freeTokenStack(s);
                    return INT_ERROR;
                }
                push(s, token);
                *resType = token->type;
                fflush(stdout);
                break;
        }
    }

    //generating code
    generateExp:
    printExpression(postfix);
    freeTokenStack(s);
    return err;
}

void freeTokenStack(Stack *s)
{
    stackItem_t *tmp = (stackItem_t *)top(s);
    while(tmp)
    {
        if(tmp->data)
            freeToken(tmp->data);
        tmp = tmp->next;
    }
    delStack(s);
    free(s);
}

Token_t *deepCopyToken(Token_t *old)
{
    Token_t *new = malloc(sizeof(Token_t));
    if(!new) return NULL;
    new->type = old->type;
    new->stringVal = NULL;
    switch(new->type)
    {
        case INT_T: new->intVal = old->intVal; break;
        case FLOAT_T: new->floatVal = old->floatVal; break;
        case NONE_T: break;
        default:
            new->stringVal = initString();
            if(!new->stringVal) return NULL;
            if(appendString(new->stringVal, old->stringVal->string) == -1) return NULL;
            break;
    }
    return new;
}

int expr(int *resType){
    //TODO: IF pro funkci nebo výraz
    //printf("in expr res type is %i\n",*resType);
    List *infix = initList();
    List *postfix = initList();

    if(resType == NULL){
        //Add the previous token to the list
        appendList(infix, tokenHolder);
    }

    while(token->type != EOL_T && token->type != EOF_T && token->type != COLON_T)
    {
        switch(token->type){
            case ASSIGNMENT_T:
            case INDENT_T:
            case DEDENT_T:
            case LEFT_SQUARE_BRACKET_T:
            case RIGHT_SQUARE_BRACKET_T:
            case COMA_T:
            case IF_T:
            case ELSE_T:
            case DEF_T:
            case RETURN_T:
            case PASS_T:
            case WHILE_T:
            case INPUTS_T:
            case INPUTI_T:
            case INPUTF_T:
            case PRINT_T:
            case LEN_T:
            case SUBSTR_T:
            case ORD_T:
            case CHR_T:
                //Invalid token in expression
                delList(infix);
                delList(postfix);
                return SYN_ERROR;
            default:
                //Store the whole expression in a list to process
                appendList(infix, token);
                token = loadToken();
                if(token->type==LEXICAL_ERROR_T) return LEX_ERROR;
                if(token==NULL) return INT_ERROR;
        }
    }

    int err = toPostfix(infix, postfix);
    if(err){
        delList(infix);
        delList(postfix);
        return err;
    }

    if(resType == NULL){
        //Expression with nowhere to store result
        delList(infix);
        delList(postfix);
        return 0;
    }


    //Semantic check
    err = semantics(resType, postfix);
    if(err){

        delList(infix);
        delList(postfix);
        return err;
    }



    delList(infix);

    delList(postfix);

    return 0;
}
