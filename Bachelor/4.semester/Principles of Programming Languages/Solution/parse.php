<?php
/* 
 *  File: parse.php
 *  Solution: IPP project 2019/2020
 *  Date: 10.2.2020
 *  Author: Jan Lorenc (xloren15)
 *  Faculty: Faculty of Information Technology VUT
 *  Description: This script parses language IPPcode20 to it's XML representation
 */

/* Constants for the types of instruction arguments */
const SYMB = "symb";
const VARIABLE = "var";
const LABEL = "label";
const TYPE = "type";

/** 
 * Class for parsing ... de facto main() of the program, it directs everything
 */
class Parser
{
    //attributes
    private $stats;          //statistics object
    private $args;           //object representing script arguments
    private $xml;            //xml writer object
    private $inputFile;      //we load the whole stdin safely in UTF8 ... loading by fgets didn't do it even after changing shell settings
    private $missingHeader;  //defines whether the header of IPPcode20 has been loaded
    
    /**
     * Creates instances of the necesary classes
     */
    function __construct()
    {
        $this->stats = new Stats(); 
        $this->args = new Args();     
        $this->xml = new XmlStream();
        $this->missingHeader = true;
    }

    /**
     * De facto main() of this script, handles the progrom flow
     */
    public function Run()
    {
        //argument check
        global $argc;
        $this->args->CheckArgs($this->stats, $argc-1);
        //reading and processing input
        try
        {
            $this->inputFile = file_get_contents('php://stdin');
        }
        catch(Exception $e)
        {
            $this->Error(11, "Error: Couldn't read the input!\n");
        }
        $this->inputFile = explode(PHP_EOL, $this->inputFile);
        foreach($this->inputFile as $line)
        {
            $rawInstruction = $this->CheckLineContent($line);
            if($rawInstruction == null)
                continue;
            $instruction = $this->ClassifyInstruction($rawInstruction);
            $this->stats->InstructionsUp();
            $this->xml->In($instruction);
        }
        //the input file could be empty / contains only white chars and comments -> header error
        if($this->missingHeader)
           $this->Error(21, "Missing the header line in the input file!\n");
        //all seems to be alright -> print the result
        $this->xml->Out();
        $this->stats->Out($this->args);
    }

    /**
     * Validates the input line
     * Cuts out comments, empty lines, superfluous whitespaces
     * @param line The line currently being processed
     * @return null on loading a line not containing an instruction or instruction pieces in an array
     */
    private function CheckLineContent($line)
    {
        //check for a comment on the line
        if(preg_match('/#/', $line))
        {
            $line = explode('#', $line, 2)[0]; //save only the part before the comment (cut the comment out)
            $this->stats->CommentsUp();
        }
        //remove superfluous white spaces
        $line = trim($line);                             //around the string
        $line = preg_replace('/(\s|\t)+/', ' ', $line);  //inside the string
        //check for an empty line
        if($line == "")
            return null;

        /*now we have a line of IPPcode20 code*/
        //check the header line
        if($this->missingHeader)
        {
            if(!preg_match('/\.ippcode20/', strtolower($line)))
                $this->Error(21, "Incorrect format of the header line in the input file!\n");
            else
                $this->missingHeader = false;
            return null;
        }
        //cut the line on instruction parts and check the counts
        $instructionParts = explode(' ', $line);
        $partsCount = count($instructionParts);
        if($partsCount < 1 || $partsCount > 4)
            $this->Error(23, "Wrong number of instruction parameters!\nUnrecognized code: ".$line."\n");
        return $instructionParts;
    }

