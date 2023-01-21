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
 * @file scanner.c
 * @brief Source file for the scanner, which implements the functionality of the lexical analysis
 *
 */

#include "scanner.h"

Stack indentStack = { .top = NULL};
int returnDedent = 0;
TokenType_t prevToken = 100;         //no token type has this value - for first loading    
char *keywords[] = {"if", "else", "def", "return", "None", "pass", "while"};
char *builtins[] = {"inputs", "inputi", "inputf", "print", "len", "substr", "ord", "chr"};

Token_t *loadToken()
{
    //token allocation
    Token_t *token = (Token_t*) malloc(sizeof(Token_t));
    if(token == NULL) return NULL; //error message to be handled in parser
    //token initialization
    token->type = UNDEFINED;
    token->stringVal = initString();
    if(!token->stringVal) return NULL;

    //if we got any pending dedents to return, return dedent
    if(returnDedent > 0)
    {
        returnDedent--;
        token->type = DEDENT_T;
        prevToken = token->type;
        return token;
    }
 
    //initialize indent counting stack, if it's the first loadToken call
    if(indentStack.top == NULL)
    {
        int *initVal = malloc(sizeof(int));
        if(!initVal) return NULL;
        *initVal = 0;
        if(!push(&indentStack, initVal)) return NULL;
    }

    //if we're on the new line, the line is so far empty (no tokens loaded or empty ones)
    bool emptyLine = prevToken == EOL_T || prevToken == INDENT_T || prevToken == DEDENT_T || prevToken == 100;
    char firstHexaChar; //in case of hexachar, remembers the first of the two chars \xHh

    char c = getchar();                  //currently loaded character
    ungetc((char)c, stdin);              //I just want to peek the value ->return it
    //check for dedent to value 0 (no space to get to indent checking)
    if(prevToken == EOL_T  && c != '#' && c != '\t' && c != '\n' && c != ' ' && *(int *)top(&indentStack)->data != 0) 
    {
        returnDedent = -1; //we return one dedent right here, so the pendings will be -1
        while(*(int *)top(&indentStack)->data != 0)
        {
            free((int *)top(&indentStack)->data);
            pop(&indentStack);
            returnDedent++;
        }
        token->type = DEDENT_T;
        prevToken = token->type;
        return token;
    }

    State_t state = START_S ; //state of the finite automata
    //loading the characters
    while((c = getchar()))
    {
        char s[] = {c, '\0'};
        switch(state)
        {
            /*** start, from here we can get to most of states or even finish it with one char tokens ***/
            case START_S: 
                //checking specific characters
                state = END_S; //we either end up in end state or it would be redericted in the switch
                switch (c)
                {
                    //characters which stand as tokens themselves
                    case '+': token->type = PLUS_OP_T; break;
                    case '-': token->type = MINUS_OP_T; break;
                    case '*': token->type = MUL_OP_T; break;    
                    case ':': token->type = COLON_T; break;
                    case ',': token->type = COMA_T; break;
                    case '(': token->type = LEFT_BRACKET_T; break;
                    case ')': token->type = RIGHT_BRACKET_T; break;
                    case '[': token->type = LEFT_SQUARE_BRACKET_T; break;
                    case ']': token->type = RIGHT_SQUARE_BRACKET_T; break;
                    case '\n': 
                        token->type = EOL_T;
                        prevToken = token->type;
                        return token;
                    case EOF:
                        //if there are any pending dedents, file doesn't end correctly(with empty line)
                        //we pop empty the stack and retorn EOF on the input => we then load again and generate dedents
                        while(*(int *)top(&indentStack)->data > 0)
                        {
                            returnDedent++;
                            free((int *)top(&indentStack)->data);
                            pop(&indentStack);
                        }
                        if(returnDedent) ungetc((char)c, stdin);
                        //dedent can be only after EOL, and at the end of the program it does no harm to add it there
                        token->type = (returnDedent > 0) ? EOL_T : EOF_T;
                        prevToken = token->type;
                        return token;
                        
                    //characters which lead to another state
                    case '/':  state = DIV_S; break;
                    case '\'': state = STRING_S; break;
                    case '#':  state = ONE_LINE_COMMENT_S; break;
                    case '\"': state = DOCSTRING_IN_1_S; break;  
                    case '!':  state = NOT_EQUAL_S; break;
                    case '>':  state = GREATER_S; break;
                    case '<':  state = LESS_S; break;
                    case '=':  state = ASSIGN_S; break;
                    case '0':  
                        state = ZERO_S;
                        if(appendString(token->stringVal, s) == -1) return NULL;
                        break;
                    case ' ':  //whitespaces are removed after each token so we start afresh -> so space here means indent      
                        state = INDENT_S;
                        break;
                    case '\t':; //tabs have nothing to do in indents (tabs as whitespaces are removed after each token)
                        int emptyLineResult = checkEmptyLine();
                        switch(emptyLineResult)
                        {
                            case 2: 
                                state = ONE_LINE_COMMENT_S;
                                break;
                            default:
                                token->type = emptyLineResult ? EOL_T : LEXICAL_ERROR_T;
                                prevToken = token->type;
                                return token;
                        }
                        break;
                    //identifier or number
                    default:
                        //checking specific groups of characters
                        if( isalpha(c) || c == '_') //ID
                        {
                            token->type = ID_T;
                            if(appendString(token->stringVal, s) == -1) return NULL;
                            state = ID_S;
                        }
                        else if(isdigit(c)) //INT
                        {
                            if(appendString(token->stringVal, s) == -1) return NULL;
                            state = INT_S;
                        }
                        else goto lexicalError; //if we got here, the input char is not valid
                }
                break;

            /*** ending state - token is already determined, it's for removing whitespaces ***/
            case END_S:
                if(c == ' ' || c == '\t') continue;
                else
                {
                    ungetc((char)c, stdin);
                    prevToken = token->type;
                    return token;
                }

            /*** indent ***/
            case INDENT_S:
                ungetc((char)c, stdin); //we need to start afresh in indent checking
                if(!checkIndents(token, &state)) return NULL;
                if(token->type != UNDEFINED)   //in case of the same indent
                {
                    prevToken = token->type;
                    return token;
                }  
                break;

            /*** identifier ***/
            case ID_S:
                if(isalnum(c) || c == '_')  //if an underscore or alphanumeric char
                {   
                    if(appendString(token->stringVal, s) == -1) return NULL; //continue building the id's value
                }
                else
                {                       
                    ungetc((char)c, stdin); //otherwise return the char for recalculation from start state
                    int id;
                    if( (id = isKeywordOrBuiltin(token->stringVal->string)) )
                    {
                        token->type = id;
                    }
                    state = END_S;     //and end function
                }
                break;

            /*** numbers ***/
            /*integer*/
            case INT_S:
                if(isdigit(c))    //continues while numbers
                {
                    if(appendString(token->stringVal, s) == -1) return NULL;
                }
                else if(c == '.') //or goes to float on dot
                {
                    state = FLOAT_S;
                    if(appendString(token->stringVal, s) == -1) return NULL;
                }
                else endNumber(c, token, &state, true); //otherwise end
                break;

            /*float*/
            case FLOAT_S:
                if(isdigit(c))
                {
                    if(appendString(token->stringVal, s) == -1) return NULL;
                }
                else if(c == 'e' || c == 'E')
                {
                    //if last char was a dot -> error e.g. 0xac.E is invalid
                    if(*(token->stringVal->string+(strlen(token->stringVal->string)-1)) == '.') goto lexicalError;
                    if(appendString(token->stringVal, s) == -1) return NULL;
                    state = TO_SCIENTIFIC_S;
                }
                else if(*(token->stringVal->string+(strlen(token->stringVal->string)-1)) == '.') //if the last char was a dot we cannot end
                    goto lexicalError;                                                           // 5. is invalid
                else endNumber(c, token, &state, false); //otherwise end
                break;

            case TO_SCIENTIFIC_S: //here we've got e/E loaded, checking if go to sientific with optional sign or not
                if(appendString(token->stringVal, s) == -1) return NULL;
                if(c == '+' || c == '-')
                    state = SIGN_SCIENTIFIC_S;
                else if(isdigit(c))
                    state = SCIENTIFIC_S;
                else
                    goto lexicalError;
                break;

            case SIGN_SCIENTIFIC_S: //here we've got a sign and we continue to scientific notation
                if(isdigit(c))
                {
                    if(appendString(token->stringVal, s) == -1) return NULL;
                    state = SCIENTIFIC_S;
                    break;
                }
                else goto lexicalError;

            case SCIENTIFIC_S:   //state of scientific notation of float
                if(isdigit(c))
                {
                    if(appendString(token->stringVal, s) == -1) return NULL;
                } 
                else endNumber(c, token, &state, false); //otherwise end
                break;
            
            /*hexadecimal*/
            case ZERO_S:
                if(isdigit(c))      //in case of a digit, it's superfluous 0 -> error
                {                   
                    //unless the last woken was assignment ... a = 05 is invalid
                    goto lexicalError;
                }
                else if(c == 'x' || c == 'X') //it is hexadecimal
                {
                    if(appendString(token->stringVal, s) == -1) return NULL;
                    state = HEXA_INT_S;
                }
                else if(c == '.')  //it's float
                {
                    state = FLOAT_S;
                    if(appendString(token->stringVal, ".") == -1) return NULL;
                }
                else endNumber(c, token, &state, true); //otherwise end
                break;

            case HEXA_INT_S:  //hexadecimal int value
                if(isdigit(c) || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F'))
                {
                    if(appendString(token->stringVal, s) == -1) return NULL;
                }
                else if(c == '.')
                {   //we need to check last char ... e.g. 0x.A is invalid
                    if(*(token->stringVal->string + strlen(token->stringVal->string)-1) == 'x') goto lexicalError;
                    if(appendString(token->stringVal, s) == -1) return NULL;
                    state = HEXA_FLOAT_S;
                }
                else if(*(token->stringVal->string+(strlen(token->stringVal->string)-1)) == '.') //if the last char was a dot we cannot end
                    goto lexicalError;                                                           // 0x5. is invalid
                else endNumber(c, token, &state, true); //otherwise end
                break;

            case HEXA_FLOAT_S:
                if(isdigit(c) || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F'))
                {
                    if(appendString(token->stringVal, s) == -1) return NULL;
                }
                else if(c == 'p') //hexadecimal float requires exponent
                {
                    //if last char was a dot -> error e.g. 0xac.p is invalid
                    if(*(token->stringVal->string+(strlen(token->stringVal->string)-1)) == '.') goto lexicalError;
                    if(appendString(token->stringVal, s) == -1) return NULL;
                    state = TO_HEXA_SCIENTIFIC_S;
                }
                else goto lexicalError;
                break;

            case TO_HEXA_SCIENTIFIC_S: //same as with normal one, checking optional sign
                if(appendString(token->stringVal, s) == -1) return NULL;
                if(c == '+' || c == '-')
                    state = SIGN_HEXA_SCIENTIFIC_S;
                else if(isdigit(c))
                    state = SCIENTIFIC_HEXA_S;
                else
                    goto lexicalError;
                break;

            case SIGN_HEXA_SCIENTIFIC_S: //same as with normal one, having a sign
                if(isdigit(c))
                {
                    if(appendString(token->stringVal, s) == -1) return NULL;
                    state = SCIENTIFIC_HEXA_S;
                    break;
                }
                else goto lexicalError;

            case SCIENTIFIC_HEXA_S: //hexadecimal floating point notation
                if(isdigit(c))      //exponent after p cannot be hexadecimal
                {
                    if(appendString(token->stringVal, s) == -1) return NULL;
                }
                else endNumber(c, token, &state, false); //otherwise end
                break;
            
            /*** strings ***/
            case STRING_S:
                switch(c)
                {
                    case '\'': //end of string
                        token->type = STRING_T;
                        state = END_S;
                        break;
                    case EOF:  //end of file inside string is unacceptable
                    case '\n': //strings can be only on one line
                        goto lexicalError;
                    case '\\': //escape sequence
                        state = ESCAPE_SEQUENCE_S;
                        break;
                    default:  //continue building the string
                        if(appendString(token->stringVal, s) == -1) return NULL;
                }
                break;

            case ESCAPE_SEQUENCE_S:
                state = STRING_S; //from escape sequence we always return to string unless going to hexadecimal one
                switch(c)
                {   //check for valid escape sequences
                    case '\'':
                        if(appendString(token->stringVal, "\'") == -1) return NULL;
                        break;
                    case '\"':
                        if(appendString(token->stringVal, "\"") == -1) return NULL;
                        break;
                    case 'n':
                        if(appendString(token->stringVal, "\n") == -1) return NULL;
                        break;
                    case 't':
                        if(appendString(token->stringVal, "\t") == -1) return NULL;
                        break;
                    case '\\':
                        if(appendString(token->stringVal, "\\") == -1) return NULL;
                        break;
                    case 'x': //hexadecimal escape sequence
                        state = HEXA_CHAR_1_S;
                        break;
                    case EOF:  //end of file inside string is unacceptable
                    case '\n': //strings can be only on one line
                        goto lexicalError;
                    default:;  //otherwise we should print what was written
                        char str[] = {'\\', c, '\0'};
                        if(appendString(token->stringVal, str) == -1) return NULL;
                        break;
                }
                break;

            case HEXA_CHAR_1_S: //hexadecimal escape sequence must have exactly 2 chars (\xhh) -> state of first one
                //check for valid hexadecimal value
                if( (c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f'))
                {
                    firstHexaChar = c;    //format is \xHH ... to 2 chars are needed to check
                    state = HEXA_CHAR_2_S;
                }
                else if( c == '\n' || c == EOF ) //lexical error
                {
                    goto lexicalError;
                }
                else  //this escape sequence is broken so we print it as it is
                {
                    char str[] = {'\\', 'x', '\0'};
                    if(appendString(token->stringVal, str) == -1) return NULL;
                    switch(c)
                    {
                        case '\\':
                            state = ESCAPE_SEQUENCE_S;
                            break;
                        case '\'': 
                            token->type = STRING_T;
                            state = END_S;
                            break;
                        default:
                            if(appendString(token->stringVal, s) == -1) return NULL;
                            state = STRING_S;
                            break;
                    }
                }
                break;
            case HEXA_CHAR_2_S: //hexadecimal escape sequence must have exactly 2 chars (\xhh) -> state of first one
                //check for valid hexadecimal value
                if( (c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f'))
                {
                    char hexaChar = '\x00'; //we need to create escape sequence out of several existing chars
                    hexaChar += convertHexaCharToDec(firstHexaChar)*16 + convertHexaCharToDec(c); //so we add the hexa value to the starting one
                    char str[] = {hexaChar, '\0'};
                    if(appendString(token->stringVal, str) == -1) return NULL;
                    state = STRING_S;
                }
                else if( c == '\n' || c == EOF ) //lexical error
                {
                    goto lexicalError;
                }
                else  //this escape sequence is broken so we print it as it is
                {
                    char str[] = {'\\', 'x', firstHexaChar, '\0'};
                    if(appendString(token->stringVal, str) == -1) return NULL;
                    switch(c)
                    {
                        case '\\':
                            state = ESCAPE_SEQUENCE_S;
                            break;
                        case '\'': 
                            token->type = STRING_T;
                            state = END_S;
                            break;
                        default:
                            if(appendString(token->stringVal, s) == -1) return NULL;
                            state = STRING_S;
                            break;
                    }
                }
                break;

            /*** comments ***/
            /*one-line comment*/
            case ONE_LINE_COMMENT_S:
                switch(c)
                {
                    case EOF: //end of file is valid on a comment line
                        token->type = EOF_T;
                        return token;
                    case '\n': //end of one-line comment
                        token->type = EOL_T;
                        prevToken = token->type;
                        return token;
                }
                break;

            /*way to docstring*/
            case DOCSTRING_IN_1_S: // here we've just got "
                switch(c)
                {
                    case '\"': //getting to docstring
                        state = DOCSTRING_IN_2_S;
                        break;
                    default: goto lexicalError;   // simple " has no meaning
                }
                break;

            case DOCSTRING_IN_2_S: // here we've just got ""
                switch(c)
                {
                    case '\"': //successfully got to docstring
                        state = DOCSTRING_S;
                        break;
                    default: goto lexicalError;  // not even 2 "" have any meaning
                }
                break;

            /*docstring*/
            case DOCSTRING_S: // here we've finally got all tree """ and are in docsting
                switch(c)
                {
                    case '\"': //end of docstring
                        state = DOCSTRING_OUT_1_S;
                        break;
                    case EOF:  //invalid end of input program
                        goto lexicalError;
                    default: 
                        if(!appendString(token->stringVal, s)) return NULL;
                }
                break;

            /*way out of docstring*/
            case DOCSTRING_OUT_1_S: // here we've just got "
                switch(c)
                {
                    case '\"': //continue getting out of docstring
                        state = DOCSTRING_OUT_2_S;
                        break;
                    case EOF:  //invalid end of input program
                        goto lexicalError;
                    default:   //any other character breaks the "escaping" string -> we stay in the docstring
                        state = DOCSTRING_S;
                        if(!appendString(token->stringVal, "\"")) return NULL;
                        break;
                }
                break;

            case DOCSTRING_OUT_2_S: // here we've just got ""
                switch(c)
                {
                    case '\"': //successful end of docstring
                        if(emptyLine && checkEmptyLine()==1)  //if before the docstring(emtyLine) and after it (checkEmptyLine)
                        {                                     //are only white spaces, it is a multiline comment and we ignore it
                            delString(token->stringVal);
                            token->stringVal = NULL;
                            token->type =  EOL_T;
                            prevToken = token->type;
                            return token;
                        }
                        else //else it's a multiline string literal
                        {
                            token->type = STRING_T;
                            state = END_S;
                        }
                        break;
                    case EOF:  //invalid end of input program
                        goto lexicalError;
                    default:   //any other character breaks the "escaping" string -> we stay in the comment
                        state = DOCSTRING_S;
                        if(!appendString(token->stringVal, "\"\"")) return NULL;
                        break;
                }
                break;

            /*** operators ***/
            case DIV_S: 
                //we either continue to // operator
                if(c == '/')
                {
                    token->type = INT_DIV_OP_T;
                }
                else //or we use current one ('/')
                {
                    ungetc((char)c, stdin);
                    token->type = DIV_OP_T;
                }
                state = END_S;
                break;

            case NOT_EQUAL_S: 
                //we either continue to != operator
                if(c == '=')
                {
                    token->type = NOT_EQUAL_T;
                    state = END_S;
                    break;
                }
                goto lexicalError; //or we get lexical error as '!' alone means nothing

            case GREATER_S: 
                //we either continue to <= operator
                if(c == '=')
                {
                    token->type = GREATER_EQUAL_T;
                } 
                else //or we stick with just < operator
                {
                    ungetc((char)c, stdin);
                    token->type = GREATER_THAN_T;
                }
                state = END_S;
                break;

            case LESS_S:
                //we either continue to >= operator
                if(c == '=')
                {
                    token->type = LESS_EQUAL_T;
                }
                else//or we stick with just > operator
                {
                    ungetc((char)c, stdin);
                    token->type = LESS_THAN_T;
                }
                state = END_S;
                break;

            case ASSIGN_S:
                //we either continue to == operator
                if(c == '=')
                {
                    token->type = EQUAL_T;
                }
                else //or we stick with just assignment operator
                {
                    ungetc((char)c, stdin);
                    token->type = ASSIGNMENT_T;
                }
                state = END_S;
                break;
        }
    }
    //we should never get here, everything should be handled by the switch/indent block in the loop
    // -> so if we somehow get here, it's an error as it isn't suppose to happen
    //OR we jump here when an lexical error is discovered -> most often case
  lexicalError:
    token->type = LEXICAL_ERROR_T;
    return token;
}

bool checkIndents(Token_t *token, State_t *state)
{
    int *indent = countSpaces();
    if(!indent) return false;
    int stackIndent;
    switch(*indent)
    {
        case -1: //lexical error due to tab in indent X still an empty line or comment can save it
            switch(checkEmptyLine())
            {
                case 0: token->type = LEXICAL_ERROR_T; break;
                case 1: token->type = EOL_T; break;
                case 2: *state = ONE_LINE_COMMENT_S; break;
            }
            break;
        case -2: //another EOL - so empty line, nothing happens
            token->type = EOL_T;
            break;
        default:
            stackIndent = *(int *)top(&indentStack)->data;
            if(stackIndent > *indent)           //if the indent is lower then the current one
            {
                //searching for this indent in the stack
                while(true)
                {
                    free((int *)top(&indentStack)->data);
                    pop(&indentStack);
                    stackIndent = *(int *)top(&indentStack)->data;
                    if(stackIndent == *indent)
                    {
                        token->type = DEDENT_T;
                        break;
                    }
                    if(stackIndent == 0) //we're on top of the stack and matching indent wasn't found
                    {
                        token->type = LEXICAL_ERROR_T;
                        break;
                    }
                    returnDedent++;
                }
            }
            else if(stackIndent < *indent)                   //if the indent is higher
            {
                if(!push(&indentStack, indent)) return false;
                token->type = INDENT_T;                     //and generate INDENT token
            }
            else //it matches and we return to start state
                *state = START_S;
    }
    if(token->type != INDENT_T)
        free(indent);
    return true;
}

int *countSpaces()
{
    int c;
    int *count = malloc(sizeof(int));if(!count) return NULL;
    *count = 1; //one space is already loaded which led us to indent state
    while( (c = getchar()) )
    {
        switch(c)
        {
            case '\n': *count = -2; return count;
            case '\t': *count = -1; return count;
            case ' ': (*count)++; break;
            default: //if its a valid character, return it to stdin and end
                ungetc((char)c, stdin);
                return count;
        }
    }
    return count;
}

int isKeywordOrBuiltin(char *str)
{
    //search in keyword array
    for(int i = 0; i < NUM_OF_KEYWORDS; i++)
        if(!strcmp(str, keywords[i]))
            return KEYWORDS + i;
    //search in built-in array
    for(int i = 0; i < NUM_OF_BUILTINS; i++)
        if(!strcmp(str, builtins[i]))
            return BUILTINS + i;
    return 0;
}

void endNumber(char c, Token_t *token, State_t *state, bool integer)
{
    ungetc(c, stdin);                               //return the char to the input
    double aux;
    sscanf(token->stringVal->string, "%lf", &aux);  //convert the string to double
    delString(token->stringVal); //because of the union I loose access to it ... need to deallocate it now
    token->stringVal = NULL;
    if(integer)                                     //assign the number to int/float
    {
        token->type = INT_T;
        token->intVal = (int)aux;
    }
    else
    {
        token->type = FLOAT_T;
        token->floatVal = aux;
    }
    *state = END_S;                                 //redirect to the end state
}

int checkEmptyLine()
{
    char c;
    while( (c = getchar()) )
    {
        switch(c)
        {
            case ' ':
            case '\t': continue;
            case '\n': return 1;
            case '#':  return 2;
            case EOF:
                ungetc((char)c, stdin);
                return 1;
            default:
                ungetc((char)c, stdin);
                return 0;
        }
    }
    return 0;
}

int convertHexaCharToDec(char c)
{
    switch(c)
    {
        case 'a': case 'A': return 10;
        case 'b': case 'B': return 11;
        case 'c': case 'C': return 12;
        case 'd': case 'D': return 13;
        case 'e': case 'E': return 14;
        case 'f': case 'F': return 15;
        default: return c - '0';
    }
}

void freeToken(Token_t *token)
{
    if(token)
    {
        if(token->type != INT_T && token->type != FLOAT_T && token->stringVal){ //string already deallocated after the conversion to int/float
            delString(token->stringVal);
        }
        free(token);
        token = NULL;
    }
}
