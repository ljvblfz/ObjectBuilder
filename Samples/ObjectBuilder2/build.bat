@echo off
if "%1" == "" goto Usage
if "%CodePlex3rdParty%" == "" goto BuildNoLogging
goto Build

:Usage
echo Usage: build [target]
echo Where: target = one of Build, Test, Clean, or Cruise
goto End

:BuildNoLogging
%windir%\Microsoft.NET\Framework\v2.0.50727\MSBuild.exe ObjectBuilder.msbuild /t:%*
goto End

:Build
%windir%\Microsoft.NET\Framework\v2.0.50727\MSBuild.exe ObjectBuilder.msbuild /logger:Kobush.Build.Logging.XmlLogger,%CodePlex3rdParty%\Kobush.Build.dll;BuildResults.xml /t:%*

:End
