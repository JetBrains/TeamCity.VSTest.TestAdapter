SET TEAMCITY_PROJECT_NAME=myproj
nuget restore ..\Samples.sln
msbuild dotNet.XUnit.Tests.csproj
"C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe" bin\Debug\dotNet.XUnit.Tests.dll /Logger:teamcity /TestAdapterPath:.