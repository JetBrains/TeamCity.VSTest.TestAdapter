namespace TeamCity.VSTest.TestAdapter.Tests
{
    using System;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Moq;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    public class TestCaseFilterTests
    {
        [SetUp]
        public void SetUp()
        {
            _environmentInfo = new Mock<IEnvironmentInfo>();
        }

        private Mock<IEnvironmentInfo> _environmentInfo;

        private TestCaseFilter CreateInstance()
        {
            return new TestCaseFilter(_environmentInfo.Object);
        }

        [Test]
        [TestCase(true, "executor://some", "", true)]
        [TestCase(true, "abc://some", "", true)]
        [TestCase(true, "executor://xunit/VsTestRunner2", "", true)]
        [TestCase(true, "executor://xunit/VsTestRunner2", TestCaseFilter.TeamcityPrefix + " abc", false)]
        [TestCase(true, "executor://xunit/VsTestRunner2", "   " + TestCaseFilter.TeamcityPrefix + " abc", false)]
        [TestCase(true, "executor://xunit/VsTestRunner2", "[xUnit.net 00:00:00.8998020] " + TestCaseFilter.TeamcityPrefix + "testSuiteFinished name", false)]

        [TestCase(false, "executor://some", "", false)]
        [TestCase(false, "abc://some", "", false)]
        [TestCase(false, "executor://xunit/VsTestRunner2", "", false)]
        [TestCase(false, "executor://xunit/VsTestRunner2", TestCaseFilter.TeamcityPrefix + " abc", false)]
        [TestCase(false, "executor://xunit/VsTestRunner2", "[xUnit.net 00:00:00.8998020] " + TestCaseFilter.TeamcityPrefix + "testSuiteFinished name", false)]
        public void ShouldFilter(bool isUnderTeamCity, string executorUri, string messagesStr, bool expectedIsSupported)
        {
            // Given
            _environmentInfo.SetupGet(i => i.IsUnderTeamCity).Returns(isUnderTeamCity);
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
    }
}