# TeamCity Test Adapter

Provides the TeamCity integration with test frameworks via the Visual Studio Test Platform or VSTest IDE tools.

[<img src="http://teamcity.jetbrains.com/app/rest/builds/buildType:(id:TeamCityPluginsByJetBrains_TeamCityVSTestTestAdapter_Build)/statusIcon.svg"/>](http://teamcity.jetbrains.com/viewType.html?buildTypeId=TeamCityPluginsByJetBrains_TeamCityVSTestTestAdapter_Build) [<img src="https://www.nuget.org/Content/Logos/nugetlogo.png" height="18">](https://www.nuget.org/packages/TeamCity.VSTest.TestAdapter)

<img src="https://github.com/JetBrains/TeamCity.VSTest.TestAdapter/blob/master/Samples/MS.Tests/Docs/NewTest.gif"/>

For each test project:

* Choose a test framework which supports [Visual Studio Test Platform](https://github.com/Microsoft/vstest).

* Add a NuGet reference to the NuGet package of the selected test framework. For example, [MSTest](https://www.nuget.org/packages/MSTest.TestFramework/), or [XUnit](https://www.nuget.org/packages/xunit/), or others.

* Add a NuGet reference to the NuGet package of the appropriate test adapter. For example, the [MSTest adapter](https://www.nuget.org/packages/MSTest.TestAdapter/), or the [XUnit adapter](https://www.nuget.org/packages/xunit.runner.visualstudio/), or others.

* Add a NuGet reference to the [Visual Studio Test Platform](https://www.nuget.org/packages/Microsoft.NET.Test.Sdk/).

   Alternatively, just create the a test project from the command line using the specified [template](https://docs.microsoft.com/en-us/dotnet/articles/core/tools/dotnet-new):

   ```
   dotnet new mstest
   ```

   or

   ```
   dotnet new xunit
   ```

To support the TeamCity integration:

* Add a NuGet reference to the [TeamCity Test Adapter](https://www.nuget.org/packages/TeamCity.VSTest.TestAdapter) to turn on the TeamCity integration.

## Supported platforms:

* VSTest IDE
* Visual Studio Test Platform 15.0.0

## Visual Studio Test Platform 15.0.0

Presently, Visual Studio has an open and extensible [test platform](https://github.com/Microsoft/vstest) with tests written using various test frameworks and run using a variety of adapters. The Test Platform, from its vantage, resolves the lifecycle of the test into a series of stages – two of which are writing and running the test – with the goal of providing extensibility at each stage.

## IDE VSTest

* To support the TeamCity integration, add a NuGet reference to the [TeamCity Test Adapter](https://www.nuget.org/packages/TeamCity.VSTest.TestAdapter)

* To run tests from the command line, use additional command line arguments: `/TestAdapterPath:.`, `/Logger:teamcity` 

   (1) The first argument points to  the path where the runner can find the assembly of the TeamCity logger (the directory of the testing aseembly).
   
   (2) The second argument points to the [TeamCity service messages](http://confluence.jetbrains.net/display/TCDL/Build+Script+Interaction+with+TeamCity#BuildScriptInteractionwithTeamCity-ServiceMessages) logger.
  
   For example:
   ```
   "C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe" dotNet.XUnit.Tests.dll /Logger:teamcity /TestAdapterPath:.
   ```

## IDE VSTest extensions

* Download the [custom logger](http://teamcity.jetbrains.com/httpAuth/app/rest/builds/buildType:TeamCityPluginsByJetBrains_TeamCityVSTestTestAdapter_Build,pinned:true,status:SUCCESS,branch:master,tags:release/artifacts/content/extensions/TeamCityAdapter.net40.zip)
* Extract the contents of the downloaded archive on the agent machine:
  * for VisualStudio 2017 - to PROGRAM_FILES(x86)\Microsoft Visual Studio\2017\<Edition>\Common7\IDE\CommonExtensions\Microsoft\TestWindow\Extensions
  * for VisualStudio 2015 - to PROGRAM_FILES\Microsoft Visual Studio 14.0\Common7\IDE\CommonExtensions\Microsoft\TestWindow\Extensions
  * for VisualStudio 2013 - to PROGRAM_FILES\Microsoft Visual Studio 12.0\Common7\IDE\CommonExtensions\Microsoft\TestWindow\Extensions
  * for VisualStudio 2012 - to PROGRAM_FILES\Microsoft Visual Studio 11.0\Common7\IDE\CommonExtensions\Microsoft\TestWindow\Extensions
* Check that the custom logger was installed correctly by executing vstest.console.exe /ListLoggers in the console on the agent machine. If the logger was installed correctly, you will see the logger with FriendlyName TeamCity listed: `TeamCity.VSTest.TestAdapter.TeamCityTestLogger URI: logger://teamcity`

See more details in the [Wiki](https://github.com/JetBrains/TeamCity.VSTest.TestAdapter/wiki).

See the [article](https://blogs.msdn.microsoft.com/visualstudioalm/2016/11/29/evolving-the-test-platform-part-3-net-core-convergence-and-cross-plat/) for details on how to create tests using the [Visual Studio Test Platform](https://github.com/Microsoft/vstest).