    /**
     * Determines the type of the instruction (checks the op. code)
     * and creates a fully defined Instruction object accordingly
     * @param parts Pieces of the instruction to be put together in an object
     * @return Instance of an Instruction object
     */
    private function ClassifyInstruction($parts)
    {
        $instructionName = strtoupper($parts[0]);
        $errMsg = "Wrong number of instruction parameters!\nUnrecognized code: ".implode(" ",$parts)."\n";
        switch($instructionName)
        {
            case "MOVE":
            case "INT2CHAR":
            case "STRLEN":
            case "TYPE":
            case "NOT":
                if(count($parts) != 3)
                    $this->Error(23, $errMsg);
                $instruction = new Instruction($instructionName, $this->stats->GetInstructions());
                $instruction->AddArgument(VARIABLE, $parts[1]);
                $instruction->AddArgument(SYMB, $parts[2]);
                return $instruction;
            case "CREATEFRAME": case "PUSHFRAME": case "POPFRAME":
            case "BREAK":
                if(count($parts) != 1)
                    $this->Error(23, $errMsg);
                return new Instruction($instructionName, $this->stats->GetInstructions());
            case "DEFVAR":
                if(count($parts) != 2)
                    $this->Error(23, $errMsg);
                $instruction = new Instruction($instructionName, $this->stats->GetInstructions());
                $instruction->AddArgument(VARIABLE, $parts[1]);
                return $instruction;
            case "CALL":
            case "JUMP":
                if(count($parts) != 2)
                    $this->Error(23, $errMsg);
                $instruction = new Instruction($instructionName, $this->stats->GetInstructions());
                $instruction->AddArgument(LABEL, $parts[1]);
                $this->stats->JumpsUp();
                return $instruction;
            case "RETURN":
                if(count($parts) != 1)
                    $this->Error(23, $errMsg);
                $this->stats->JumpsUp();
                return new Instruction($instructionName, $this->stats->GetInstructions());
            case "PUSHS":
            case "WRITE":
            case "EXIT":
            case "DPRINT":
                if(count($parts) != 2)
                    $this->Error(23, $errMsg);
                $instruction = new Instruction($instructionName, $this->stats->GetInstructions());
                $instruction->AddArgument(SYMB, $parts[1]);
                return $instruction;
            case "POPS":
                if(count($parts) != 2)
                    $this->Error(23, $errMsg);
                $instruction = new Instruction($instructionName, $this->stats->GetInstructions());
                $instruction->AddArgument(VARIABLE, $parts[1]);
                return $instruction;
            case "ADD": case "SUB": case "MUL": case "IDIV":
            case "LT": case "GT": case "EQ":
            case "AND": case "OR":
            case "STRI2INT":
            case "CONCAT": case "GETCHAR": case "SETCHAR": case "IDIV":
                if(count($parts) != 4)
                    $this->Error(23, $errMsg);
                $instruction = new Instruction($instructionName, $this->stats->GetInstructions());
                $instruction->AddArgument(VARIABLE, $parts[1]);
                $instruction->AddArgument(SYMB, $parts[2]);
                $instruction->AddArgument(SYMB, $parts[3]);
                return $instruction;
            case "READ":
                if(count($parts) != 3)
                    $this->Error(23, $errMsg);
                $instruction = new Instruction($instructionName, $this->stats->GetInstructions());
                $instruction->AddArgument(VARIABLE, $parts[1]);
                $instruction->AddArgument(TYPE, $parts[2]);
                return $instruction;
            case "LABEL":
                if(count($parts) != 2)
                    $this->Error(23, $errMsg);
                $instruction = new Instruction($instructionName, $this->stats->GetInstructions());
                $instruction->AddArgument(LABEL, $parts[1]);
                $this->stats->LabelsUp($parts[1]);
                return $instruction;
            case "JUMPIFEQ": case "JUMPIFNEQ":
                if(count($parts) != 4)
                    $this->Error(23, $errMsg);
                $instruction = new Instruction($instructionName, $this->stats->GetInstructions());
                $instruction->AddArgument(LABEL, $parts[1]);
                $instruction->AddArgument(SYMB, $parts[2]);
                $instruction->AddArgument(SYMB, $parts[3]);
                $this->stats->JumpsUp();
                return $instruction;
            default: //instruction not recognized
                $this->Error(22, "Operational code not recognized!\n");
        }
    }

