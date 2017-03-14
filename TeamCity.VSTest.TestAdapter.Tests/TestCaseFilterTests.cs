namespace TeamCity.VSTest.TestAdapter.Tests
{
    using System;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    public class TestCaseFilterTests
    {
        [Test]
        [TestCase("executor://some", "", true)]
        [TestCase("abc://some", "", true)]
        [TestCase("executor://xunit/VsTestRunner2", "", true)]
        [TestCase("executor://xunit/VsTestRunner2", TestCaseFilter.TeamcityPrefix + " abc", false)]
        [TestCase("executor://xunit/VsTestRunner2", "   " + TestCaseFilter.TeamcityPrefix + " abc", false)]
        [TestCase("executor://xunit/VsTestRunner2", "[xUnit.net 00:00:00.8998020] " + TestCaseFilter.TeamcityPrefix + "testSuiteFinished name", false)]

        public void ShouldFilter(string executorUri, string messagesStr, bool expectedIsSupported)
        {
            // Given
            var filter = CreateInstance();
            var testCase = new TestCase {ExecutorUri = new Uri(executorUri)};
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