@echo off
if "%1" == "" goto Usage

pushd ObjectBuilder
call build %1
popd

if errorlevel 1 goto End

copy ObjectBuilder\ObjectBuilder\bin\Release\ObjectBuilder.??? 3rdParty > nul
copy ObjectBuilder\ObjectBuilder.EventBroker\bin\Release\ObjectBuilder.EventBroker.??? 3rdParty > nul
copy ObjectBuilder\ObjectBuilder.Injection\bin\Release\ObjectBuilder.Injection.??? 3rdParty > nul
copy ObjectBuilder\ObjectBuilder.Interception\bin\Release\ObjectBuilder.Interception.??? 3rdParty > nul

pushd CodePlexContainer
call build %1
popd

goto End

:Usage
echo Usage: build [target]
echo Where: target = one of Build, Test, Clean, or Cruise

:End