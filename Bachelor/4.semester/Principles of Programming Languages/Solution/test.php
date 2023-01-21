<?php
/* 
 *  File: test.php
 *  Solution: IPP project 2019/2020
 *  Date: 15.2.2020
 *  Author: Jan Lorenc (xloren15)
 *  Faculty: Faculty of Information Technology VUT
 *  Description: This script runs automated tests on parser.php and interpret.py
 */

/** Abstract class for error messages */
abstract class AbstractError
{
    /**
     * Ends program when an error occures
     * @param errCode Value to be returned by the script (error code)
     * @param msg Message to be displayed to the user
     */
    protected function Error($errCode, $msg)
    {
        fprintf(STDERR, $msg);
        exit($errCode);
    }
}

 /** Class for testing ... de facto main() of the program, it directs everything */
class Tester extends AbstractError
{
    //attributes
    private $args;                 //program arguments
    private $htmlWriter;           //instance of HtmlWriter class
    private $dirs;                 //array of all the directories to tests ... contains only 1 if --testlist is not set
    private $sideTests;            //in case of --testlist, there can be not only dirs but also tests.src, special array for those
    private $numOfTests;           //sum of all tests for overall statistics
    private $numOfSuccessfulTests; //sum of successful tests for overall statistics

    /** Initializes its attributes. */
    function __construct()
    {
        $this->args = new Args();
        $this->args->CheckArgs();
        $this->htmlWriter = new HtmlWriter();
        $this->dirs = array();
        $this->sideTests = array();
        $this->LoadDirs();
        $this->numOfTests = 0;
        $this->numOfSuccessfulTests = 0;
    }

    /** Loads directories and files to be tested*/
    private function LoadDirs()
    {
        $opts = $this->args->GetOptions();
        $recursive = array_key_exists("recursive", $opts);
        $regexFilter = array_key_exists("match", $opts) ? $opts["match"] : null;
        //do we have a bunch of dirs or just one?
        if($this->args->GetTestList() !== null)
        {
            //read from the testlist file
            try
            {
                $testlist = file_get_contents($this->args->GetTestList());
            }
            catch(Exception $e)
            {
                $this->Error(11, "Error: Couldn't read from the \"".$this->args->GetTestList()."\" file!\n");
            }
            $testlist = explode(PHP_EOL, $testlist);
            //get the dirs / test files
            foreach($testlist as $test)
            {
                //skip empty lines
                $test = trim($test);
                if($test == "")
                    continue;
                //check if it exists
                if(!file_exists($test))
                    $this->Error(11, "Error: Couldn't find the \"".$test."\"!\n");
                //is it a dir or a test file?
                if(is_dir($test))
                    array_push($this->dirs, new TestDir($test, $recursive, $regexFilter));
                elseif(preg_match("/.*\.src$/", $test))
                    array_push($this->sideTests, substr($test, 0, -4));
                else
                    $this->Error(11, "Error: Invalid test path: \"".$test."\"!\n");
            }
        }
        else //without --testfile option
        {
            array_push($this->dirs, new TestDir($this->args->GetDir(), $recursive, $regexFilter));
            $this->sideTests = null;
        }
    }

    /** Run the tests */
    public function Run()
    {
        //inquire which script to test (parse.php | interpret.py | both)
        $opts = $this->args->GetOptions();
        $run = "all";
        if(array_key_exists("parse-only", $opts))
            $run = "parse-only";
        if(array_key_exists("int-only", $opts))
            $run = "int-only";
        //first run those in the folders
        foreach($this->dirs as $dir)
        {
            $this->htmlWriter->folders .= "<li>".$dir->GetName()."</li>";
            $tests = $dir->GetTests();
            $this->RunTests($tests, $run, $dir->GetName());
        }
        //run the tests on the individual set of tests
        if($this->sideTests !== null && count($this->sideTests) > 0)
        {
            foreach($this->sideTests as $test) //generate missing files
                Tester::GenerateFiles($test);
            $this->RunTests($this->sideTests, $run, "Individual tests");
        } 
        //output html
        $overallResult = $this->numOfTests > 0 ? floor($this->numOfSuccessfulTests/$this->numOfTests*100)."% passed" : "-";
        $recursive = array_key_exists("recursive", $opts) ? "yes" : "no";
        $this->htmlWriter->GenerateHtml($run, $overallResult, $recursive);
        $this->htmlWriter->Out();
    }

