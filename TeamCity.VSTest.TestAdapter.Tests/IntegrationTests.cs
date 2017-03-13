namespace TeamCity.VSTest.TestAdapter.Tests
{
    using System;
    using Helpers;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    [Category("Integration")]
    public class IntegrationTests
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        [TestCase(@"IntegrationTests\dotNetCore.XUnit.Tests\dotNetCore.XUnit.Tests.csproj")]
        [TestCase(@"IntegrationTests\dotNetCore.MS.Tests\dotNetCore.MS.Tests.csproj")]
        [TestCase(@"IntegrationTests\dotNet.MS.Tests\dotNet.MS.Tests.csproj")]
        public void ShouldRunTestsWhenUnderTeamCity(string projectName)
        {
            // Given
            var testCommandLine = new CommandLine(
                @"dotnet",
                "test",
                projectName,
                @"/p:VSTestLogger=teamcity;VSTestTestAdapterPath=.");

            // When
            testCommandLine.AddEnvitonmentVariable(EnvironmentInfo.TeamCityProjectEnvVarName, "someproj");
            testCommandLine.TryExecute(out CommandLineResult result).ShouldBe(true);

            // Then
            result.ExitCode.ShouldBe(1);
            result.StdError.Trim().ShouldBe(string.Empty);
            ServiceMessages.GetNumberServiceMessage(result.StdOut).ShouldBeGreaterThan(0);
            ServiceMessages.ResultShouldContainCorrectStructureAndSequence(result.StdOut);
        }

        [Test]
        [TestCase(@"IntegrationTests\dotNetCore.XUnit.Tests\dotNetCore.XUnit.Tests.csproj")]
        [TestCase(@"IntegrationTests\dotNetCore.MS.Tests\dotNetCore.MS.Tests.csproj")]
        [TestCase(@"IntegrationTests\dotNet.MS.Tests\dotNet.MS.Tests.csproj")]
        public void ShouldNotGenerateServiceMessageWhenNotUnderTeamCity(string projectName)
        {
            // Given
            var testCommandLine = new CommandLine(
                @"dotnet",
                "test",
                projectName,
                @"/p:VSTestLogger=teamcity;VSTestTestAdapterPath=.");

            // When
            testCommandLine.TryExecute(out CommandLineResult result).ShouldBe(true);

            // Then
            result.ExitCode.ShouldBe(1);
            result.StdError.Trim().ShouldBe(string.Empty);
            ServiceMessages.GetNumberServiceMessage(result.StdOut).ShouldBe(0);
        }
    }
}