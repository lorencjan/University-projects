# Solution: FLP - project 2 - Turing machine
# File: Makefile
# Author: Jan Lorenc (xloren15)
# University: Brno University of technology: Faculty of information technology
# Date: 04.04.2022

TARGET := flp21-log

build: $(TARGET)
$(TARGET): *.pl
	swipl -q --goal=main --stand_alone=true -o $@ -c $^

clean:
	rm -f $(TARGET)
