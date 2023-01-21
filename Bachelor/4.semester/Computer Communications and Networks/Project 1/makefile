# File: makefile
# Solution: IPK - Project 1
# Date: 22.2.2020
# Author: Jan Lorenc (xloren15)
# Faculty: Faculty of Information Technology VUT
# Description: This is a makefile for building, running and cleaning the project

proj=./src/Server.csproj

build: 
	dotnet build $(proj)

run:
	dotnet run -p $(proj) $(PORT)

clean:
	dotnet clean $(proj)
	
restore:
	dotnet restore $(proj)