    /**
     * Ends program when an error occures
     * @param errCode Value to be returned by the script (error code)
     * @param msg Message to be displayed to the user
     */
    private function Error($errCode, $msg)
    {
        fprintf(STDERR, $msg);
        exit($errCode);
    }
}

/** 
 * Class representing an instruction 
 */
class Instruction
{
    //attributes
    private $name;  //name of the instruction
    private $order; //instruction's order
    private $args;  //array of instruction's arguments

    /**
     * Initializes the object of type Instruction with the following data
     * @param name Name of the instruction
     * @param order Instruction's order
     */
    function __construct($name, $order)
    {
        $this->name = $name;
        $this->order = $order;
        $this->args = array();
    }

    //getters
    public function GetName()  { return $this->name; }
    public function GetOrder() { return $this->order; }
    public function GetArgs()  { return $this->args; }

    /**
     * Checks the argument's validity and adds it to the objects $args attribute
     * @param type Type that the argument should have and against which it's going to be tested
     * @param value Full value of the argument from IPPcode20 (with type/scope and value)
     */
    public function AddArgument($type, $value)
    {
        switch($type)
        {
            case SYMB: //symbol can be both variable and value
                //in case of a value
                if(preg_match('/^(int|string|bool|nil)@.*$/', $value))
                {
                    $splitValue = explode('@', $value, 2);
                    switch($splitValue[0])
                    {
                        case "int":
                            if(!preg_match('/^(\+|-)?[0-9]+$/', $splitValue[1]))
                                $this->Error();
                            $type = "int";
                            break;
                        case "string":
                            if(preg_match('/[[:space:]]/', $splitValue[1]) ||    //is there a space?
                               preg_match('/\\\\(?![0-9]{3})/', $splitValue[1])) //is there non escape sequence backslash?
                               $this->Error();
                            $type = "string";
                            break;
                        case "bool":
                            if(!preg_match('/^(true|false)$/', $splitValue[1]))
                                $this->Error();
                            $type = "bool";
                            break;
                        default: //nil
                            if(!preg_match('/^nil$/', $splitValue[1]))
                                $this->Error();
                            $type = "nil";
                    }
                    $value = $splitValue[1];
                }
                //now it's not a value, so if it's not a variable, it's an error
                elseif(preg_match('/^(LF|GF|TF)@(_|-|\$|&|%|\*|[a-zA-Z])(_|-|\$|&|%|\*|[0-9a-zA-Z])*$/', $value))
                    $type = "var";
                else
                    $this->Error();
                break;
            case LABEL:
                if(!preg_match('/^(_|-|\$|&|%|\*|[a-zA-Z])(_|-|\$|&|%|\*|[0-9a-zA-Z])*$/', $value))
                    $this->Error();
                break;
            case VARIABLE:
                if(!preg_match('/^(LF|GF|TF)@(_|-|\$|&|%|\*|[a-zA-Z])(_|-|\$|&|%|\*|[0-9a-zA-Z])*$/', $value))
                    $this->Error();
                break;
            case TYPE:
                if(!preg_match('/^(int|string|bool)$/', $value))
                    $this->Error();
                break;
            default: //no other is allowed
                $this->Error();
        }
        //if we got here, no validation error occured -> argument is fine, let's add it
        if($type == "string") //if it's a string ... replace <>& for &lt;, &gt; &amp;
        {                                                            
            str_replace('<', "&lt;", $value);
            str_replace('>', "&gt;", $value);
            str_replace('&', "&amp;", $value);
        }
        //the same goes for & in variable and label
        if($type == VARIABLE || $type == LABEL)
            str_replace('&', "&amp;", $value);
        //cannot use associative array as the key would be overwritten ... ^ is forbidden char in variable names in IPPcod20 -> ok
        array_push($this->args, $type."^^^".$value); 
   }

