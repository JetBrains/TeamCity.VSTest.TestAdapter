namespace TeamCity.VSTest.TestAdapter.Tests
{
    using Helpers;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    [Category("Integration")]
    public class IntegrationTests
    {
        public static object[] TestProjects =
        {
            new object[] { @"IntegrationTests\dotNetCore.XUnit.Tests\dotNetCore.XUnit.Tests.csproj" },
            new object[] { @"IntegrationTests\dotNet.XUnit.Tests\dotNet.XUnit.Tests.csproj" },
            new object[] { @"IntegrationTests\dotNetCore.MS.Tests\dotNetCore.MS.Tests.csproj" },
            new object[] { @"IntegrationTests\dotNet.MS.Tests\dotNet.MS.Tests.csproj" }
        };

        [SetUp]
        public void SetUp()
        {
        }

        [Test, TestCaseSource(nameof(TestProjects))]
        public void ShouldProduceServiceMessagesWhenUnderTeamCity(string projectName)
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
            ServiceMessages.GetNumberServiceMessage(result.StdOut).ShouldBe(10);
            ServiceMessages.ResultShouldContainCorrectStructureAndSequence(result.StdOut);
        }

        [Test, TestCaseSource(nameof(TestProjects))]
        public void ShouldNotProduceServiceMessageWhenItIsNotUnderTeamCity(string projectName)
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