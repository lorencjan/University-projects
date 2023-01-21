-- Solution: FLP - project 1 - dka-2-mka
-- File: ParseInput.hs
-- Author: Jan Lorenc
-- Login: xloren15
-- University: Brno University of technology: Faculty of information technology
-- Date: 21.2.2022

module ParseInput where

import Types
import qualified Data.Set as Set
import qualified Data.Map as Map
import Data.Char ( isSpace, isAsciiLower )
import Data.Maybe ( catMaybes, isNothing )
import Text.Read ( readMaybe )
import System.IO
import System.Exit

-- constant for repetitive invalid input error
invalidInput :: [Char]
invalidInput = "Error: Input has invalid format!"

-- ends program execution with an error message
exitOnFailure :: String -> IO a
exitOnFailure msg = do
    hPutStrLn stderr $ "Error: " ++ msg
    exitFailure

-- takes a string and removes all whitespaces inside it
removeWhiteSpaces :: String -> String
removeWhiteSpaces = filter (not . isSpace)

-- splits a string by a delimeter
split :: String -> Char -> [String]
split str delim = singleSplit [] str
    where
        singleSplit acc [] = [acc]
        singleSplit acc (x:xs)
            | x == delim = acc : singleSplit [] xs
            | otherwise = singleSplit (acc ++ [x]) xs

-- takes a line which should contain numbers representing states sepparated by commas
-- checks if all are correct numbers and returns set of states
parseStates :: String -> IO States
parseStates input = do
    let line = removeWhiteSpaces input
    if null line  -- not that it's formally incorrect but FA without states doesn't make sense
        then exitOnFailure "No states were specified!"
        else do
            let ss = map readMaybe $ split line ','
            if any isNothing ss
                then exitOnFailure invalidInput
                else return $ Set.fromList $ catMaybes ss

-- takes a line which should contain just letters representing alphabet symbols
-- checks if all are lowercase letters and returns set of characters (symbolse) = alphabet
parseAlphabet :: String -> IO Alphabet
parseAlphabet input = do
    let line = removeWhiteSpaces input
    if null line  -- again not that it's formally incorrect but FA without alphabet doesn't make sense
        then exitOnFailure "No alphabet symbols were specified!"
    else if all isAsciiLower line
        then return $ Set.fromList line
        else exitOnFailure invalidInput

-- takes a line which should contain single number representing initial state
-- checks if it's truly single number and if it belongs to the states set
parseInitState :: String -> States -> IO State
parseInitState input ss = do
    parsed <- parseStates input
    if Set.size parsed == 1
        then do
            let s0 = head (Set.toList parsed)
            if Set.member s0 ss
                then return s0
                else exitOnFailure "Initial state must be among specified states!"
        else exitOnFailure invalidInput

-- takes a line which should contain numbers representing final states
-- checks if all are correct numbers and if they belong to the states set
parseFinalStates :: String -> States -> IO States
parseFinalStates input ss = do
    parsed <- parseStates input
    if Set.isSubsetOf parsed ss  -- here no check for 'empty' as FA without final states can be seen though rarely
        then return parsed
        else exitOnFailure "Final states must be a subset of specified states!"

-- takes remaining input lines which should contain FA rules
-- checks if they have correct format and transforms them into the StateRules map structure
parseRules :: [String] -> States -> Alphabet -> IO StateRules
parseRules inputLines ss a = do
    -- parses single line into a Rule type
    let parseRule :: String -> IO Rule
        parseRule line = do
            let ruleElems = split line ','
            if length ruleElems /= 3 then exitOnFailure invalidInput else do
                -- checks if the string representing state is valid and returns it as State
                let parseRuleState :: String -> IO State
                    parseRuleState state = case readMaybe state :: Maybe State of
                        Just s -> if Set.member s ss
                            then return s
                            else exitOnFailure ("Rule state " ++ show s ++ " is not in states!")
                        Nothing -> exitOnFailure invalidInput

                -- checks if the string representing rule symbol is valid and returns it as Symbol
                let parseRuleSymbol :: String -> IO Symbol
                    parseRuleSymbol str = do
                        if length str == 1
                            then do 
                                let symbol = head str
                                if Set.member symbol a
                                    then return symbol
                                    else exitOnFailure ("Rule symbol " ++ [symbol] ++ " is not in alphabet!")
                            else exitOnFailure invalidInput

                src <- parseRuleState $ head ruleElems
                symbol <- parseRuleSymbol $ filter (not . isSpace) $ ruleElems !! 1
                dst <- parseRuleState $ ruleElems !! 2
                return (src, symbol, dst)

    -- accumulator function that takes line representing a rule and adds the rule to the resulting StateRules map structure
    let rule2StateRules :: IO StateRules -> String -> IO StateRules
        rule2StateRules ioStateRules inputLine = do 
            (src, symbol, dst) <- parseRule inputLine
            stateRules <- ioStateRules
            let ruleSteps = getRuleSteps src stateRules
            -- if the rule already exists, fail if non-deterministic or ignore if duplicate
            updatedRuleSteps <- case Map.lookup symbol ruleSteps of
                Just x -> if x == dst then return ruleSteps else exitOnFailure "Given finite automata is not deterministic!"
                Nothing -> return $ Map.insert symbol dst ruleSteps
            
            return $ Map.insert src updatedRuleSteps stateRules

    let cleanLines = filter (not . null) $ map removeWhiteSpaces inputLines
    foldl rule2StateRules (return Map.empty) cleanLines

-- takes all lines of the input file and converts it to a finite automata
parseFA :: [String] -> IO FA
parseFA inputLines = do
    if length inputLines < 4  -- there should at least be states and alphabet, there can be no rules
        then exitOnFailure invalidInput
        else do
            let (s:a:s0:f:r) = inputLines
            parsedStates <- parseStates s
            parsedAlphabet <- parseAlphabet a
            parsedInitState <- parseInitState s0 parsedStates
            parsedFinalStates <- parseFinalStates f parsedStates
            parsedRules <- parseRules r parsedStates parsedAlphabet

            return FA {
                states = parsedStates,
                alphabet = parsedAlphabet,
                initState = parsedInitState,
                finalStates = parsedFinalStates,
                rules = parsedRules
            }
