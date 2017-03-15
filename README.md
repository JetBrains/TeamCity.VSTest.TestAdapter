# TeamCity Test Adapter

[<img src="http://teamcity.jetbrains.com/app/rest/builds/buildType:(id:TeamCityPluginsByJetBrains_TeamCityVSTestTestAdapter_Build)/statusIcon"/>](http://teamcity.jetbrains.com/viewType.html?buildTypeId=TeamCityPluginsByJetBrains_TeamCityVSTestTestAdapter_Build) [<img src="https://www.nuget.org/Content/Logos/nugetlogo.png" height="18">](https://www.nuget.org/packages/TeamCity.VSTest.TestAdapter)

## Visual Studio Test Platform

Presently, Visual Studio has an open and extensible [test platform](https://github.com/Microsoft/vstest) with tests being written using various test frameworks and run using a variety of adapters. The Test Platform, from its vantage, resolves the lifecycle of the test into a series of stages – two of which are writing and running the test – with the goal of providing extensibility at each stage.

See the [article](https://blogs.msdn.microsoft.com/visualstudioalm/2016/11/29/evolving-the-test-platform-part-3-net-core-convergence-and-cross-plat/) for details how to create tests using [Visual Studio Test Platform](https://github.com/Microsoft/vstest).

## Usage

TeamCity Test Adapter provides the [TeamCity](https://www.jetbrains.com/teamcity/) integration with test frameworks via [Visual Studio Test Platform](https://github.com/Microsoft/vstest) mentioned above.

For each test project:

* Choose a test framework which supports [Visual Studio Test Platform](https://github.com/Microsoft/vstest).

* Add reference to the NuGet package of the selected test framework. For example [MSTest](https://www.nuget.org/packages/MSTest.TestFramework/) or [XUnit](https://www.nuget.org/packages/xunit/) or others.

* Add reference to the NuGet package of the appropriate test adapter. For example [MSTest adapter](https://www.nuget.org/packages/MSTest.TestAdapter/) or [XUnit adapter](https://www.nuget.org/packages/xunit.runner.visualstudio/) or others.

* Add reference to the [Visual Studio Test Platform](https://www.nuget.org/packages/Microsoft.NET.Test.Sdk/).

To support the TeamCity integration:

* Add reference to the [TeamCity Test Adapter](https://www.nuget.org/packages/TeamCity.VSTest.TestAdapter) to turn on the TeamCity integration.

## Example of test solution

[This](https://github.com/JetBrains/TeamCity.VSTest.TestAdapter/tree/master/Samples) solution contains 2 projects:

* [.net 4.5 XUnit tests](https://github.com/JetBrains/TeamCity.VSTest.TestAdapter/tree/master/Samples/dotNet.XUnit.Tests), to run run tests do the [following](https://github.com/JetBrains/TeamCity.VSTest.TestAdapter/blob/master/Samples/dotNet.XUnit.Tests/test.cmd):

```
nuget restore ..\Samples.sln
dotnet test -l=teamcity
```

* [netcoreapp 1.0 MS tests](https://github.com/JetBrains/TeamCity.VSTest.TestAdapter/tree/master/Samples/dotNetCore.MS.Tests), to run run tests the [following](https://github.com/JetBrains/TeamCity.VSTest.TestAdapter/blob/master/Samples/dotNetCore.MS.Tests/test.cmd):

```
dotnet restore
dotnet test -l=teamcity -a=.
```
It is important to note that in this case you should additionally specify a directory where logger should be found (an directory of testing assembly here).

The project file for this project looks like:

```xml
<Project Sdk="Microsoft.NET.Sdk">
...
  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="15.0.0" />
    <PackageReference Include="MSTest.TestAdapter" Version="..." />
    <PackageReference Include="MSTest.TestFramework" Version="..." />
    <PackageReference Include="TeamCity.VSTest.TestAdapter" Version="..." />    
  </ItemGroup>
...
</Project>

```

## Example of build project

[<img src="http://teamcity.jetbrains.com/app/rest/builds/buildType:(id:TeamCityPluginsByJetBrains_TeamCityVSTestTestAdapter_Samples)/statusIcon"/>](http://teamcity.jetbrains.com/viewType.html?buildTypeId=TeamCityPluginsByJetBrains_TeamCityVSTestTestAdapter_Samples)
