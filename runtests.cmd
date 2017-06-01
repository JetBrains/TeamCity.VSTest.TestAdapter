rem SET VSTEST_HOST_DEBUG=1
rem SET VSTEST_RUNNER_DEBUG=1
SET TEAMCITY_PROJECT_NAME=myproj

rem /p:VSTestLogger=teamcity;VSTestTestAdapterPath=.

dotnet test IntegrationTests\dotNetCore.XUnit.Tests\dotNetCore.XUnit.Tests.csproj -c=Debug -l=teamcity -a=.
dotnet test IntegrationTests\dotNet.XUnit.Tests\dotNet.XUnit.Tests.csproj -c=Debug -l=teamcity -a=.
dotnet test IntegrationTests\dotNetCore.MS.Tests\dotNetCore.MS.Tests.csproj -c=Debug -l=teamcity -a=.
dotnet test IntegrationTests\dotNet.MS.Tests\dotNet.MS.Tests.csproj -c=Debug -l=teamcity -a=.

"C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe" IntegrationTests\dotNet.MS.Tests\bin\Debug\dotNet.MS.Tests.dll /Logger:TeamCity /TestAdapterPath:. /diag:aa.txt