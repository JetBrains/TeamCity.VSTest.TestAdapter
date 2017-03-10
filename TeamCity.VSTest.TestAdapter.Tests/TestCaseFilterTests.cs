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
        }

        [Test]
        [TestCase("executor://some", true)]
        [TestCase("abc://some", true)]
        [TestCase("executor://xunit/VsTestRunner", false)]
        [TestCase("executor://xunit/VsTestRunner2", false)]
        public void Should(string executorUri, bool expectedIsSupported)
        {
            // Given
            var filter = CreateInstance();
            var testCase = new TestCase() {ExecutorUri = new Uri(executorUri)};

            // When
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
