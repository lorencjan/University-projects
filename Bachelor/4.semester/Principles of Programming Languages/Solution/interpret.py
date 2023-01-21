#!/usr/bin/env python3

"""
 *  File: interpret.py
 *  Solution: IPP project 2019/2020
 *  Date: 23.2.2020
 *  Author: Jan Lorenc (xloren15)
 *  Faculty: Faculty of Information Technology VUT
 *  Description: This script interprets the XML representation of language IPPcode20
"""

import sys, getopt, re
import xml.etree.ElementTree as XML

class Const:
    """Simple namespace class for program constants"""
    
    Int = "int"
    String = "string"
    Bool = "bool"
    Null = "nil"
    Var = "var"
    Label = "label"
    GF = "GF"
    LF = "LF"
    TF = "TF"



class Error:
    """Class with a method implementing the error end of program ... to be inhereted from"""
    
    @staticmethod
    def Error(errCode, msg):
        sys.stderr.write(msg + "\n")
        sys.exit(errCode)



class Stats:
    """This class takes care of statistical information for STATI extension"""
    
    File = None   #output file
    Insts = 0     #total number of called instructions
    Vars = 0      #current number of initialized variables
    VarsMax = 0   #maximum number or initialized variables at the same time
    #define whether to print them and if so, their number is the index in args -> lower goes first
    PrintInsts = None
    PrintVars = None
    Print = False #defines whether to even print the file
    
    @classmethod
    def UninitVar(cls, var):
        """Increases the number of currently initialized variable,
           if the variable hasn't been initialized, also sets new max if necessary"""
        if var.Type == None:
            cls.Vars += 1
        if cls.Vars > cls.VarsMax:
            cls.VarsMax = cls.Vars
    
    @classmethod
    def FrameDestroyed(cls, frameVars):
        """Decrements initialized vars counter for each initialized variable in the destroyed frame"""
        if frameVars == None:
            return
        for var in frameVars.values():
            if var.Type != None:
                cls.Vars -= 1

    @classmethod
    def Out(cls):
        """Prints the statistical output to the file"""
        if cls.Print == False:
            return
        
        if cls.PrintInsts != None and cls.PrintVars != None:
            while len(cls.PrintInsts) != 0 or len(cls.PrintVars) != 0:
                if len(cls.PrintInsts) == 0:
                    for i in cls.PrintVars:
                        cls.File.write(str(cls.VarsMax) + "\n")
                    break
                if len(cls.PrintVars) == 0:
                    for i in cls.PrintInsts:
                        cls.File.write(str(cls.Insts) + "\n")
                    break
                insts = cls.PrintInsts[0]
                var = cls.PrintVars[0]
                if insts < var:
                    cls.File.write(str(cls.Insts) + "\n")
                    cls.PrintInsts.pop(0)
                else:
                    cls.File.write(str(cls.VarsMax) + "\n")
                    cls.PrintVars.pop(0)
        elif cls.PrintInsts == None and cls.PrintVars != None:
            for i in cls.PrintVars:
                cls.File.write(str(cls.VarsMax) + "\n")
        elif cls.PrintInsts != None and cls.PrintVars == None:
            for i in cls.PrintInsts:
                cls.File.write(str(cls.Insts) + "\n")
        cls.File.close()