    /**
     * Run the parse-only tests fot the set of tests
     * @param tests Array of test files to be teste
     * @param run Defines which script to test (parse.php | interpret.py | both)
     * @param testGroup Defines the group of tests, typically the name of their directory
     */
    private function RunTests($tests, $run, $testGroup)
    {
        //get the testing programs
        $parser = $this->args->GetParser();
        $jexamxml = $this->args->GetJexamxml();
        $interpret = $this->args->GetInterpret();
        //statistical variables
        $numOfTests = 0;
        $numOfSuccessfulTests = 0;
        //init the html table for the tests
        $this->htmlWriter->tables .= "<table><tr><th colspan=\"2\">".$testGroup."</th></tr>";
        //run tests
        foreach($tests as $test)
        {
            $success = false;
            $numOfTests++;
            $parseTmpFile = tmpfile();
            $intTmpFile = tmpfile();
            if($parseTmpFile === false || $intTmpFile === false)
                $this->Error(12, "File error: Couldn't create temporary file for test output!\n");
            $parseTmpFilePath = stream_get_meta_data($parseTmpFile)["uri"];
            $intTmpFilePath = stream_get_meta_data($intTmpFile)["uri"];
            //run the type of test that was specified
            switch($run)
            {
                case "parse-only":
                    exec("php7.4 \"".$parser."\" < \"".$test.".src\" > \"".$parseTmpFilePath."\" 1>&1", $output, $retVal);
                    break;
                case "int-only":
                    exec("python3.8 \"".$interpret."\" --source=\"".$test.".src\" < \"".$test.".in\" > \"".$intTmpFilePath."\" 1>&1", $output, $retVal);
                    break;
                default:
                    exec("php7.4 \"".$parser."\" < \"".$test.".src\" > \"".$parseTmpFilePath."\" 1>&1", $output, $retVal);
                    if($retVal == 0) //in case of an error, use this error code for it's the first one
                        exec("python3.8 \"".$interpret."\" --source=\"".$parseTmpFilePath."\" < \"".$test.".in\" > \"".$intTmpFilePath."\" 1>&1", $output, $retVal);
            }
            //get the expected return value from .rc file
            try
            {
                $expectedRetVal = file_get_contents($test.".rc");
            }
            catch(Exception $e)
            {
                $this->Error(11, "File error: Working with file data failed!\n");
            }
            //compare it and if ok, continue with file comparison
            if($retVal == $expectedRetVal)
            {
                if($retVal == 0) //run if not an error
                {
                    switch($run)
                    {
                        case "parse-only":
                            exec("java -jar \"".$jexamxml."\" \"".$test.".out\" \"".$parseTmpFilePath."\"", $output, $retVal);
                            break;
                        default:
                            exec("diff \"".$test.".out\" \"".$intTmpFilePath."\"", $output, $retVal);
                    }
                }
                else  //if there is expected en error, there will be no output
                    $retVal = 0;
                if($retVal == 0)
                {
                    $success = true;
                    $numOfSuccessfulTests++;
                }
            }
            //close tmpfiles
            fclose($parseTmpFile);
            fclose($intTmpFile);
            //write result to HTML
            $testName = $testGroup == "Individual tests" ? substr($test, 1) : str_replace($testGroup, "", $test);
            $res = $success ? "<td class=\"success\">PASSED</td>" : "<td class=\"fail\">FAILED</td>";
            $this->htmlWriter->tables .= "<tr><td>".substr($testName, 1)."</td>".$res."</tr>";
        }
        //finalize the table
        $percentage = $numOfTests > 0 ? "<span class=\"test-result\">".(floor($numOfSuccessfulTests/$numOfTests*100))."%</span>" : "-";
        $this->numOfTests += $numOfTests;
        $this->numOfSuccessfulTests += $numOfSuccessfulTests;
        $this->htmlWriter->tables .= "</table><div class=\"folder-result\">
            <p>Results: <span class=\"test-result\">".$numOfSuccessfulTests."/".$numOfTests." passed</span> ... 
            Percentage: ".$percentage."</p></div>";
    }

