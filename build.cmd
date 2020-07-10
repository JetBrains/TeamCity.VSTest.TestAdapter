SET TEAMCITY_VERSION=10
SET TEAMCITY_PROJECT_NAME=myproj

msbuild build.proj /t:Build;Test /p:Configuration=Release