class Args(Error):
    """Class taking care of program argument loading and checking"""
    
    ArgErrMsg = "Argument error: Wrong set of arguments.\nSee --help for more information."
    FileErrMsg = "Input error: Couldn't open a file from program arguments."
    ProgramInput = dict()
    
    @classmethod   
    def GetArgs(cls):
        """Method gets the program arguments, checks their format and combination an return usable input data"""
        #cannot use getopts or others because i want the option to repear the arguments
        args = [argv.split('=') for argv in sys.argv[1:]]
        for arg in args:
            if len(arg) == 1:
                arg.append(None)
        argsKeys = [arg[0] for arg in args]
        #check not permitted
        permitted = ["--help", "--source", "--input", "--stats", "--insts", "--vars"]
        for arg in argsKeys:
            if arg not in permitted:
                cls.Error(10, cls.ArgErrMsg)
        #only insts and vars can be multiple time
        if(argsKeys.count("--help") > 1 or argsKeys.count("--source") > 1 or
           argsKeys.count("--input") > 1 or argsKeys.count("--stats") > 1):
            cls.Error(10, cls.ArgErrMsg)
        #at least source/input is required
        if len(args) == 0:
            cls.Error(10, cls.ArgErrMsg)
        #process
        args = dict(args)
        if len(args) == 1:
            if "--help" in args and args["--help"] == None: #cannot have any tail e.g. --help=something cannot pass
                print("This program interprets an XML representation of IPPcode20 language and outputs the result from the interpreted program.")
                print("Program options:")
                print("--help         Prints this info message. Cannot be combined with other parameters")
                print("--source=file  Defines the location of XML file to be interpreted")
                print("--input=file   Defines the location of file with standard input for the interpreted program")
                print("--stats=file   Defines the location of file to write the statistics into")
                print("--vars         Defines if the statistics count total initialized variables")
                print("--insts        Defines if the statistics count total number executed instructions")
                print("Options --vars and --insts cannot go without --stats and are the only one that can repeat themselves")
                print("All of the options individually are optional, although at least one of --source or --input is required and the input of the missing one will be awaited in standard input")
                sys.exit(0)
            elif "--source" in argsKeys:
                cls.ProgramInput["src"] = cls.LoadFile(args["--source"])
                cls.ProgramInput["in"] = sys.stdin.readlines()
            elif "--input" in argsKeys:
                cls.ProgramInput["in"] = cls.LoadFile(args["--input"], True)
                cls.ProgramInput["src"] = sys.stdin.read()
            else:
                cls.Error(10, cls.ArgErrMsg)
        else:
            if("--help" in argsKeys or                                                           #--help only alone
               "--stats" not in argsKeys and ("--insts" in argsKeys or "--vars" in argsKeys) or  #--stats required for other stats options
               "--source" not in argsKeys and "--input" not in argsKeys):                      #at least one of source/input must be present
                cls.Error(10, cls.ArgErrMsg)
            #load input
            if "--source" in argsKeys and "--input" in argsKeys:
                cls.ProgramInput["src"] = cls.LoadFile(args["--source"])
                cls.ProgramInput["in"] = cls.LoadFile(args["--input"], True)
            elif "--source" in argsKeys:
                cls.ProgramInput["src"] = cls.LoadFile(args["--source"])
                cls.ProgramInput["in"] = sys.stdin.readlines()    
            else:
                cls.ProgramInput["in"] = cls.LoadFile(args["--input"], True)
                cls.ProgramInput["src"] = sys.stdin.read()
            #stats
            if "--stats" in argsKeys:
                Stats.Print = True
                try:
                    Stats.File = open(args["--stats"], "w")
                except:
                    cls.Error(12, "File error: Couldn't open output --stats file.")
                #these can be multiple times
                if "--insts" in argsKeys:
                    if args["--insts"] != None:
                        cls.Error(10, cls.ArgErrMsg)
                    Stats.PrintInsts = Args.FindIndexes(argsKeys, "--insts")
                if "--vars" in argsKeys:
                    if args["--vars"] != None:
                        cls.Error(10, cls.ArgErrMsg)
                    Stats.PrintVars = Args.FindIndexes(argsKeys, "--vars")
    
    @classmethod   
    def LoadFile(cls, path, isStdin = False):
        """Loads contents of a file and returns it in a file object""" 
        try:
            with open(path) as f:
                file = f.readlines() if isStdin else f.read()
                return file
        except:
            cls.Error(11, cls.FileErrMsg)

    @staticmethod
    def FindIndexes(listToSearch, occurence):
        """Finds all indexes of given item in the list"""
        indexes = list()
        startSearch = 0
        while True:
            try:
                i = listToSearch.index(occurence, startSearch)
            except:
                return indexes
            startSearch = i+1
            indexes.append(i)



class State:
    """Reflects the current state of the application and changes it according to the situation
       Stores variables, labels, current frames an changes their accessibility"""
    
    # frame : dictionary of variables name:Variable
    Vars = {Const.GF: dict(), Const.LF: None, Const.TF: None}
    # name : number or the instruction to jump to
    Labels = dict()
    # list of dicts of template:   type (LF/TF) : list of variables
    FrameStack = list()
    # number of currently executed instruction (order attribute)
    InstructionNumber = 1
    # overall insruction count
    InstructionCount = 0
    # are we in a function?
    CallStack = list()
    # data stack of Constant types for pushs and pops instrucitons
    DataStack = list()



class Variable():
    """Simple class representing a variable ... name + type + value"""
    
    def __init__(self, name, varType=None, val=None):
        self.Name = name
        self.Type = varType
        self.Value = val



class Constant():
    """Simple class representing a constant value ... type + value"""
    
    def __init__(self, valType, val):
        self.Type = valType
        self.Value = val
 