    /**
     * Generates missing files for the test
     * @param file Name of the test
     */
    public static function GenerateFiles($file)
    {
        try
        {
            if(!file_exists($file.".in"))
                file_put_contents($file.".in", "");
            if(!file_exists($file.".out"))
                file_put_contents($file.".out", "");
            if(!file_exists($file.".rc"))
                file_put_contents($file.".rc", "0");
        }
        catch(Exception $e)
        {
            fprintf(STDERR, "File error: Couldn't create missing files.\n");
            exit(12);
        }
    }
}

/** Class for argument info and checking */
class Args extends AbstractError
{
    //attributes
    private $dir;            //directory of the tests
    private $testlist;       //file with paths to directories containing the tests
    private $parser;         //path to parse.php
    private $interpret;      //path to interpret.py
    private $jexamxml;       //path to JAR package of A7Soft JExamXML tool
    private $options;        //script arguments

    /**
     * Initializes the default paths of the testing scripts/tests 
     */
    function __construct()
    {
        $this->dir = getcwd();
        $this->parser = "./parse.php";
        $this->interpret = "./interpret.py";
        $this->jexamxml = "/pub/courses/ipp/jexamxml/jexamxml.jar";
        $this->options = getopt("", ["help", "directory:", "recursive", "parse-script:", "int-script:", "parse-only", "int-only", "jexamxml:", "testlist:", "match:"]);
    }

    /**
     * Checks the correct combination of the script argument==options
     */
    public function CheckArgs()
    {
        $argErrMsg = "Argument error: Wrong set of arguments.\nSee \"tests.php --help\" for more info.\n";
        $fileErrMsg = "File error: Couldn't open the file ";
        
        //if there isn't the same number of options and arguments,
        //then there's an argument which is not an option or the option is incorrect
        global $argc;
        if(count($this->options) != $argc-1)
            $this->Error(10, $argErrMsg);

        /* all arguments are correct, now check combinations */
        //--help must be alone
        if(array_key_exists("help", $this->options))
        {
            if(count($this->options) > 1)
                $this->Error(10, $argErrMsg);
            //it is alone -> print help and exit
            fprintf(STDOUT, "This program tests the functionality of scripts parser.php and interpret.py.\nIt takes all the tests in given folder and runs them ");
            fprintf(STDOUT, "through parse.php which should output an XML representation of IPPcode20 for interpret.py which then compiles it.\n");
            fprintf(STDOUT, "Output of this program is and HTML table with the results of the tests\n");
            fprintf(STDOUT, "Program options:\n");
            fprintf(STDOUT, "--help              - Prints the information about this program. Cannot be used with other options.\n");
            fprintf(STDOUT, "--directory=path    - Directory with the tests. If omitted, it'll search for the tests in current working directory\n");
            fprintf(STDOUT, "--recursive         - Program will also search for the tests in subdirectories.\n");
            fprintf(STDOUT, "--parse-script=file - Path to the parse.php script. By default it takes ./parse.php file.\n");
            fprintf(STDOUT, "--int-script=file   - Path to the interpret.py script. By default it takes ./interpret.py file.\n");
            fprintf(STDOUT, "--parse-only        - It will run the tests only for the parse.php script. Cannot be combined with --int-script=file or --int-only options.\n");
            fprintf(STDOUT, "--int-only          - It will run the tests only for the interpret.py script. Cannot be combined with --parse-script=file or --parse-only options.\n");
            fprintf(STDOUT, "--jexamxml=file     - Path to JAR package of A7Soft JExamXML tool. By default it takes the one in merlins /pub/courses/ipp/jexamxml/ shared folder.\n");
            fprintf(STDOUT, "--testlist=file     - It will take the paths of directories/files with the tests from given file. Cannot be combined with --directory option.\n");
            fprintf(STDOUT, "--match=regexp      - Program will only take the tests whose names match the regular exression.\n");
            exit(0);
        }

        //--parse-only cannot be used with --int-script=file / --int-only
        if(array_key_exists("parse-only", $this->options))
        {
            if(array_key_exists("int-script", $this->options) || array_key_exists("int-only", $this->options))
                $this->Error(10, $argErrMsg);
        }
        //--int-only cannot be used with --parse-script=file / --parse-only
        if(array_key_exists("int-only", $this->options))
        {
            if(array_key_exists("parse-script", $this->options) || array_key_exists("parse-only", $this->options))
                $this->Error(10, $argErrMsg);
        }
        //--testlist=file cannot be used with --directory
        if(array_key_exists("testlist", $this->options) && array_key_exists("directory", $this->options))
            $this->Error(10, $argErrMsg);
        //check regex validity
        if(array_key_exists("match", $this->options) && preg_match("/".$this->options["match"]."/", null) === false)
            $this->Error(11, "Invalid regular expression!\n");

        //set the given values for directory, parse-script, int-script, jexamxml, testlist because we'll always work with them
        //recursive, parse-only, int-only and match are just optional, $this->options will suffice for it
        if(array_key_exists("directory", $this->options))
        {
            $this->dir = $this->options["directory"];
            if (!file_exists($this->dir))
                $this->Error(11, "Path error: Specified directory doesn't exit!\n");
        }
        if(array_key_exists("parse-script", $this->options))
            $this->parser = $this->options["parse-script"];
        if(array_key_exists("int-script", $this->options))
            $this->interpret = $this->options["int-script"];
        if(array_key_exists("jexamxml", $this->options))
            $this->jexamxml = $this->options["jexamxml"];
        if(array_key_exists("testlist", $this->options))
        {
            $this->testlist = $this->options["testlist"];
            if(!file_exists($this->testlist))
                $this->Error(11, $fileErrMsg.$this->testlist."\n");
        }

        //now check the existence of the files we're working with
        if (!file_exists($this->parser) && !array_key_exists("int-only", $this->options))
            $this->Error(11, $fileErrMsg.$this->parser."\n");
        if (!file_exists($this->interpret) && !array_key_exists("parse-only", $this->options))
            $this->Error(11, $fileErrMsg.$this->interpret."\n");
        if (!file_exists($this->jexamxml) && !array_key_exists("int-only", $this->options))
            $this->Error(11, $fileErrMsg.$this->jexamxml."\n");
    }

