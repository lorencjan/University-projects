# [FLP - dka-2-mka]
### About
Solution: *FLP - project 1 - dka-2-mka*
Author: *Jan Lorenc*
University: *Brno University of technology: Faculty of information technology*
Year: *2021/22*

The solution implements conversion of deterministic finite automata (DFA) to minimal finite automata (MFA) with total transition function.

### Implementation
Source code consists of 4 files:
* `Types.hs` - Contains custom types used in the solution. In addition, few helper function for handling the new types are implemented here.
* `ParseInput.hs` - Implements functions reading the input DFA and parses it into an internal representation of the automata. It allows for whitespaces. Other structural differences cause errors. Moreover, it checks the FA structure (e.g. if initial state is among states, if there are even any states etc.) and if the automata is deterministic.
* `Minimize.hs` - Takes care of the automata minimization. Firstly, it removes unreachable states with corresponding transition rules. Secondly, it transforms the automata to complete automata by adding sink state. This is done to meet the total transition function condition. Lastly, it merges nondistinguishable states to create MFA.
* `Main.hs` - Entrypoint of the program. It just parses the arguments and runs the input parsing and minimization if required.

### Usage
```
make
flp21-fun (-i | -t) [file]
    -i      Prints only the parsed DFA
    -t      Prints minimized DFA
    [file]  Name of the file with input DFA.
            If omitted, program reads from standard input
```