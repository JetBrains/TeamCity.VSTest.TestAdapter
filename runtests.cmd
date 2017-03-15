rem SET VSTEST_HOST_DEBUG=1
rem SET VSTEST_RUNNER_DEBUG=1

rem Integration
rem /p:VSTestLogger=teamcity;VSTestTestAdapterPath=.

rem dotnet test IntegrationTests\dotNetCore.XUnit.Tests\dotNetCore.XUnit.Tests.csproj -c=Release -l=teamcity -a=.
rem dotnet test IntegrationTests\dotNet.XUnit.Tests\dotNet.XUnit.Tests.csproj -c=Release -l=teamcity -a=.
rem dotnet test IntegrationTests\dotNetCore.MS.Tests\dotNetCore.MS.Tests.csproj -c=Release -l=teamcity -a=.
rem dotnet test IntegrationTests\dotNet.MS.Tests\dotNet.MS.Tests.csproj -c=Release -l=teamcity -a=.

rem Samples
pushd Samples\dotNet.XUnit.Tests
dotnet restore dotNet.XUnit.Tests.csproj
dotnet test dotNet.XUnit.Tests.csproj -c=Release -l=teamcity -a=.
popd

rem dotnet restore Samples\dotNet.XUnit.Tests\dotNet.XUnit.Tests.csproj
rem dotnet test Samples\dotNet.XUnit.Tests\dotNet.XUnit.Tests.csproj -c=Release -l=teamcity -a=.
rem dotnet restore Samples\dotNetCore.MS.Tests\dotNetCore.MS.Tests.csproj
rem dotnet test Samples\dotNetCore.MS.Tests\dotNetCore.MS.Tests.csproj -c=Release -l=teamcity -a=.

rem "C:\Program Files\dotnet\dotnet.exe" test Samples\dotNet.XUnit.Tests\dotNet.XUnit.Tests.csproj -c=Release -l=teamcity -a=.
rem "C:\Program Files\dotnet\dotnet.exe" test Samples\dotNetCore.MS.Tests\dotNetCore.MS.Tests.csproj -c=Release -l=teamcity -a=.