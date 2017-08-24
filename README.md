# TeamCity Test Adapter

Provides the TeamCity integration with test frameworks via the Visual Studio Test Platform or VSTest IDE tools.

[<img src="http://teamcity.jetbrains.com/app/rest/builds/buildType:(id:TeamCityPluginsByJetBrains_TeamCityVSTestTestAdapter_Build)/statusIcon.svg"/>](http://teamcity.jetbrains.com/viewType.html?buildTypeId=TeamCityPluginsByJetBrains_TeamCityVSTestTestAdapter_Build) [<img src="https://www.nuget.org/Content/Logos/nugetlogo.png" height="18">](https://www.nuget.org/packages/TeamCity.VSTest.TestAdapter)

<img src="https://github.com/JetBrains/TeamCity.VSTest.TestAdapter/blob/master/Samples/MS.Tests/Docs/NewTest.gif"/>

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
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="15.3.0" />
    <PackageReference Include="MSTest.TestFramework" Version="1.1.18" />
    <PackageReference Include="MSTest.TestAdapter" Version="1.1.18" />
    <PackageReference Include="TeamCity.VSTest.TestAdapter" Version="1.0.2" />    
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

* Download the [custom logger](http://teamcity.jetbrains.com/httpAuth/app/rest/builds/buildType:TeamCityPluginsByJetBrains_TeamCityVSTestTestAdapter_Build,pinned:true,status:SUCCESS,branch:master,tags:release/artifacts/content/TeamCity.VSTest.TestLogger.zip)

* Extract the contents of the downloaded archive on the agent machine:

  * for VisualStudio 2017 - to PROGRAM_FILES(x86)\Microsoft Visual Studio\2017\<Edition>\Common7\IDE\CommonExtensions\Microsoft\TestWindow\Extensions
  
  * for VisualStudio 2015 - to PROGRAM_FILES\Microsoft Visual Studio 14.0\Common7\IDE\CommonExtensions\Microsoft\TestWindow\Extensions
  
  * for VisualStudio 2013 - to PROGRAM_FILES\Microsoft Visual Studio 12.0\Common7\IDE\CommonExtensions\Microsoft\TestWindow\Extensions
  
  * for VisualStudio 2012 - to PROGRAM_FILES\Microsoft Visual Studio 11.0\Common7\IDE\CommonExtensions\Microsoft\TestWindow\Extensions
  
* Check that the custom logger was installed correctly by executing vstest.console.exe /ListLoggers in the console on the agent machine. If the logger was installed correctly, you will see the logger with FriendlyName TeamCity listed: `TeamCity.VSTest.TestLogger.TeamCityTestLogger URI: logger://teamcity`

See more details in the [Wiki](https://github.com/JetBrains/TeamCity.VSTest.TestAdapter/wiki).

See the [article](https://blogs.msdn.microsoft.com/visualstudioalm/2016/11/29/evolving-the-test-platform-part-3-net-core-convergence-and-cross-plat/) for details on how to create tests using the [Visual Studio Test Platform](https://github.com/Microsoft/vstest).
