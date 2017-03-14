dotnet restore
SET TEAMCITY_PROJECT_NAME=somename
dotnet test /p:VSTestLogger=teamcity;VSTestTestAdapterPath=.