    /**
     * Ends program when an error occures
     */
    private function Error()
    {
        fprintf(STDERR, "Error: Invalid instruction argument. Instruction number: ".$this->order."\n");
        exit(23);
    }
}

/** Class representing the output XML document */
class XmlStream
{
    //attributes
    private $xml;    //the stream==document itself
    private $root;   //root <program> element

    /**
     * Initializes the xml stream, sets the paramaters(settings) and creates the root element
     */
    function __construct()
    {
        $this->xml = new DOMDocument("1.0", "UTF-8");       //header
        $this->root = $this->xml->createElement("program"); //base element
        $this->root->setAttribute("language", "IPPcode20");
        $this->xml->appendChild($this->root);
        $this->xml->formatOutput = true;                    //nice formatting
    }

    /**
     * Method to write (convert) an instruction to the xml file
     * @param instruction Instruction to be written to the xml
     */
    function In($instruction)
    {
        //create instruction element
        $elmt = $this->xml->createElement("instruction");
        $elmt->setAttribute("opcode", strtoupper($instruction->GetName()));
        $elmt->setAttribute("order", $instruction->GetOrder());
        //create elements of instruction arguments
        $argCount = 1;
        foreach($instruction->GetArgs() as $arg)
        {
            $arg = explode("^^^", $arg, 2);
            $argElmt = $this->xml->createElement("arg".$argCount++);
            $argElmt->setAttribute("type", $arg[0]);
            $argElmt->textContent = $arg[1];
            $elmt->appendChild($argElmt);
        }
        $this->root->appendChild($elmt);
    }

    /**
     * prints the stream to stdout
     */
    function Out()
    {
        $this->xml->save("php://stdout");
    }
}

/** Class for argument info and checking */
class Args
{
    //attributes
    private $options;       //available program options
    private $errorMessage;  //error message for incorrect argument input

    /**
     * Initializes an Args object with default options and error message
     */
    function __construct()
    {
        $this->GetOpts();
        $this->errorMessage = "Argument error: Wrong set of arguments.\nSee \"parse.php --help\" for more info.\n";
    }

    /**
     * Script options can repeat itself -> cannot use getopt as it takes it only ones
     */
    private function GetOpts()
    {
        global $argv;
        //multiple --help is an error and multiple --stats will not be tested (Krivka said on forum) -> can use getopt on these ones
        $this->options = getopt("", ["help", "stats:"]);
        foreach($argv as $arg) //"loc", "comments", "labels", "jumps"
        {
            switch($arg)
            {
                case "--loc": array_push($this->options, "loc"); break;
                case "--comments": array_push($this->options, "comments"); break;
                case "--labels": array_push($this->options, "labels"); break;
                case "--jumps": array_push($this->options, "jumps"); break;
            }
        }
    }

    /**
     * Ends program on invalid argument input
     */
    private function Error()
    {
        fwrite(STDERR, $this->errorMessage);
        exit(10);
    }

