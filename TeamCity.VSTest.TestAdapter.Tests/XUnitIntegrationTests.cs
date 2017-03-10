namespace TeamCity.VSTest.TestAdapter.Tests
{
    using System;
    using Helpers;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    [Category("Integration")]
    public class XUnitIntegrationTests
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void ShouldGenerateServiceMessageWhenUnderTeamCityByItself()
        {
            // Given
            var testCommandLine = new CommandLine(
                @"dotnet",
                "test",
                @"IntegrationTests\dotNetCore.XUnit.Tests\dotNetCore.XUnit.Tests.csproj",
                @"/p:VSTestLogger=teamcity;VSTestTestAdapterPath=.");

            // When
            testCommandLine.AddEnvitonmentVariable(EnvironmentInfo.TeamCityProjectEnvVarName, "someproj");
            testCommandLine.TryExecute(out CommandLineResult result).ShouldBe(true);

            // Then
            result.ExitCode.ShouldBe(1);
            result.StdError.Trim().ShouldBe(string.Empty);
            ServiceMessages.ResultShouldContainCorrectStructureAndSequence(result.StdOut);
            result.StdOut.IndexOf("##teamcity[testSuiteStarted name='Test collection for ", StringComparison.Ordinal).ShouldBeGreaterThan(0);
        }

        [Test]
        public void ShouldRunTests()
        {
            // Given
            var testCommandLine = new CommandLine(
                @"dotnet",
                "test",
                @"IntegrationTests\dotNetCore.XUnit.Tests\dotNetCore.XUnit.Tests.csproj",
                @"/p:VSTestLogger=teamcity;VSTestTestAdapterPath=.");

            // When
            testCommandLine.TryExecute(out CommandLineResult result).ShouldBe(true);

            // Then
            result.ExitCode.ShouldBe(1);
            result.StdError.Trim().ShouldBe(string.Empty);
            ServiceMessages.ResultShouldContainCorrectStructureAndSequence(result.StdOut);
        }
    }
}