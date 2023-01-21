-- Solution: FLP - project 1 - dka-2-mka
-- File: Main.hs
-- Author: Jan Lorenc
-- Login: xloren15
-- University: Brno University of technology: Faculty of information technology
-- Date: 21.2.2022

module Main where

import Minimize
import ParseInput ( exitOnFailure, parseFA )
import Types ( printFA )
import System.IO ()
import System.Environment ( getArgs )

-- reads program arguments, checks their correctness
-- returns file path (empty for stdin) and True for Minimization, False for just FA output
readArgs :: IO (String, Bool)
readArgs = do
    args <- getArgs
    let argCount = length args

    -- checks if option is valid and create proper tuple
    let createResult = \opt path -> if opt == "-i" || opt == "-t"
        then return (path, opt == "-t")
        else exitOnFailure "Invalid option. Use -i or -t!"

    -- returns appropriate tuple according to number of arguments
    if argCount == 2 then do
        let (opt:path:_) = args
        createResult opt path
    else if argCount == 1 then createResult (head args) []
    else exitOnFailure "Invalid number of arguments!"

main :: IO()
main = do
    (path, minimize) <- readArgs
    input <- if null path then getContents else readFile path 
    fa <- parseFA $ lines input
    printFA $ if minimize then minimizeFA fa else fa