class Executor(Error):
    """Contains a method for each instruction which implements its execution"""
        
    @staticmethod
    def CreateFrame():
        """Creates a temporary frame TF"""
        Stats.FrameDestroyed(State.Vars[Const.TF])
        State.Vars[Const.TF] = dict()
        
    @staticmethod
    def PushFrame():
        """Pushes the temporary frame TF to the top of the local frames stack"""
        if State.Vars[Const.TF] == None:
            Executor.Error(55, "Error: No temporary frame to push.")
        #safe current local
        if State.Vars[Const.LF] != None:
            State.FrameStack.append(State.Vars[Const.LF])
        #put TF to LF
        State.Vars[Const.LF] = State.Vars[Const.TF]
        State.Vars[Const.TF] = None
        
    @staticmethod
    def PopFrame():
        """Pops the top local frame from the stack and makes a temporary one out of it"""
        if State.Vars[Const.LF] == None:
            Executor.Error(55, "Error: No local frame to pop.")
        #put LF to TF
        Stats.FrameDestroyed(State.Vars[Const.TF])
        State.Vars[Const.TF] = State.Vars[Const.LF]
        State.Vars[Const.LF] = None
        #put previous LF to current LF
        if len(State.FrameStack) > 0:
            State.Vars[Const.LF] = State.FrameStack.pop()
        
    @staticmethod
    def Return():
        """Moves the program execution behind the CALL instruction which put us into this function"""
        if len(State.CallStack) == 0:
            Executor.Error(56, "Runtime error: Cannot use return instruction if not in a function.")
        State.InstructionNumber = State.CallStack.pop()
    
    @staticmethod
    def Break():
        """Prints to stderr the current program state"""
        lines = list()
        lines.append("Currently processed instruction number: " + str(State.InstructionNumber))
        lines.append("\nTotal instruction lines: " + str(State.InstructionCount))
        lines.append("\nTotal instructions executed: " + str(Stats.Insts))
        lines.append("\nFrame stack:")
        GF = LF = TF = ""
        for gf in State.Vars[Const.GF].values():
            GF += "{" + "{0}:{1} ({2})".format(gf.Name, gf.Value, gf.Type) + "}, "
        lines.append("\nGF: " + GF[0:-2])
        if State.Vars[Const.LF] != None:
            for lf in State.Vars[Const.LF].values():
                LF += "{" + "{0}:{1} ({2})".format(lf.Name, lf.Value, lf.Type) + "}, "
        lines.append("\nLF: " + LF[0:-2])
        if State.Vars[Const.TF] != None:
            for tf in State.Vars[Const.TF].values():
                TF += "{" + "{0}:{1} ({2})".format(tf.Name, tf.Value, tf.Type) + "}, "
        lines.append("\nTF: " + TF[0:-2])
        lines.append("\nCall stack:\n")
        lines.append(str(State.CallStack))
        lines.append("\nData stack:\n")
        lines.append(str(State.DataStack) + "\n")
        sys.stderr.writelines(lines)
        
    @staticmethod
    def Defvar(var):
        """Takes an xml element representing a variable and defines it"""
        frame, var = Decoder.GetVar(var, 1)
        if State.Vars[frame] == None:
            Executor.Error(55, "Error: Trying to define a variable in nonexitstent frame.")
        if var.Name in State.Vars[frame]:
            Executor.Error(52, "Error: Variable " + var.Name + " has already been defined in " + frame + " frame.")
        State.Vars[frame][var.Name] = var
        
    @staticmethod
    def Call(label):
        """Jumps to the given label and stores the instruction's incremented number to the callstack"""
        State.CallStack.append(State.InstructionNumber)
        Executor.Jump(label)
        
    @staticmethod
    def Pushs(symb):
        """Takes a variable/constant and pushes it's value (Constant object) to the data stack"""
        State.DataStack.append(symb)
        
    @staticmethod
    def Pops(var):
        """Stores the value on the top of the data stack to the given variable"""
        try:
            val = State.DataStack.pop()
        except:
            Executor.Error(56, "Error: The data stack is empty.")
        Executor.Move(var, val)

    @staticmethod
    def Write(symb):
        """Writes the symbols value to stdout"""
        # nil@nil as empty string
        if symb.Type == Const.Null:
            print('', end='')
        # boolean as true/false and not True/False like Python would have
        elif symb.Type == Const.Bool:
            toPrint = "true" if symb.Value == True else "false"
            print(toPrint, end='')
        else: # otherwise normal
            print(symb.Value, end='')
        
    @staticmethod
    def Jump(label):
        """Jumps to the given label by changing the currently executed intruction number"""
        if label not in State.Labels:
            Executor.Error(52, "Error: Label " + label + " doesn't exist.")
        State.InstructionNumber = State.Labels[label]
        Stats.Insts += 1 #by jumping we're skipping the label instruction but it should be counted
        
    @staticmethod
    def Exit(symb):
        """Ends the execution of the program with given exit code"""
        # needs to be 0-49, co integer
        exitCode = symb.Value
        if symb.Type != Const.Int or exitCode < 0 or exitCode > 49:
            Executor.Error(57, "Error: Invalid exit code.")
        sys.exit(exitCode)
        
    @staticmethod
    def Dprint(symb):
        """Prints to stderr the value of the given symbol"""
        sys.stderr.write(str(symb.Value))
        
    @staticmethod
    def Move(var, symb):
        """Moves the value of the given symbol to the variable"""
        var.Type = symb.Type
        var.Value = symb.Value
        
    @staticmethod
    def Not(var, symb):
        """Takes a boolean symbol which it negates and stores it to the variable"""
        if symb.Type != Const.Bool:
            Executor.Error(53, "Error: Expected operand of type 'bool'.")
        var.Type = Const.Bool
        var.Value = not symb.Value
        
    @staticmethod
    def Int2Char(var, symb):
        """Converts integer value form given symbol to a one character string which it stores to the variable"""
        if symb.Type != Const.Int:
            Executor.Error(53, "Error: Expected operand of type 'int'.")
        try:
            var.Type = Const.String
            var.Value = chr(symb.Value)
        except:
            Executor.Error(58, "Error: Unicode number out of boundaries.")    
            
    @staticmethod
    def Read(var, readType):
        """Reads from standard input a value of given type and stores it to the variable"""
        #check types
        if readType == Const.Null: #can be only int, string, bool
            Executor.Error(53, "Error: Invaid operand type.")
        #read input
        if len(Args.ProgramInput["in"]) > 0:
            inputVal = Args.ProgramInput["in"][0].strip() #get rid of the trailing 'newline' as input() would have
            Args.ProgramInput["in"].pop(0)                #remove the read one
        else:
            inputVal = None
        #convert
        var.Type = readType
        try:
            if readType == Const.Int:
                var.Value = int(inputVal)
            elif readType == Const.String:
                var.Value = inputVal
            else: #bool
                var.Value = inputVal.lower() == "true"
            #check null input
            if inputVal == None:
                raise Exception() #it's gonna be nil@nil if either conversion fails or nothing was read
        except:
            var.Type = Const.Null
            var.Value = None
              
    @staticmethod
    def Strlen(var, symb):
        """Stores the length of string value in symbol to the variable"""
        if symb.Type != Const.String:
            Executor.Error(53, "Error: Expected operand of type 'string'.")
        var.Type = Const.Int
        var.Value = len(symb.Value)
            
    @staticmethod
    def Type(var, symb):
        """Stores the symbol's type as the variable's value."""
        var.Type = Const.String
        var.Value = "" if symb.Type == None else symb.Type
        
    @staticmethod
    def Add(var, symb1, symb2):
        """Adds the integer values in given symbols and stores the result into the variable"""
        Executor.CheckArithmeticArgs(var, symb1, symb2)
        var.Value = symb1.Value + symb2.Value
        
    @staticmethod
    def Sub(var, symb1, symb2):
        """Subtracts the integer values in given symbols and stores the result into the variable"""
        Executor.CheckArithmeticArgs(var, symb1, symb2)
        var.Value = symb1.Value - symb2.Value
        
    @staticmethod
    def Mul(var, symb1, symb2):
        """Multiplies the integer values in given symbols and stores the result into the variable"""
        Executor.CheckArithmeticArgs(var, symb1, symb2)
        var.Value = symb1.Value * symb2.Value
        
    @staticmethod
    def Idiv(var, symb1, symb2):
        """Divides the integer values in given symbols and stores the result as integer into the variable"""
        Executor.CheckArithmeticArgs(var, symb1, symb2)
        if symb2.Value == 0:
            Executor.Error(57, "Error: Zero division error.")
        var.Value = symb1.Value // symb2.Value
        
    @staticmethod
    def Lt(var, symb1, symb2):
        """Compares two values of type int/string/bool of given as symbols in parameters and stores the boolean result into the variable"""
        if symb1.Type != symb2.Type or symb1.Type == Const.Null:
            Executor.Error(53, "Error: Invalid operand type.")
        var.Type = Const.Bool
        var.Value = symb1.Value < symb2.Value
        
    @staticmethod
    def Gt(var, symb1, symb2):
        """Compares two values of type int/string/bool of given as symbols in parameters and stores the boolean result into the variable"""
        Executor.Lt(var, symb1, symb2)
        #it's negation of LT instruction, BUT need to check if the symbols aren't equal
        var.Value = True if var.Value == False and symb1.Value != symb2.Value else False
        
    @staticmethod
    def Eq(var, symb1, symb2):
        """Compares the given symbols and stores boolean True to the variable if the are equal"""
        var.Type = Const.Bool
        #we can compare with 'nil' values here
        if symb1.Type == Const.Null and symb2.Type == Const.Null:
            var.Value = True
        elif symb1.Type == Const.Null or symb2.Type == Const.Null:
            var.Value = False
        elif symb1.Type != symb2.Type: #if none is null, they must be of the same type
            Executor.Error(53, "Error: Invalid operand type.")
        else:
            var.Value = symb1.Value == symb2.Value
        
    @staticmethod
    def And(var, symb1, symb2):
        """Applies the logical AND (conjunction) on the boolean symbols and the boolean result stores to the variable"""
        Executor.CheckLogicalArgs(var, symb1, symb2)
        var.Value = symb1.Value and symb2.Value
        
    @staticmethod
    def Or(var, symb1, symb2):
        """Applies the logical OR (disjunction) on the boolean symbols and the boolean result stores to the variable"""
        Executor.CheckLogicalArgs(var, symb1, symb2)
        var.Value = symb1.Value or symb2.Value
        
    @staticmethod
    def Stri2Int(var, symb1, symb2):
        """To the variable stores the ordinal Unicode value of a character from symb1 string on symb2 position"""
        if symb1.Type != Const.String or symb2.Type != Const.Int:
            Executor.Error(53, "Error: Invalid operand type.")
        if symb2.Value < 0:
            Executor.Error(58, "Error: Negative index.")
        try:
            var.Type = Const.Int
            char = symb1.Value[symb2.Value]
            var.Value = ord(char)
        except:
            Executor.Error(58, "Error: Index out of boundaries of a string.")
        
    @staticmethod
    def Concat(var, symb1, symb2):
        """Concatenates strings from given symbols and stores the result to the variable"""
        if symb1.Type != Const.String or symb2.Type != Const.String:
            Executor.Error(53, "Error: Invalid operand type.")
        var.Type = Const.String
        var.Value = symb1.Value + symb2.Value
        
    @staticmethod
    def GetChar(var, symb1, symb2):
        """To the variable stores a character from symb1 string on symb2 position"""
        if symb1.Type != Const.String or symb2.Type != Const.Int:
            Executor.Error(53, "Error: Invalid operand type.")
        if symb2.Value < 0:
            Executor.Error(58, "Error: Negative index.")
        try:
            var.Type = Const.String
            var.Value = symb1.Value[symb2.Value]
        except:
            Executor.Error(58, "Error: Index out of boundaries of a string.")
        
    @staticmethod
    def SetChar(var, symb1, symb2):
        """Puts a character from symb2 in the symb1 position of the string value of the variable"""
        if var.Type != Const.String or symb1.Type != Const.Int or symb2.Type != Const.String:
            Executor.Error(53, "Error: Invalid operand type.")
        if len(symb2.Value) == 0:
            Executor.Error(58, "Error: Invalid string value.")
        if symb1.Value < 0:
            Executor.Error(58, "Error: Negative index.")
        try:
            tmp = list(var.Value)
            tmp[symb1.Value] = symb2.Value[0]
            var.Value = "".join(tmp)
        except:
            Executor.Error(58, "Error: Index out of boundaries of a string.")
        
    @staticmethod
    def JumpIfEq(label, symb1, symb2):
        """If the values of given symbols equal, jumps to the given label"""
        tmp = Variable("tmp", Const.Bool)
        Executor.Eq(tmp, symb1, symb2)
        if tmp.Value:
            Executor.Jump(label)
        
    @staticmethod
    def JumpIfNotEq(label, symb1, symb2):
        """If the values of given symbols don't equal, jumps to the given label"""
        tmp = Variable("tmp", Const.Bool)
        Executor.Eq(tmp, symb1, symb2)
        if tmp.Value == False:
            Executor.Jump(label)
    
    @staticmethod
    def CheckArithmeticArgs(var, symb1, symb2):
        """Checks the correct format of operands of the arithmetic instructions"""
        if symb1.Type != Const.Int or symb2.Type != Const.Int:
            Executor.Error(53, "Error: Arithmetic instructions require integer operands.")
        var.Type = Const.Int

    @staticmethod
    def CheckLogicalArgs(var, symb1, symb2):
        """Checks the funciton parameters for the logical AND and OR instructions"""
        if symb1.Type != symb2.Type or symb1.Type != Const.Bool:
            Executor.Error(53, "Error: Invalid operand type.")
        var.Type = Const.Bool


        
