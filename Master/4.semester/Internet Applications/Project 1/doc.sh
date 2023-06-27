#!/bin/bash

# File: doc.sh
# Author: Jan Lorenc (xloren15)
# Date: 12.02.2023
# Course: WAP - Internet applications
# Assignment: Prototype chain traversal
# University: Brno University of Technology - Faculty of Information Technology
# Description: Simple documentation generating script. Without any argument, it just runs doc script from package.json.
#              If --install option is provided, it install doc dependencies (jsdoc framework). Otherwise ends with argument error.


if [[ $# -eq 0 ]]
then
	npm run doc
elif [[ $# -eq 1 && "$1" = "--install" ]]
then
    npm run install-doc
    npm run doc
else
    echo "Argument error: Run without arguments or use single --install argument to install doc requirements beforehand."
    exit 1
fi

exit 0
