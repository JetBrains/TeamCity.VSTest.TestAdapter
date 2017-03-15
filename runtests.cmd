rem SET VSTEST_HOST_DEBUG=1
rem SET VSTEST_RUNNER_DEBUG=1

rem Integration
rem /p:VSTestLogger=teamcity;VSTestTestAdapterPath=.

dotnet test IntegrationTests\dotNetCore.XUnit.Tests\dotNetCore.XUnit.Tests.csproj -c=Release -l=teamcity -a=.
dotnet test IntegrationTests\dotNet.XUnit.Tests\dotNet.XUnit.Tests.csproj -c=Release -l=teamcity -a=.
dotnet test IntegrationTests\dotNetCore.MS.Tests\dotNetCore.MS.Tests.csproj -c=Release -l=teamcity -a=.
dotnet test IntegrationTests\dotNet.MS.Tests\dotNet.MS.Tests.csproj -c=Release -l=teamcity -a=.