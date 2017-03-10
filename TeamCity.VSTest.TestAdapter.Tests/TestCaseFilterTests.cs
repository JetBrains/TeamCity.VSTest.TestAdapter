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
        [TestCase(true, "executor://some", true)]
        [TestCase(true, "abc://some", true)]
        [TestCase(false, "executor://some", true)]
        [TestCase(false, "abc://some", true)]
        [TestCase(true, "executor://xunit/VsTestRunner", false)]
        [TestCase(false, "executor://xunit/VsTestRunner", true)]
        [TestCase(true, "executor://xunit/VsTestRunner2", false)]
        [TestCase(false, "executor://xunit/VsTestRunner2", true)]
        public void Should(bool isUnderTeamCity, string executorUri, bool expectedIsSupported)
        {
            // Given
            _environmentInfo.SetupGet(i => i.IsUnderTeamCity).Returns(isUnderTeamCity);
            var filter = CreateInstance();
            var testCase = new TestCase {ExecutorUri = new Uri(executorUri)};

            // When
            var actualIsSupported = filter.IsSupported(testCase);

            // Then
            actualIsSupported.ShouldBe(expectedIsSupported);
        }
    }
}