namespace TeamCity.VSTest.TestAdapter.Tests
{
    using Helpers;
    using Shouldly;
    using Xunit;

    public class IntegrationTests
    {
        
        [Theory]
        [InlineData(@"IntegrationTests\dotNetCore.XUnit.Tests\dotNetCore.XUnit.Tests.csproj")]
        [InlineData(@"IntegrationTests\dotNet.XUnit.Tests\dotNet.XUnit.Tests.csproj")]
        [InlineData(@"IntegrationTests\dotNetCore.MS.Tests\dotNetCore.MS.Tests.csproj")]
        [InlineData(@"IntegrationTests\dotNet.MS.Tests\dotNet.MS.Tests.csproj")]

        public void ShouldProduceServiceMessages(string projectName)
        {
            // Given
            var testCommandLine = new CommandLine(
                @"dotnet",
                "test",
                projectName,
                "-l=teamcity",
                "-a=.",
#if DEBUG
                "-c=Debug"
#else
                "-c=Release"
#endif
                );

            // When
            testCommandLine.TryExecute(out CommandLineResult result).ShouldBe(true);

            // Then
            result.ExitCode.ShouldBe(1);
            result.StdError.Trim().ShouldBe(string.Empty);
            ServiceMessages.GetNumberServiceMessage(result.StdOut).ShouldBe(10);
            ServiceMessages.ResultShouldContainCorrectStructureAndSequence(result.StdOut);
        }
    }
}