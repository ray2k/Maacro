@call "%VS110COMNTOOLS%\VsDevCmd.bat"
msbuild .\Maacro\Maacro.csproj /t:Clean
msbuild .\Maacro\Maacro.csproj /t:Build /p:OutDir=..\build /p:Configuration=Release
@del .\build\*.pdb
@del .\build\*.xml
pause