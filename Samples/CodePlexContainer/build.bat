@echo off
if "%1" == "" goto Usage
goto BuildMe

:Usage
echo Usage: build [target]
echo Where: target = one of Build, Test, Clean, or Cruise
goto End

:BuildMe
%windir%\Microsoft.NET\Framework\v2.0.50727\MSBuild.exe DependencyInjection.msbuild /logger:Kobush.Build.Logging.XmlLogger,%CodePlex3rdParty%\Kobush.Build.dll;BuildResults.xml /p:BuildTarget=%1

:End