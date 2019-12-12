## Test Adapter for [<img src="https://cdn.worldvectorlogo.com/logos/teamcity.svg" height="20" align="center"/>](https://www.jetbrains.com/teamcity/)

[<img src="http://jb.gg/badges/official.svg"/>](https://confluence.jetbrains.com/display/ALL/JetBrains+on+GitHub) [![NuGet TeamCity.VSTest.TestAdapter](https://buildstats.info/nuget/TeamCity.VSTest.TestAdapter?includePreReleases=false)](https://www.nuget.org/packages/TeamCity.VSTest.TestAdapter) [![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](https://opensource.org/licenses/Apache-2.0) [<img src="http://teamcity.jetbrains.com/app/rest/builds/buildType:(id:TeamCityPluginsByJetBrains_TeamCityVSTestTestAdapter_Build)/statusIcon.svg"/>](http://teamcity.jetbrains.com/viewType.html?buildTypeId=TeamCityPluginsByJetBrains_TeamCityVSTestTestAdapter_Build&guest=1)

Provides the TeamCity integration with test frameworks via the Visual Studio Test Platform or VSTest IDE tools.

<img src="https://github.com/JetBrains/TeamCity.VSTest.TestAdapter/blob/master/Samples/MS.Tests/Docs/NewTest.gif"/>

It is important to note that the above demo works from the command line when the environment variable `TEAMCITY_VERSION` exists with any value during `dotnet` commands. TeamCity automatically specifies this environment variable by the current TeamCity version for each build step.

## Supported platforms:

* Visual Studio Test Platform
* VSTest Console version 12+

## Visual Studio Test Platform

Presently, Visual Studio has an open and extensible [test platform](https://github.com/Microsoft/vstest) with tests written using various test frameworks and run using a variety of adapters. The Test Platform, from its vantage, resolves the lifecycle of the test into a series of stages – two of which are writing and running the test – with the goal of providing extensibility at each stage.

For each test project:

* Add a NuGet reference to the [Visual Studio Test Platform](https://www.nuget.org/packages/Microsoft.NET.Test.Sdk/).
   
   ```
   dotnet add package Microsoft.NET.Test.Sdk
   ```
   
* Add NuGet references to NuGet packages of the selected test framework and coresponding test adapter which supports [Visual Studio Test Platform](https://github.com/Microsoft/vstest). For example:
   * MSTest [Framework](https://www.nuget.org/packages/MSTest.TestFramework/) and [Adapter](https://www.nuget.org/packages/MSTest.TestAdapter/)
   
   ```
   dotnet add package MSTest.TestFramework   
   dotnet add package MSTest.TestAdapter
   ```
   
   * XUnit [Framework](https://www.nuget.org/packages/xunit/) and [Adapter](https://www.nuget.org/packages/xunit.runner.visualstudio/)
   
   ```
   dotnet add package xunit   
   dotnet add package xunit.runner.visualstudio
   ```
   
   * NUnit [Framework](https://www.nuget.org/packages/NUnit/) and [Adapter](https://www.nuget.org/packages/NUnit3TestAdapter/)
   
   ```
   dotnet add package NUnit   
   dotnet add package NUnit3TestAdapter
   ```
   
   * DevTeam [Framework](https://www.nuget.org/packages/DevTeam.TestFramework/) and [Adapter](https://www.nuget.org/packages/DevTeam.TestAdapter/)
   
   ```
   dotnet add package DevTeam.TestFramework   
   dotnet add package DevTeam.TestAdapter
   ```

   or others. 
  
Alternatively to two steps above, you could create the a test project from the command line using the specified [template](https://docs.microsoft.com/en-us/dotnet/articles/core/tools/dotnet-new):

   ```
   dotnet new mstest
   ```

   or

   ```
   dotnet new xunit
   ```

* To support the TeamCity integration add a NuGet reference to the [TeamCity Test Adapter](https://www.nuget.org/packages/TeamCity.VSTest.TestAdapter) to turn on the TeamCity integration.

   ```
   dotnet add package TeamCity.VSTest.TestAdapter
   ```
   
Thus, the final project file could look like the following:

``` xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>    
    <TargetFrameworks>net45;netcoreapp1.0;netcoreapp2.0</TargetFrameworks>    
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="15.8.0" />
    <PackageReference Include="MSTest.TestFramework" Version="1.3.2" />
    <PackageReference Include="MSTest.TestAdapter" Version="1.3.2" />
    <PackageReference Include="TeamCity.VSTest.TestAdapter" Version="1.0.15" />    
  </ItemGroup>  
</Project>
```

## VSTest Console

* To support the TeamCity integration, add a NuGet reference to the [TeamCity Test Adapter](https://www.nuget.org/packages/TeamCity.VSTest.TestAdapter)

* To run tests from the command line, use additional command line arguments: `/TestAdapterPath:.`, `/Logger:teamcity` 

   * The first argument points to  the path where the runner can find the assembly of the TeamCity logger (the directory of the testing aseembly).
   
   * The second argument points to the [TeamCity service messages](http://confluence.jetbrains.net/display/TCDL/Build+Script+Interaction+with+TeamCity#BuildScriptInteractionwithTeamCity-ServiceMessages) logger.
  
For example:
```
"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe" dotNet.XUnit.Tests.dll /Logger:teamcity /TestAdapterPath:.
```
## VSTest extensions

* Download the [custom logger](http://teamcity.jetbrains.com/guestAuth/app/rest/builds/buildType:TeamCityPluginsByJetBrains_TeamCityVSTestTestAdapter_Build,pinned:true,status:SUCCESS,branch:master,tags:release/artifacts/content/TeamCity.VSTest.TestLogger.zip)

* Extract the contents of the downloaded archive on the agent machine:

|Visual Studio|Content directory|Target directory|
|--- | --- | --- |
|2019|vstest15|PROGRAM_FILES(x86)\Microsoft Visual Studio\2019\<Edition>\Common7\IDE\Extensions\TestPlatform\Extensions|
|2017 update 5 onwards|vstest15|PROGRAM_FILES(x86)\Microsoft Visual Studio\2017\<Edition>\Common7\IDE\Extensions\TestPlatform\Extensions|
|2017 till update 4|vstest15|PROGRAM_FILES(x86)\Microsoft Visual Studio\2017\<Edition>\Common7\IDE\CommonExtensions\Microsoft\TestWindow\Extensions|
|2015|vstest14|PROGRAM_FILES\Microsoft Visual Studio 14.0\Common7\IDE\CommonExtensions\Microsoft\TestWindow\Extensions|
|2013|vstest12|PROGRAM_FILES\Microsoft Visual Studio 12.0\Common7\IDE\CommonExtensions\Microsoft\TestWindow\Extensions|
|2012|vstest12|PROGRAM_FILES\Microsoft Visual Studio 11.0\Common7\IDE\CommonExtensions\Microsoft\TestWindow\Extensions|
  
* Check that the custom logger was installed correctly by executing vstest.console.exe /ListLoggers in the console on the agent machine. For instance: 
```
C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\Common7\IDE\Extensions\TestPlatform\vstest.console.exe /ListLoggers
```
If the logger was installed correctly, you will see the logger with FriendlyName TeamCity listed:
```
TeamCity.VSTest.TestLogger.TeamCityTestLogger
   Uri: logger://TeamCity   
   FriendlyName: TeamCity
```

See more details in the [Wiki](https://github.com/JetBrains/TeamCity.VSTest.TestAdapter/wiki).

See the [article](https://blogs.msdn.microsoft.com/visualstudioalm/2016/11/29/evolving-the-test-platform-part-3-net-core-convergence-and-cross-plat/) for details on how to create tests using the [Visual Studio Test Platform](https://github.com/Microsoft/vstest).

## Docker Container Support

To run tests from within a Docker container that is hosted on a machine running the TeamCity agent, the container must have a `TEAMCITY_PROJECT_NAME` or `TEAMCITY_VERSION` environment variable set. Example:

```
docker run --rm -v $PWD:/app -w /app -e TEAMCITY_VERSION microsoft/dotnet:2.1-sdk dotnet test
```

## Known issues

* Tests are not reported for .NET Core xunit test projects when the logging verbosity level is `minimal` or `quiet` because of [issue](https://github.com/xunit/xunit/issues/1706), so try use the verbosity level `normal` or more detailed, for instance:
  * `dotnet test --verbosity normal`
  * `dotnet vstest mytests.dll /logger:console;verbosity=normal`
 