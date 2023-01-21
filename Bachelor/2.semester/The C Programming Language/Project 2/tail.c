//*  File: tail.c
//*  Solution: IJC-DU2
//*  Date: 23.3.2019
//*  Author: Jan Lorenc
//*  Faculty: Faculty of information technologies VUT
//*  Compiler: gcc 7.3.0
//*  Description: This program simulates basic functionalities of shell tail program

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <ctype.h>
#include <stdbool.h>

#define LINE_LENGTH_LIMIT 2000
#define DEFAULT_NUM_OF_LINES 10

void tail(int numOfLines, FILE* file);
void frontTail(int start, FILE* file);
int getArguments(int *numOfLines, int *numOfFiles, bool *fromEnd, char ***fileNames, int argc, char *argv[]);
FILE* stdinToTmpfile(void);

int main(int argc, char* argv[])
{
	// default values
	bool fromEnd = true;
	int numOfLines = DEFAULT_NUM_OF_LINES;
	int numOfFiles = 0;
	char **fileNames = NULL;
	if(argc > 1)
	{
		if(!getArguments(&numOfLines, &numOfFiles, &fromEnd, &fileNames, argc, argv))
			return 0;
	}
	//if no files were in the arguments, read stdin
	if(numOfFiles==0)
	{
		FILE* tmp = stdinToTmpfile();
		if(fromEnd)
			tail(numOfLines, tmp);
		else
			frontTail(numOfLines, tmp);
		fclose(tmp);     //unnecessary, but valgrind will be happy
		free(fileNames); //no bytes were allocated, but to keep malloc-free convention
		return 0;
	}
	// runing the tail function on each file
	for(int i = 0; i < numOfFiles; i++)
	{
		//try to open the file
		FILE* file = fopen(fileNames[i], "r");
		if(!file)
		{
			fprintf(stderr, "Error: %s could not be opened. It is probably unreadable file or it does not exist!\n", fileNames[i]);
			continue;
		}
		//print file separator and name if there are more file on input
		if(numOfFiles != 1)
			printf("==> %s <==\n", fileNames[i]);
		//call tail to print the result
		if(fromEnd)
			tail(numOfLines, file);
		else
			frontTail(numOfLines, file);
		
		fclose(file);
		//print newline between files
		if(i+1 != numOfFiles)
			printf("\n");
	}
	
	// deallocatig all memory
	for(int i = 0; i<numOfFiles; i++)
		free(fileNames[i]);
	free(fileNames);
	return 0;
}

/*** function prints last "numOfLines" lines from "file" ***/
void tail(int numOfLines, FILE* file)
{
	//get to the end of file and skip the last chararcter, if it is a newline, we ignore it as standard tail, else it doesn't matter
	if(fseek(file, -1, SEEK_END))  		 
		return;		//if error occured, it means the file is empty and we went before the beginning -> nothing happens

	//searching for new lines to get to position where we start printing
	int counter = 0;
	while(true)
	{
		//if the position is about to go before the beginning of the file, there is not enough lines in the file -> we skip it
		if(fseek(file, -1, SEEK_CUR))
			break;
		if(fgetc(file) == '\n')
			counter++;
		//if less then N lines are in the file, first char would be skipped due to the previous condition -> we need to get back
		//but only if its not the Nth line ... that would get us on '\n' char of previous one ... in this case we stop the search
		if(counter != numOfLines)
			fseek(file, -1, SEEK_CUR);
		else
			break;
	}

	//printing results
	bool errorHasOccured = false;
    char str[LINE_LENGTH_LIMIT];
    while (fgets(str, LINE_LENGTH_LIMIT, file)) 
    {
    	//if the line is longer then our buffer, we print what we've got, print error and move to the beginning of next line
    	if(str[strlen(str)-1] != '\n' && getc(file) != EOF)
    	{
    		fseek(file, -1, SEEK_CUR); //get one char back - if the check for EOF was EOL, we'd skip a whole line
    		printf("%s\n", str);
    		if(!errorHasOccured)
    		{
    			fprintf(stderr, "Error: Line is too long. Maximum length of line is %d characters -> line has been cut to that!\n", DEFAULT_NUM_OF_LINES);
    			errorHasOccured = true;
    		}
    		int c;
    		while((c=getc(file)) != '\n')
    		{	
    			if(c == EOF)
    				break;
    		}
    	}
    	else //else we just print the line
    		printf("%s", str);
    } 
}

