rem SET VSTEST_HOST_DEBUG=1
rem SET VSTEST_RUNNER_DEBUG=1
rem SET TEAMCITY_PROJECT_NAME=aaa
rem dotnet test IntegrationTests\dotNetCore.XUnit.Tests\dotNetCore.XUnit.Tests.csproj /p:VSTestLogger=teamcity;VSTestTestAdapterPath=.
rem dotnet test IntegrationTests\dotNetCore.MS.Tests\dotNetCore.MS.Tests.csproj /p:VSTestLogger=teamcity;VSTestTestAdapterPath=.
dotnet test IntegrationTests\dotNet.MS.Tests\dotNet.MS.Tests.csproj /p:VSTestLogger=teamcity;VSTestTestAdapterPath=.