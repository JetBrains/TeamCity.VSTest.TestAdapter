SET TEAMCITY_PROJECT_NAME=myproj
dotnet restore
"C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe" Debug\netcoreapp1.0\dotNetCore.MS.Tests.dll /Logger:teamcity /TestAdapterPath:.