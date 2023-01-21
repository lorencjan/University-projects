# [FLP - turing machine]
### About
Solution: *FLP - project 2 - turing machine*
Author: *Jan Lorenc*
University: *Brno University of technology: Faculty of information technology*
Year: *2021/22*

The solution implements simulation of non-deterministic Turing machine (TM) in prolog.

### Implementation
Whole source code can be found in just one file:
* `flp21-log.pl` - Program first reads the input and parses rule lines (all but last). Rules are then validated and program ends on validation error if a rule isn't in correct format. In addition, first state is checked if it truly is initial state S. Then the actual simulation begins. Firstly, it finds the current state and symbol under head. Secondly, it finds appropriate rule and lastly, it applies the rule and continues recursively in the simulation execution. One the final state F is reached, program ends and prints all visited TM configurations.

### Tests
In /tests directory can by found following tests. Each has an .in and .out file.
test1 - Accepts language (a|b)^n. Duration: 18ms.
test2 - Tests shifts. Duration: 19ms.
test3 - Transforms series of a^n to b^n. Duration: 21ms.
test-reference - Reference input test. Duration: 21ms.
test-fail-abnormal-termination - Terminates abnormaly as final state F is missing an it doesn't know how to continue. Duration: 21ms.
test-fail-missing-input-state - Recognizes that initial state is missing. Duration: 23ms.
test-fail-invalid-format - Fails on unknown tape symbol. Duration: 24ms.

### Usage
```
make
flp21-log < input.in > output.out
```
