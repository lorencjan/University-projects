# WAP - Prototype chain traversal
### About
Course: *WAP - Internet applications*  
Solution: *Project 1 - Prototype chain traversal*  
Author: *Jan Lorenc (xloren15)*  
University: *Brno University of Technology - Faculty of Information Technology*  
Year: *2022/23*  

This project implements `iterateProperties` generator, which returns property names of a given object. Project was developed and tested on node.js version 14.17.0 (corresponding to merlin server at the time).

### Files
* `iterate.mjs` - This module implements and exports the property generator.
* `test.sh` - Runs `__test__/iterate.test.mjs` unit tests. Requires jest testing framework. If it is not installed, run `sh test.sh --install` which first installs jest module and then runs the tests.
* `doc.sh` - Generates jsDoc documentation. Requires jsDoc module. If it is not installed, run `sh doc.sh --install` which first installs jsDoc module and then generates the documentation.
* `jsdoc.config.json` - Configuration file for jsDoc which by default ignores .mjs files so they need to be included.
* `package.json` - Not really necessary for the project, but it provides general config for the solution in one place. It describes what packages are required (jest, jsDoc), provides jest configuration and defines scripts for running and installing tests and documentation.
* `__test__/iterate.test.mjs` - This script implements the unit tests.
