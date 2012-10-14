@call "%VS110COMNTOOLS%\VsDevCmd.bat"
@del /S /Q .\build\*.*
msbuild .\Maacro\Maacro.csproj /t:Clean
msbuild .\Maacro\Maacro.csproj /t:Build /p:OutDir=..\build\x64\ /p:Configuration=Release 
/p:Platform=x64
msbuild .\Maacro\Maacro.csproj /t:Build /p:OutDir=..\build\x86\ /p:Configuration=Release /p:Platform=x86
@del /S /Q .\build\*.pdb
@del /S /Q .\build\*.xml
pause