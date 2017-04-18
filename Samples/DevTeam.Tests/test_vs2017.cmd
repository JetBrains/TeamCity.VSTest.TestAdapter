SET TEAMCITY_PROJECT_NAME=myproj
dotnet restore
msbuild dotNetCore.MS.Tests.csproj
"C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe" bin\Debug\net45\DevTeam.Tests.dll /Logger:teamcity /TestAdapterPath:.