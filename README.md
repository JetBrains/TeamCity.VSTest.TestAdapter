# TeamCity Test Adapter

Provides the TeamCity integration with test frameworks via Visual Studio Test Platform or IDE VSTest tools.

[<img src="http://teamcity.jetbrains.com/app/rest/builds/buildType:(id:TeamCityPluginsByJetBrains_TeamCityVSTestTestAdapter_Build)/statusIcon.svg"/>](http://teamcity.jetbrains.com/viewType.html?buildTypeId=TeamCityPluginsByJetBrains_TeamCityVSTestTestAdapter_Build) [<img src="https://www.nuget.org/Content/Logos/nugetlogo.png" height="18">](https://www.nuget.org/packages/TeamCity.VSTest.TestAdapter)

## Supported platforms:
* IDE VSTest
* Visual Studio Test Platform 15.0.0

## IDE VSTest

* To support the TeamCity integration, add the NuGet reference to the [TeamCity Test Adapter](https://www.nuget.org/packages/TeamCity.VSTest.TestAdapter)

* To run tests from the command line, use additional command line arguments `/TestAdapterPath:. /Logger:teamcity` 

   (1) The first argument points to find the path where the runner might find the assembly of TeamCity logger (the directory of testing aseembly).
   
   (2) The second argument specifies to use [TeamCity service messages](http://confluence.jetbrains.net/display/TCDL/Build+Script+Interaction+with+TeamCity#BuildScriptInteractionwithTeamCity-ServiceMessages) logger.
  
   For example:
   ```
   "C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe" dotNet.XUnit.Tests.dll /Logger:teamcity /TestAdapterPath:.
   ```
 
## Visual Studio Test Platform 15.0.0

Presently, Visual Studio has an open and extensible [test platform](https://github.com/Microsoft/vstest) with tests being written using various test frameworks and run using a variety of adapters. The Test Platform, from its vantage, resolves the lifecycle of the test into a series of stages – two of which are writing and running the test – with the goal of providing extensibility at each stage.

See the [article](https://blogs.msdn.microsoft.com/visualstudioalm/2016/11/29/evolving-the-test-platform-part-3-net-core-convergence-and-cross-plat/) for details how to create tests using [Visual Studio Test Platform](https://github.com/Microsoft/vstest).

For each test project:

* Choose a test framework which supports [Visual Studio Test Platform](https://github.com/Microsoft/vstest).

* Add the NuGet reference to the NuGet package of the selected test framework. For example [MSTest](https://www.nuget.org/packages/MSTest.TestFramework/) or [XUnit](https://www.nuget.org/packages/xunit/) or others.

* Add the NuGet reference to the NuGet package of the appropriate test adapter. For example [MSTest adapter](https://www.nuget.org/packages/MSTest.TestAdapter/) or [XUnit adapter](https://www.nuget.org/packages/xunit.runner.visualstudio/) or others.

* Add the NuGet reference to the [Visual Studio Test Platform](https://www.nuget.org/packages/Microsoft.NET.Test.Sdk/).

   Alternatively, just create the a test project from the command line using the specified [template](https://docs.microsoft.com/en-us/dotnet/articles/core/tools/dotnet-new):

   ```
   dotnet new mstest
   ```

   or

   ```
   dotnet new xunit
   ```

To support the TeamCity integration:

* Add the NuGet reference to the [TeamCity Test Adapter](https://www.nuget.org/packages/TeamCity.VSTest.TestAdapter) to turn on the TeamCity integration.

<img src="https://github.com/JetBrains/TeamCity.VSTest.TestAdapter/blob/master/Samples/MS.Tests/Docs/NewTest.gif"/>

See more details in [Wiki](https://github.com/JetBrains/TeamCity.VSTest.TestAdapter/wiki).
