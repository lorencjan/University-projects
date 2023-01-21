//*  File: tail2.cc
//*  Solution: IJC-DU2
//*  Date: 29.3.2019
//*  Author: Jan Lorenc
//*  Faculty: Faculty of information technologies VUT
//*  Compiler: g++ 7.3.0
//*  Description: This program simulates basic functionalities of shell tail program

#include<iostream>
#include<fstream>
#include<string>
#include<cctype>  // for isdigit function
#include<vector>

using namespace std;

enum Mode{STANDARD, FROM_BEGINNING, EXIT};

/*** Class Tail simulates shell tail programm***/
class Tail
{
private:
	Mode mMode;
	unsigned int mNumOfLines;
	vector<string> mArgs;
	vector<string> mFileNames;
public:
	Tail();
	void GetArguments(int argc, char *argv[]);
	void FrontTail(istream &file);
	void BackTail(istream &file);
	void Execute();
};

/*** MAIN ***/

int main(int argc, char* argv[])
{
	//ios_base::sync_with_stdio(false);	// increse spead of iostream, !!!valgrinds 6 memory leaks!!!
	Tail tail;
	tail.GetArguments(argc, argv);
	tail.Execute();
	return 0;
}


/*** DEFINITIONS OF TAIL METHODS ***/

// constructor sets mode to standard (print last N lines) and
// sets default value of 10 to the number of lines to be printed
Tail::Tail()
{
	mMode = STANDARD;
	mNumOfLines = 10;
}

// Method processes program arguments, does not return any value but sets class attributes
void Tail::GetArguments(int argc, char *argv[])
{
	// check if there are any arguments
	if(argc < 2)
		return;
	
	// converting c strings to c++ strings for better later use
	for(int i=1; i<argc; i++)
		mArgs.push_back(argv[i]);

	//if there is an option as a argument, expect another one with its value
	if(!mArgs[0].compare("-n"))
	{
		if(mArgs.size()>1)
		{
			//check if the first character is a digit or format of '+'
			if(!isdigit(mArgs[1].at(0)) && mArgs[1].at(0) != '+')
			{
				cerr << "Argument error: A numerical or +NUMBER value is required after -n option!" << endl;
				goto exit;
			}
			else if (mArgs[1].at(0) == '+')	//in case of '+' format, we go from the beginning
					mMode = FROM_BEGINNING;
			//all characters must be digits -> whole argument is a number
			for(unsigned int i=1; i<mArgs[1].length(); i++)
			{
				if(!isdigit(mArgs[1].at(i)))
				{
					cerr << "Argument error: A numerical or +NUMBER value is required after -n option!" << endl;
					goto exit;
				}
			}
			//now I now i have an integer in the argument string
			mNumOfLines = stoi(mArgs[1]);
		}
		else  //there is no value for -n option
		{
			cerr << "Argument error: A value is required after -n option!" << endl;
			goto exit;
		}
		// load filenames
		for(int i=3; i<argc; i++)
			mFileNames.push_back(mArgs[i-1]);
	}
	else //without the option we just copy all arguments to the array
	{
		for(int i=1; i<argc; i++)
			mFileNames.push_back(mArgs[i-1]);
	}
	return;

	exit:
		mMode = EXIT;
		return;
}

// Method simulates tail in + mode ... prints the file starting with Nth line
void Tail::FrontTail(istream &file)
{
	string line;
	//skips N-1 lines
	for(unsigned int i = 1; i < mNumOfLines; i++)
		getline(file, line);
	//prints the rest
	while(getline(file, line))
		cout << line << endl;
}

// Method prints last "mNumOfLines" lines from "file"
void Tail::BackTail(istream &file)
{
	// loads the file line by line
	vector<string> lines;
	string line;
	while(getline(file, line))
		lines.push_back(line);
	// removes lines till we're not on Nth line
	while( lines.size() > mNumOfLines )
		lines.erase(lines.begin());
	for (auto &line : lines)
		cout << line << endl;
}

void Tail::Execute()
{
	// if an error occured earlier in the programm, ends
	if(mMode == EXIT)
		return;
	// if no files were given, read from stdin and executes tail
	if(mFileNames.empty())
	{
		if(mMode == STANDARD)
			BackTail(cin);
		else
			FrontTail(cin);
	}
	else // else executes tail for each file given
	{
		bool first = true;
		for (auto filename : mFileNames)
		{
			ifstream file;
			file.open(filename);
			if(file.is_open())
			{
				if(mFileNames.size() > 1)
				{
					if(!first)
						cout << endl;
					else
						first = false;
					cout << "==> " << filename << " <==" << endl;
				}
				if(mMode == STANDARD)
					BackTail(file);
				else
					FrontTail(file);
				file.close();
			}
			else
			{
				if(!first)
					cout << endl;
				cerr << "Error: " << filename << " could not be opened. It is probably unreadable file or it does not exist!" << endl;
				continue;
			}
		}
	}
}
