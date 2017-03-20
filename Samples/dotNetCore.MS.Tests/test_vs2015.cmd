SET TEAMCITY_PROJECT_NAME=myproj
dotnet restore
msbuild dotNetCore.MS.Tests.csproj
"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe" bin\Debug\net45\dotNetCore.MS.Tests.dll /Logger:teamcity /TestAdapterPath:.