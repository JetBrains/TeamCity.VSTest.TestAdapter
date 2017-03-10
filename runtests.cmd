rem SET VSTEST_HOST_DEBUG=1
rem SET VSTEST_RUNNER_DEBUG=1
rem SET TEAMCITY_PROJECT_NAME=aaa
dotnet test IntegrationTests\dotNetCore.XUnit.Tests\dotNetCore.XUnit.Tests.csproj /p:VSTestLogger=teamcity;VSTestTestAdapterPath=.