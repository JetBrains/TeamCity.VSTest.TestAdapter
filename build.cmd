SET TEAMCITY_VERSION=10
SET TEAMCITY_PROJECT_NAME=myproj

msbuild build.proj /t:Test /p:Configuration=Debug
rem msbuild build.proj /t:Build /p:Configuration=Release
rem copy TeamCity.VSTest.TestLogger\bin\Debug\TeamCity.VSTest.TestAdapter.1.0.2.nupkg C:\Downloads