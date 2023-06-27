#!/bin/bash

# File: test.sh
# Author: Jan Lorenc (xloren15)
# Date: 12.02.2023
# Course: WAP - Internet applications
# Assignment: Prototype chain traversal
# University: Brno University of Technology - Faculty of Information Technology
# Description: Simple unit test running script. Without any argument, it just runs test script from package.json.
#              If --install option is provided, it install test dependencies (jest framework). Otherwise ends with argument error.


if [[ $# -eq 0 ]]
then
	npm run test
elif [[ $# -eq 1 && "$1" = "--install" ]]
then
    npm run install-test
    npm run test
else
    echo "Argument error: Run without arguments or use single --install argument to install test requirements beforehand."
    exit 1
fi

exit 0