class Decoder(Error):
    """Recognizes instructions and arguments, checks corrects formats"""
    
    #instructions
    # 0 params
    Instr_with_0_params = ["CREATEFRAME", "PUSHFRAME", "POPFRAME", "RETURN", "BREAK"]
    # 1 param
    Instr_with_1_params = ["DEFVAR", "CALL", "PUSHS", "POPS", "WRITE", "LABEL", "JUMP", "EXIT", "DPRINT"]
    # 2 params
    Instr_with_2_params = ["MOVE", "NOT", "INT2CHAR", "READ", "STRLEN", "TYPE"]
    # 3 params ... <var> <symb1> <symb2>   vs.   <label> <symb1> <symb2>
    Instr_with_3_params_1 = ["ADD", "SUB", "MUL", "IDIV", "LT", "GT", "EQ", "AND", "OR", "STRI2INT", "CONCAT", "GETCHAR", "SETCHAR"]
    Instr_with_3_params_2 = ["JUMPIFEQ", "JUMPIFNEQ"]
    
    @classmethod
    def Decode(cls, instruction):
        """Decodes given instruction, checks its format and executes it"""
        #sort argumes ... arg3, arg1, arg2 is valid argument sequence
        instruction[:] = sorted(instruction, key=lambda child: child.tag)
        #firstly, it really needs to be an instruction
        if instruction.tag != "instruction":
            cls.Error(32, "Xml error: Expected instruction element")
        #check the correct format
        attr = instruction.attrib
        if len(attr) != 2 or ("order" not in attr and "opcode" not in attr):
            cls.Error(32, "Xml error: Instructions must have exactly two attrributes ... order and opcode")
        #check any non-xml characters (non children) in its text area
        if (instruction.text != None and instruction.text.strip() != '') or (instruction.tail != None and instruction.tail.strip() != ''):
            cls.Error(31, "Xml error: Non xml element detected")
        
        #now go according to number of arguments
        wrongArgNumErrMsg = "Xml error: Unknown instruction or incorrect number of instruction's child elements"
        opcode = attr["opcode"].upper()        
        if len(instruction) == 0 and opcode in cls.Instr_with_0_params:
            if opcode == "CREATEFRAME":
                Executor.CreateFrame()
            elif opcode == "PUSHFRAME":
                Executor.PushFrame()
            elif opcode == "POPFRAME":
                Executor.PopFrame()
            elif opcode == "RETURN":
                Executor.Return()
            else: #opcode == "BREAK"
                Executor.Break()
                
        elif len(instruction) == 1 and opcode in cls.Instr_with_1_params:
            arg = instruction[0]
            #get symbol argument for those that need it
            if opcode == "PUSHS" or opcode == "WRITE" or opcode == "EXIT" or opcode == "DPRINT":
                symb = cls.GetSymb(arg, 1)
            #get label argument for those that need it
            if opcode == "CALL" or opcode == "LABEL" or opcode == "JUMP":
                label = cls.GetLabelVal(arg) 
            
            if opcode == "DEFVAR":
                Executor.Defvar(arg)
            elif opcode == "CALL":
                Executor.Call(label)
            elif opcode == "PUSHS":
                Executor.Pushs(symb)
            elif opcode == "POPS":
                frame, var = cls.GetVar(arg, 1)
                cls.CheckVar(var.Name, frame)
                Stats.UninitVar(var)
                Executor.Pops(var)
            elif opcode == "WRITE":
                Executor.Write(symb)
            elif opcode == "LABEL":
                pass #labels were registered at the beginning of the program
            elif opcode == "JUMP":
                Executor.Jump(label)
            elif opcode == "EXIT":
                Executor.Exit(symb)
            elif opcode == "DPRINT":
                Executor.Dprint(symb)
            else:
                cls.Error(32, wrongArgNumErrMsg)    
                
        elif len(instruction) == 2 and opcode in cls.Instr_with_2_params:
            #all have a variable as destination operand
            frame, var = cls.GetVar(instruction[0], 1)
            cls.CheckVar(var.Name, frame)
            Stats.UninitVar(var)
            #all but READ have symb as the second operand
            if opcode == "READ":
                readType = cls.GetType(instruction[1])
            else: # TYPE instruction is the only one that can work with uninitialized symbol, need to check that 
                symb = cls.GetSymb(instruction[1], 2, True) if opcode == "TYPE" else cls.GetSymb(instruction[1], 2)
            
            if opcode == "MOVE":
                Executor.Move(var, symb)
            elif opcode == "NOT":
                Executor.Not(var, symb)
            elif opcode == "INT2CHAR":
                Executor.Int2Char(var, symb)
            elif opcode == "READ":
                Executor.Read(var, readType)
            elif opcode == "STRLEN":
                Executor.Strlen(var, symb)
            elif opcode == "TYPE":
                Executor.Type(var, symb)
            else:
                cls.Error(32, wrongArgNumErrMsg)  
        
        elif len(instruction) == 3:
            # 2. and 3. arg are the same for all
            if opcode in cls.Instr_with_3_params_1 or opcode in cls.Instr_with_3_params_2:
                symb1 = cls.GetSymb(instruction[1], 2)
                symb2 = cls.GetSymb(instruction[2], 3)
            else:
                cls.Error(32, wrongArgNumErrMsg)
            
            if opcode in cls.Instr_with_3_params_1:
                #first get the destination variable
                frame, var = cls.GetVar(instruction[0], 1)
                cls.CheckVar(var.Name, frame)
                Stats.UninitVar(var)
                #execute function
                if opcode == "ADD":
                    Executor.Add(var, symb1, symb2)
                elif opcode == "SUB":
                    Executor.Sub(var, symb1, symb2)
                elif opcode == "MUL":
                    Executor.Mul(var, symb1, symb2)
                elif opcode == "IDIV":
                    Executor.Idiv(var, symb1, symb2)
                elif opcode == "LT":
                    Executor.Lt(var, symb1, symb2)
                elif opcode == "GT":
                    Executor.Gt(var, symb1, symb2)
                elif opcode == "EQ":
                    Executor.Eq(var, symb1, symb2)
                elif opcode == "AND":
                    Executor.And(var, symb1, symb2)
                elif opcode == "OR":
                    Executor.Or(var, symb1, symb2)
                elif opcode == "STRI2INT":
                    Executor.Stri2Int(var, symb1, symb2)
                elif opcode == "CONCAT":
                    Executor.Concat(var, symb1, symb2)
                elif opcode == "GETCHAR":
                    Executor.GetChar(var, symb1, symb2)
                else: # opcode == "SETCHAR":
                    Executor.SetChar(var, symb1, symb2)
            
            else:
                #first get the label to jump to
                label = cls.GetLabelVal(instruction[0])
                if label not in State.Labels:
                    cls.Error(52, "Error: Undefined label.")
                #execute jumps
                if opcode == "JUMPIFEQ":
                    Executor.JumpIfEq(label, symb1, symb2)
                else: # opcode == "JUMPIFNEQ":
                    Executor.JumpIfNotEq(label, symb1, symb2) 
        else:
            cls.Error(32, wrongArgNumErrMsg)
        
    @classmethod
    def CheckArgSyntax(cls, elmt, argNumber):
        """Takes an XML element representing an argument and checks its xml syntax"""
        #it has to be an agr element
        if elmt.tag != "arg"+str(argNumber):
            cls.Error(32, "Xml error: Unexpected xml element. An arg"+str(argNumber)+" expected.")
        #it needs to have a type attribute
        if "type" not in elmt.attrib:
            cls.Error(32, "Xml error: Missing argument type.")
        #only type attribute is allowed
        if len(elmt.attrib) != 1:
            cls.Error(32, "Xml error: Unknown attribute of an instruction argument element.")
        #check any non-xml characters in between
        if elmt.tail != None and elmt.tail.strip() != '':
            cls.Error(31, "Xml error: Non xml element detected.")
        #arguments cannot have children
        if len(elmt) > 0:
            cls.Error(32, "Xml error: Arguments cannot have children.")
    
    @classmethod
    def GetSymb(cls, elmt, argNumber, isTypeInstr = False):
        """Takes an XML element representing a constant or a variable and returns the constant value of the symbol"""
        Decoder.CheckArgSyntax(elmt, argNumber)
        #var or constant?
        if elmt.attrib["type"] == Const.Var:
            frame, var = cls.GetVar(elmt, argNumber, False)
            #check if the variable exists in current frame
            cls.CheckVar(var.Name, frame)
            #check if it is initialized ... X   TYPE instruction works with uninit as well, need to check that
            if var.Type == None and isTypeInstr == False:
                cls.Error(56, "Error: Trying to use an uninitialized variable.")
            return Constant(var.Type, var.Value)
        else:                           
            return cls.GetConst(elmt)
        
    @classmethod
    def GetVar(cls, elmt, argNumber, checkElmtSyntax=True):
        """Takes an XML element representing a variable and returns it as an python object in a tuple with it's frame"""
        #if called from GetSymb() it will already be checked, if it's standalone, need to chek it
        if checkElmtSyntax == True:
            Decoder.CheckArgSyntax(elmt, argNumber)
            if elmt.attrib["type"] != Const.Var:
                cls.Error(53, "Xml error: Invalid operand type in xml.")
        #now check the frame and correct name
        regex = re.compile(r'^(LF|GF|TF)@(_|-|\$|&|%|\*|[a-zA-Z])(_|-|\$|&|%|\*|[0-9a-zA-Z])*$')
        varName = elmt.text.strip()
        if regex.match(varName) == None:
            cls.Error(32, "Xml error: Invalid format of a variable")
        frame, name = varName.split('@')
        #does the variable exist?
        var = State.Vars[frame][name] if State.Vars[frame] != None and name in State.Vars[frame] else Variable(name)
        #return as a tuple - frame:variable
        return (frame, var)
    
    @classmethod
    def CheckVar(cls, name, frame):
        """Checks the existence of the variables frame and the variable in that frame"""
        if State.Vars[frame] == None:     #check if the frame even exist
            cls.Error(55, "Error: Trying to access a variable in nonexitstent frame.")
        if name not in State.Vars[frame]: #check if the variable exists in current frame
            cls.Error(54, "Error: Undefined variable")
    
    @classmethod
    def GetConst(cls, elmt):
        """Takes an XML element representing a variable and returns it as an python object"""
        #check type validity
        elmt.text = '' if elmt.text == None else elmt.text.strip()
        if elmt.attrib["type"] == Const.Int:
            if re.search(r'^(\+|-)?[0-9]+$', elmt.text) == None:
                cls.Error(32, "Xml error: Invalid integer constant")
            val = int(elmt.text)
        elif elmt.attrib["type"] == Const.String:
            if re.search(r' ', elmt.text) != None or re.search(r'\\(?![0-9]{3})', elmt.text) != None:
                cls.Error(32, "Xml error: Invalid string constant")
            #decode ascii in string
            val = r"{0}".format(elmt.text)
            escapeSequences = list(set(re.findall(r"\\([0-9]{3})", val)))	
            for es in escapeSequences:
                # for some reason \092 == '\' is broken
                val = re.sub(r"\\092", r"\\", val) if es == "092" else re.sub(r"\\{0}".format(es), chr(int(es)), val)
            #decode <>&
            val = re.sub(r"&amp;", '&', val)
            val = re.sub(r"&lt;", '<', val)
            val = re.sub(r"&gt;", '>', val)
        elif elmt.attrib["type"] == Const.Bool:
            if re.search(r'^(true|false)$', elmt.text) == None:
                cls.Error(32, "Xml error: Invalid boolean constant")
            val = True if elmt.text == "true" else False
        elif elmt.attrib["type"] == Const.Null:
            if re.search(r'^nil$', elmt.text) == None:
                cls.Error(32, "Xml error: Invalid nil constant")
            val = None
        else:
            cls.Error(32, "Xml error: Invalid operand type in xml.")
        #return as tuple - None:value ...None to easily distinguished from Variable as it gets called from GetSymb()
        return Constant(elmt.attrib["type"], val)

    @classmethod
    def GetType(cls, elmt):
        """Takes and type xml argument, checks it and returns it's value"""
        cls.CheckArgSyntax(elmt, 2)
        if elmt.attrib["type"] != "type":
            cls.Error(53, "Error: Invalid instruction operand.")
        val = elmt.text.strip()
        if re.search(r'^(int|string|bool|nil)$', val) == None:
            cls.Error(31, "Xml error: Invalid type of type argument.")
        return val
            
    @classmethod
    def GetLabelVal(cls, elmt):
        """Takes an xml argument element, checks if it is a label and returns it"""
        cls.CheckArgSyntax(elmt, 1)
        if elmt.attrib["type"] == Const.Label:
            val = elmt.text.strip()
            if re.search(r'^(_|-|\$|&|%|\*|[a-zA-Z])(_|-|\$|&|%|\*|[0-9a-zA-Z])*$', val) == None:
                cls.Error(32, "Xml error: Invalid label name format.")
            return val
        #if it's valid but not label ... type error
        elif(elmt.attrib["type"] == Const.Int or elmt.attrib["type"] == Const.Bool or
             elmt.attrib["type"] == Const.String or elmt.attrib["type"] == Const.Null):
            cls.Error(53, "Error: Expected operand of type label.")
        #if it's not even valid type value ... xml error
        else:
            cls.Error(32, "Xml error: Unknown operand type.")
    
    @classmethod
    def IsLabel(cls, elmt):
        """Takes a correct xml instruction and determines if it is a label and returns it's value"""
        if elmt.attrib["opcode"] == "LABEL":
            if len(elmt) != 1:
                cls.Error(32, "Xml error: Invalid number instruction children.")
            label = cls.GetLabelVal(elmt[0])
            return label
        else:
            return None
        
    @classmethod
    def CheckInstruction(cls, elmt):
        """Takes an xml instruction and checks it's format"""
        #it has to be an instruction element
        if elmt.tag != "instruction":
            cls.Error(32, "Xml error: Unexpected xml element. An instruction expected")
        #it needs to have opcode and order attributes
        if len(elmt.attrib) != 2 or "opcode" not in elmt.attrib or "order" not in elmt.attrib:
            cls.Error(32, "Xml error: Instructions need to have exactly 2 attributes ... \"opcode\" and \"order\".")
        #check instruction order ... only format, actual value checks the caller
        try:
            order = int(elmt.attrib["order"])
        except:
            cls.Error(32, "Xml error: Invalid format instruction order attribute.")
        #check any non-xml characters in between
        if elmt.tail != None and elmt.tail.strip() != '':
            cls.Error(31, "Xml error: Non xml element detected")
        return order


   