    //getters
    public function GetDir() { return $this->dir; }
    public function GetTestList() { return $this->testlist; }
    public function GetParser() { return $this->parser; }
    public function GetInterpret() { return $this->interpret; }
    public function GetJexamxml() { return $this->jexamxml; }
    public function GetOptions() { return $this->options; }
}

/**
 * Symbolizes a directory with the tests.
 * Searches for the tests, checks/creates necessary files and store all the tests in it
 */
class TestDir extends AbstractError
{
    //attributes
    private $name;      //name of this directory
    private $tests;     //array of all the test
    private $recursive; //boolean defining if we should search the folder recursively
    private $filter;    //gegex filter for the test files

    /**
     * Initializes the object attribtues
     * @param name Path to this folder
     * @param recursive Boolean defining if we should search the folder recursively
     * @param filter Regex filter for the test files
     */
    function __construct($name, $recursive, $filter)
    {
        $this->name = realpath($name);
        $this->recursive = $recursive;
        $this->filter = $filter;
        $this->tests = $this->FindTests();
    }

    /**
     * Finds all the tests in this folder
     * @return Array full of test files
     */
    private function FindTests()
    {
        //srcFiles will contain all the .src files
        $dir = new RecursiveDirectoryIterator($this->name);
        $iterator = $this->recursive ? new RecursiveIteratorIterator($dir, RecursiveIteratorIterator::CHILD_FIRST) : new IteratorIterator($dir);
        $srcFiles = new RegexIterator($iterator, "/^.+\.src$/", RecursiveRegexIterator::GET_MATCH);
        //search for .in, .out, .rc files and create if necessary
        $tests = array();
        $inSubDirs = array();
        foreach($srcFiles as $file)
        {
            //apply the regex filter rule from program options
            $file = preg_replace('/\.src$/', '', $file[0]);
            preg_match_all('/[^\\/\\\\]+$/', $file, $nonAbsoluteName);
            if(!preg_match("/".$this->filter."/", $nonAbsoluteName[0][0]))
                continue;
            //recreate missing files if necessary
            Tester::GenerateFiles($file);
            //sort it to look nice
            $relativeName = str_replace($this->name."/", "", $file);
            if(strpos($relativeName, "/"))
                array_push($inSubDirs, $file);
            else
                array_push($tests, $file);
        }
        return array_merge($tests, $inSubDirs);
    }

