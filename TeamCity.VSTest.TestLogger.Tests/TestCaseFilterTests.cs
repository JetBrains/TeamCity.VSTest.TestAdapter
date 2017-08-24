namespace TeamCity.VSTest.TestLogger.Tests
{
    using System;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Shouldly;
    using Xunit;

    public class TestCaseFilterTests
    {
        [Theory]
        // Some messages from some executor
        [InlineData("executor://some", "aaa", true)]
        [InlineData("abc://some", "bbb", true)]
        // Empty message from XUnit
        [InlineData("executor://xunit/VsTestRunner2", "", true)]
        // Service message from XUnit but without xUnit prefix
        [InlineData("executor://xunit/VsTestRunner2", "##teamcity[abc", true)]
        // Service message from XUnit but without xUnit prefix
        [InlineData("executor://xunit/VsTestRunner2", "   ##teamcity[ abc", true)]
        // Service message from XUnit with xUnit prefix
        [InlineData("executor://xunit/VsTestRunner2", "[xUnit.net 00:00:00.8998020] ##teamcity[testSuiteFinished name", false)]
        // Service message from XUnit but from some executor
        [InlineData("abc://some", "[xUnit.net 00:00:00.8998020] ##teamcity[testSuiteFinished name", true)]
        // Service message from XUnit with invalid position of xUnit prefix
        [InlineData("executor://xunit/VsTestRunner2", "abc[xUnit.net 00:00:00.8998020] ##teamcity[testSuiteFinished name", true)]
        [InlineData("executor://xunit/vstestrunner2", "abc[xUnit.net 00:00:00.8998020] ##teamcity[testSuiteFinished name", true)]
        [InlineData("executor://xunit/VsTestRunner2", "abc[xunit.net 00:00:00.8998020] ##teamcity[testSuiteFinished name", true)]
        [InlineData("executor://Xunit/vstestRunner2", "abc[XUNIT.net 00:00:00.8998020] ##TeamCity[abc", true)]

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