class Interpret(Error):
    """This is the main class fo the program which directs its flow"""
    
    @classmethod
    def Run(cls):
        """Executes the program, directs all major parts of it"""
        #get and check program arguments
        Args.GetArgs()
        #parse and check input xml
        try:
            xmlRoot = XML.fromstring(Args.ProgramInput["src"])
        except:
            cls.Error(31, "Error: Incorrect format of XML in input file.")
        #check correct root element <program>
        cls.CheckRootElmt(xmlRoot)
        
        #check instruction's format, order them and store all labels
        orderdInstr = dict();
        State.InstructionCount = len(xmlRoot)
        for instr in xmlRoot:
            #check order bounderies and uniqueness ... there's no top boundary, 1,2,5,6,7 is valid -> order number can be higher than
            order = Decoder.CheckInstruction(instr)
            if order < 1 or order in orderdInstr:
                cls.Error(32, "Xml error: Invalid instruction order")
            orderdInstr[order] = instr
            #find labels
            label = Decoder.IsLabel(instr)
            if label != None:
                if label in State.Labels:
                    cls.Error(52, "Error: Label " + label + " already defined")
                else:
                    State.Labels[label] = order
                
        #run the interpretation
        while State.InstructionNumber <= max(orderdInstr):
            if State.InstructionNumber not in orderdInstr:
                State.InstructionNumber += 1
                continue
            instr = orderdInstr[State.InstructionNumber]
            Decoder.Decode(instr)
            State.InstructionNumber += 1
            Stats.Insts += 1
        
        Stats.Out()
    
    @classmethod
    def CheckRootElmt(cls, root):
        """Check the correct format of the root <program> XML element"""
        errMsg = "Xml error: Incorrect format of root <program> element."
        #root element must be <program> and max 3 attributes are allowed
        if root.tag != "program" or len(root.attrib) > 3:
            cls.Error(32, errMsg)
        #language attribute is required and it must have value of "IPPcode20"
        if "language" not in root.attrib or root.attrib["language"] != "IPPcode20":
            cls.Error(32, errMsg)
        #name and description optional attributes
        if len(root.attrib) == 2 and not ("name" in root.attrib or "description" in root.attrib):
            cls.Error(32, errMsg)
        if len(root.attrib) == 3 and not ("name" in root.attrib and "description" in root.attrib):
            cls.Error(32, errMsg)
        #invalid character between tags
        if root.text.strip() != '' or (root.tail != None and root.tail.strip() != ''):
            cls.Error(31, "xml error: Unexpected character in xml structure.")

Interpret.Run()