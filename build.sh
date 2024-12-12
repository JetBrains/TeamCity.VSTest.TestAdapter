TEAMCITY_VERSION=10
TEAMCITY_PROJECT_NAME=myproj
dotnet msbuild build.proj /t:Build;Test -r /p:Configuration=Release