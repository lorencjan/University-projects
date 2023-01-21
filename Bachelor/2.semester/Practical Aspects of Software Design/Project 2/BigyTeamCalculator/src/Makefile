PROJNAME=Calculator
PROJ=$(PROJNAME)\$(PROJNAME)\bin\Debug\$(PROJNAME).exe
PROJFILE=.\$

PROF_NAME=Profiling
PROF=$(PROF_NAME).exe
PROF_PATH=..\$(PROF_NAME)\$(PROF_NAME)\bin\Debug

DOCUMENTATION=..\dokumentace.pdf
EVALUATION=..\hodnoceni.txt
HANDOVER=..\odevzdani.txt
ZIPNAME=xloren15_xkalab11_xstane45_xhrani01.zip

PROJ_BIN=$(PROJNAME)\$(PROJNAME)\bin
PROJ_OBJ=$(PROJNAME)\$(PROJNAME)\obj
MATH_BIN=$(PROJNAME)\CustomMath\bin
MATH_OBJ=$(PROJNAME)\CustomMath\obj 
TEST_BIN=$(PROJNAME)\MathLibraryTests\bin
TEST_OBJ=$(PROJNAME)\MathLibraryTests\obj
PROF_BIN=..\$(PROF_NAME)\$(PROF_NAME)\bin 
PROF_OBJ=..\$(PROF_NAME)\$(PROF_NAME)\obj

.PHONY : all pack clean run profile help

all : run

pack : 
	7z a -tzip $(ZIPNAME) $(PROJNAME) $(EVALUATION) $(HANDOVER) $(DOCUMENTATION)
clean :
	if exist "$(PROJ_BIN)" rmdir /S /Q $(PROJ_BIN)
	if exist "$(PROJ_OBJ)" rmdir /S /Q $(PROJ_OBJ)
	if exist "$(MATH_BIN)" rmdir /S /Q $(MATH_BIN) 
	if exist "$(MATH_OBJ)" rmdir /S /Q $(MATH_OBJ)
	if exist "$(TEST_BIN)" rmdir /S /Q $(TEST_BIN)
	if exist "$(TEST_OBJ)" rmdir /S /Q $(TEST_OBJ) 
	if exist "$(PROF_BIN)" rmdir /S /Q $(PROF_BIN) 
	if exist "$(PROF_OBJ)" rmdir /S /Q $(PROF_OBJ) 

run :
	$(PROJ)
profile :
	cd $(PROF_PATH) && $(PROF)
help :
	@echo This makefile doesnt build the program. You must do it via Visual Studio before you runs the calculator.
	@echo make: runs the alculator
	@echo make pack: using 7zip makes zip file named xloren15_xkalab11_xstane45_xhrani01.zip 
	@echo		   whitch contains hodnoceni.txt, odevzdani.txt, dokumentace.pdf and folder Calculator            
	@echo make clean: remove all files that should not hand in
	@echo make run:  runs the alculator
	@echo make profile: runs profiling program