    /**
     * checks the correctness of argument input
     * @param stats Statistics object
     * @param argCount Number of arguments provided by user
     */
    public function CheckArgs($stats, $argCount)
    {
        //if there isn't the same number of options and arguments,
        //then there's an argument which is not an option or the option is incorrect
        if(count($this->options) != $argCount)
        {
            $this->Error();
        }

        //all arguments are correct, now check combinations
        switch($argCount)
        {
            case 0: //all is good, no parameters
                return;
            case 1: // 1 parameter means --help or lone --stats
                if(array_key_exists("help", $this->options))
                {
                    fprintf(STDOUT, "This script takes IPPcode20 code in standard input (stdin) and writes it's XML representation to the standard output (stdout).\n");
                    fprintf(STDOUT, "Program options:\n");
                    fprintf(STDOUT, "--help       - Prints the information about this program. Cannot be used with other options.\n");
                    fprintf(STDOUT, "--stats=file - Programs writes IPPcode20 statistics to the specified file.\n");
                    fprintf(STDOUT, "--loc        - The statistics displays the number of lines of the IPPcode20 program.\n");
                    fprintf(STDOUT, "--comments   - The statistics displays number of comments in the IPPcode20 program.\n");
                    fprintf(STDOUT, "--labels     - The statistics displays number of labels in the IPPcode20 program.\n");
                    fprintf(STDOUT, "--jumps      - The statistics displays number of (un)conditional jumps, function calls and returns in the IPPcode20 program.\n");
                    fprintf(STDOUT, "The last four statistical options must be used with the --stats=file option!\n");
                    exit(0);
                }
                elseif(array_key_exists("stats", $this->options))
                    $stats->SetFileName($this->options["stats"]);
                else //any other option is statistical option X cannot be without --stats
                    $this->Error();
            default: // more options means --stats + any number of others, need to check the presence of --stats and NOT --help
                if(!array_key_exists("stats", $this->options) || array_key_exists("help", $this->options))    
                    $this->Error();
                else
                    $stats->SetFileName($this->options["stats"]);
        }
    }

    //getter for options
    public function GetOptions() { return $this->options; }
}

/** Class collects statistic information for STATP extension */
class Stats
{
    //atributes
    private $comments;     //number of comments
    private $labels;       //number of labels
    private $labelsArr;    //array of labels for checking that they are unique
    private $jumps;        //number of (un)conditional jumps, function calls, returns
    private $file;         //name of the file to output the statistical results
    private $instructions; //handles the instruction order -> contains a value for the next instruction ...
                           //also holds value for --loc option which is basically just one less

    /**
     * Initializes the object with zero values
     */
    function __construct()
    {
        $this->comments = 0;
        $this->labels = 0;
        $this->labelsArr = array();
        $this->jumps = 0;
        $this->instructions = 1;
    }

    //getters
    public function GetComments()     { return $this->comments; }
    public function GetLabels()       { return $this->labels; }
    public function GetJumps()        { return $this->jumps; }
    public function GetInstructions() { return $this->instructions; }

    //setters
    public function SetFileName($fileName) { $this->file = $fileName; }
    //these are incrementive ones
    public function CommentsUp()     { $this->comments++; }
    public function JumpsUp()        { $this->jumps++; }
    public function InstructionsUp() { $this->instructions++; }
    //label increment also checks for label uniqueness, parameter is the currently found label
    public function LabelsUp($label) 
    { 
        if(!in_array($label, $this->labelsArr))
        {
            $this->labels++;
            array_push($this->labelsArr, $label);
        }
    }

    /**
     * In argument options it finds what it shoul write out a then 
     * it writes statistical results to the specified file
     * @param args Script arguments object
     */
    public function Out($args)
    {
        $out = "";
        $print = false;
        //collects the info ... stats is a key, rest are values
        foreach($args->GetOptions() as $key => $value)
        {
            if($key === "stats")
                $print = true;
            switch($value)
            {
                case "loc":
                    $out = $out.($this->GetInstructions()-1)."\n";
                    break;
                case "comments":
                    $out = $out.$this->GetComments()."\n";
                    break;
                case "labels":
                    $out = $out.$this->GetLabels()."\n";
                    break;
                case "jumps":
                    $out = $out.$this->GetJumps()."\n";
                    break;
                default:
                    break;
            }
        }
        //print it
        if($print)
        {
            try
            {
                file_put_contents($this->file, $out);
            }
            catch(Exception $e)
            {
                fprintf(STDERR, "Error: Couldn't write the statistics to the specified file!\n");
                exit(12);
            }
        }
    }
}

/* Main() of the program */
try
{
    $parser = new Parser();
    $parser->Run();
}
catch(Exception $e)
{
    fwrite(STDERR, "Internal error occurred!\n");
    exit(99);
}
?>