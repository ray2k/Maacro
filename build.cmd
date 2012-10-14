@call "%VS110COMNTOOLS%\VsDevCmd.bat"
@rmdir /S /Q .\build\*.*
msbuild .\Maacro\Maacro.csproj /t:Clean
msbuild .\Maacro\Maacro.csproj /t:Build /p:SolutionDir=%CD% /p:Platform=x64 /p:OutDir=..\build\x64\ /p:Configuration=Release
msbuild .\Maacro\Maacro.csproj /t:Build /p:SolutionDir=%CD% /p:Platform=x86 /p:OutDir=..\build\x86\ /p:Configuration=Release
@del /S /Q .\build\*.pdb
@del /S /Q .\build\*.xml
@del /S /Q .\build\*.zip
@rmdir /S /Q .\build\x86\de\
@rmdir /S /Q .\build\x86\es\
@rmdir /S /Q .\build\x86\fr\
@rmdir /S /Q .\build\x86\it\
@rmdir /S /Q .\build\x86\ja\
@rmdir /S /Q .\build\x86\ko\
@rmdir /S /Q .\build\x86\zh-Hans\
@rmdir /S /Q .\build\x86\zh-Hant\
@rmdir /S /Q .\build\x86\de\
@rmdir /S /Q .\build\x86\es\
@rmdir /S /Q .\build\x86\fr\
@rmdir /S /Q .\build\x86\it\
@rmdir /S /Q .\build\x86\ja\
@rmdir /S /Q .\build\x86\ko\
@rmdir /S /Q .\build\x86\zh-Hans\
@rmdir /S /Q .\build\x86\zh-Hant\
@rmdir /S /Q .\build\x64\de\
@rmdir /S /Q .\build\x64\es\
@rmdir /S /Q .\build\x64\fr\
@rmdir /S /Q .\build\x64\it\
@rmdir /S /Q .\build\x64\ja\
@rmdir /S /Q .\build\x64\ko\
@rmdir /S /Q .\build\x64\zh-Hans\
@rmdir /S /Q .\build\x64\zh-Hant\
@rmdir /S /Q .\build\x64\de\
@rmdir /S /Q .\build\x64\es\
@rmdir /S /Q .\build\x64\fr\
@rmdir /S /Q .\build\x64\it\
@rmdir /S /Q .\build\x64\ja\
@rmdir /S /Q .\build\x64\ko\
@rmdir /S /Q .\build\x64\zh-Hans\
@rmdir /S /Q .\build\x64\zh-Hant\
@copy /Y .\build\x86\en\*.dll .\build\x86\*.dll
@copy /Y .\build\x64\en\*.dll .\build\x64\*.dll
@rmdir /S /Q .\build\x86\en\
@rmdir /S /Q .\build\x64\en\

.\Tools\7za a .\Build\Maacro-1.0.0-x86.zip -r .\build\x86\*.*
.\Tools\7za a .\Build\Maacro-1.0.0-x64.zip -r .\build\x64\*.*
pause