    //Getters
    public function GetTests() { return $this->tests; }
    public function GetName() { return $this->name; }
}

/** This class takes care of generating the HTML output code */
class HtmlWriter
{
    //attributes
    private $html;   //resulting html
    public $tables;  //piece of html for test tables
    public $folders; //piece of html for folder list

    /** Null initialize the attributes*/
    function __construct()
    {
        $this->html = "";
        $this->tables = "";
        $this->folders = "";
    }

    /**
     * Creates the html content
     * @param type Type of the tests ... full, parse-only, int-only
     * @param result Percentage overall result
     * @param recursive Did we search the given folders recursively
     */
    public function GenerateHtml($type, $result, $recursive)
    {
        $this->html = 
        "<!DOCTYPE html>
        <html>
            <head>
                <meta charset=\"utf-8\" />
                <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">
                <title>Results of test.php</title>
            </head>
        
            <style>
                body{
                    background: #f2f2f2;
                    font-family: Verdana, sans-serif;
                    padding: 0% 12.5%;
                }
                h1{
                    font: 700 2.5em Calibri, seriff;  
                }
                .navbar, footer{
                    background: black;
                    color: white;
                    min-height: 60px;
                }
                .navbar{
                    text-align: center;
                    padding: 1px;
                }
                footer{
                    position: absolute;
                    width: 100%;
                    bottom: 0px;
                }
                .container{
                    position: relative;
                    width: 100%;
                    background-color: white;
                    padding: 0;
                    padding-bottom: 60px;
                    min-height: 950px;
                    margin-top: -10px;
                }
                p{
                    margin: 0;
                }
                ul{
                    margin: 2px;
                    list-style-type: none;
                }
                .general-info, .test-tables, .caption{
                    padding: 15px 0px;
                    padding-left: 2.5%;
                    width: 100%;
                }
                .caption{width: 95%;}
                .test-result{
                    font-weight: bold;
                    color: #4caf50;
                }
                .folder-result{
                    margin: 8px 0px 20px;
                }
                table {
                    border-collapse: collapse;
                    width: 95%;
                }
                th, td {
                    text-align: left;
                    padding: 8px;
                }
                td.resultCell{
                    text-align: center;
                    font-weight: bold;
                }
                td.success, td.success{
                    width: 30%;
                }
                td.success{
                    color: #4caf50;
                }
                td.fail{
                    color: red;
                }
                tr{
                    background-color: #fcfcfc;
                }
                tr:nth-child(even){
                    background-color: #f2f2f2;
                }
                th {
                    background-color: black;
                    color: white;
                }
            </style>
        
            <body>  
                <div class=\"container\">
                    <div class=\"navbar\">
                        <h1>Tests results</h1>
                    </div>
                    <div class=\"general-info\">
                        <p>Overall score: <span class=\"test-result\">".$result."</span></p>
                        <p>Test type: <b>".$type."</b></p>
                        <p>Given directories with tests:</p>
                        <ul>".$this->folders."</ul>
                        <p>Recursive search for tests: <b>".$recursive."</b></p>
                    </div>
                    <hr/>
                    <p class=\"caption\">There is one table for each test directory specified in program arguments. 
                    Tests in subfolders in case of '--recursive' option are included in that table. 
                    Therefore there is only one table for '--directory=path' option and multiple for '--testlist=file' option.</p>
                    <div class=\"test-tables\">"
                    .$this->tables.
                    "</div>
                    <footer>
                        <p>&copy; Jan Lorenc (xloren15)</p>
                        <p>Faculty of information technology VUT</p>
                        <p>2020</p>
                    </footer>
                </div> 
            </body>
        </html>";
    }

    /** Writes out the html */
    public function Out()
    {
        fwrite(STDOUT, $this->html);
    }
}

/* Run the program ... de facto main() */
try
{
    $tester = new Tester();
    $tester->Run();
}
catch(Exception $e)
{
    fwrite(STDERR, "Internal error occurred!\n");
    exit(99);
}
?>