/*** function simulates tail in + mode ... prints the file starting with Nth line ***/
void frontTail(int start, FILE* file)
{
	//skips N-1 lines
	int counter = 0;
	int c = getc(file);
	while(counter != start-1)
	{
		if(c == '\n')
			counter++;
		if(c == EOF)
			return;
		c = getc(file);
	}
	//prints the rest
	while(c!=EOF)
	{
		putchar(c);
		c = getc(file);
	}
}

/* function gets all relevant information from program arguments and pass it back via pointers
 * we get number of lines to print no matter the mode, the mode ( + means we skip N lines and print the rest, 
 * sole number indicates N last lines) and array of names of files
 * returns 1 if successfull, 0 if not
 */
int getArguments(int *numOfLines, int *numOfFiles, bool *fromEnd, char ***fileNames, int argc, char *argv[])
{
	//if there is an option as a argument, expect another one with its value
	if(!strcmp(argv[1], "-n"))
	{
		if(argv[2])
		{
			//check if the first character is a digit or format of '+'
			if(!isdigit(argv[2][0]) && argv[2][0] != '+')
			{
				fprintf(stderr, "Argument error: A numerical or +NUMBER value is required after -n option!\n");
				return 0;
			}
			else if (argv[2][0] == '+')	//in case of '+' format, we go from the beginning
					*fromEnd = false;
			//all characters must be digits -> whole argument is a number
			for(unsigned int i=1; i<strlen(argv[2]); i++)
				if(!isdigit(argv[2][i]))
				{
					fprintf(stderr, "Argument error: A numerical or +NUMBER value is required after -n option!\n");
					return 0;
				}
			//convert argument to a number
			if(sscanf(argv[2], "%d", numOfLines) != 1)
			{
				fprintf(stderr, "Argument error: Wrong format of -n value!\n");
				return 0;
			}
		}
		else  //there is no value for -n option
		{
			fprintf(stderr, "Argument error: A value is required after -n option!\n");
			return 0;
		}
		*numOfFiles = argc-3;
		//allocate memory for array of files
		if( (*fileNames = malloc(sizeof(char*)*(*numOfFiles))) == NULL)
		{
			fprintf(stderr, "Memory allocation error: Error occured while allocating memory!\n");
			return 0;
		}
		for(int i=0; i<*numOfFiles; i++)
		{
			int length = strlen(argv[i+3])+1;
			if( ((*fileNames)[i] = malloc(length)) == NULL)
			{
				fprintf(stderr, "Memory allocation error: Error occured while allocating memory!\n");
				return 0;
			}
			//copy the arguments to the file array
			memcpy((*fileNames)[i], argv[i+3], length);//strcpy((*fileNames)[i], argv[i+3]);
		}
	}
	else //without the option we just copy all arguments to the array
	{
		*numOfFiles = argc-1;
		if( (*fileNames = malloc(sizeof(char*)*(*numOfFiles))) == NULL)
		{
			fprintf(stderr, "Memory allocation error: Error occured while allocating memory!\n");
			return 0;
		}
		for(int i=0; i<*numOfFiles; i++)
		{
			int length = strlen(argv[i+1])+1;
			if( ((*fileNames)[i] = malloc(length)) == NULL)
			{
				fprintf(stderr, "Memory allocation error: Error occured while allocating memory!\n");
				return 0;
			}
			memcpy((*fileNames)[i], argv[i+1], length);
		}
	}
	return 1;
}

/*** reads stdin to a tmpfile which it returns ***/
FILE* stdinToTmpfile(void)
{
	//cannot just use stdin file from the beginning, because then
	//the program wouldn't wait if not even <file was given
	FILE* tmp = tmpfile();
	int c;
	while( (c=getc(stdin)) != EOF )
		putc(c, tmp);
	rewind(tmp);
	return tmp;
}
