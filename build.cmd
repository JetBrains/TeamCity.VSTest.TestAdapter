SET TEAMCITY_VERSION=10
SET TEAMCITY_PROJECT_NAME=myproj

MSBuild.exe build.proj /t:Build;Test -r /p:Configuration=Release