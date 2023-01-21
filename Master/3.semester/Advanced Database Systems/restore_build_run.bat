
dotnet restore
dotnet build

start cmd /c call run_command.bat
start cmd /c call run_projection.bat
start cmd /c call do_projection.bat
start cmd /c call run_query.bat