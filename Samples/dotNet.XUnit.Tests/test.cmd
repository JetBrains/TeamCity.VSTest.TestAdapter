dotnet restore
dotnet build
SET TEAMCITY_PROJECT_NAME=somename
dotnet test /p:VSTestLogger=teamcity;VSTestTestAdapterPath=.