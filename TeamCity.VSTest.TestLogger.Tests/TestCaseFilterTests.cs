namespace TeamCity.VSTest.TestLogger.Tests
{
    using System;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Shouldly;
    using Xunit;

    public class TestCaseFilterTests
    {
        [Theory]
        [InlineData("executor://some", "", true)]
        [InlineData("abc://some", "", true)]
        [InlineData("executor://xunit/VsTestRunner2", "", true)]
        [InlineData("executor://xunit/VsTestRunner2", TestCaseFilter.TeamcityPrefix + " abc", false)]
        [InlineData("executor://xunit/VsTestRunner2", "   " + TestCaseFilter.TeamcityPrefix + " abc", false)]
        [InlineData("executor://xunit/VsTestRunner2", "[xUnit.net 00:00:00.8998020] " + TestCaseFilter.TeamcityPrefix + "testSuiteFinished name", false)]

        public void ShouldFilter(string executorUri, string messagesStr, bool expectedIsSupported)
        {
            // Given
            var filter = CreateInstance();
            var testCase = new TestCase("abc", new Uri(executorUri), "aaa.dll");
            var messages = messagesStr.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);

            // When
            foreach (var message in messages)
            {
                filter.RegisterOutputMessage(message);
            }
            
            var actualIsSupported = filter.IsSupported(testCase);

            // Then
            actualIsSupported.ShouldBe(expectedIsSupported);
        }

        private static TestCaseFilter CreateInstance()
        {
            return new TestCaseFilter();
        }
    }
}