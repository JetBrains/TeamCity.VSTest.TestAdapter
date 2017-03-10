namespace TeamCity.VSTest.TestAdapter.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using Helpers;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
    using Moq;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    [SuppressMessage("ReSharper", "ImplicitlyCapturedClosure")]
    public class TeamCityTestLoggerTests
    {
        [SetUp]
        public void SetUp()
        {
            _lines.Clear();
            _events = new Events();

            _testCaseFilter = new Mock<ITestCaseFilter>();
            _testCaseFilter.Setup(i => i.IsSupported(It.IsAny<TestCase>())).Returns(true);

            _suiteNameProvider = new Mock<ISuiteNameProvider>();
            _suiteNameProvider.Setup(i => i.GetSuiteName(It.IsAny<string>(), It.IsAny<string>())).Returns<string, string>((baseDir, source) => source);

            var root = new Root(_lines);
            _logger = new TeamCityTestLogger(root, _testCaseFilter.Object, _suiteNameProvider.Object);
            _logger.Initialize(_events, null);
        }

        private readonly List<string> _lines = new List<string>();
        private Events _events;
        private TeamCityTestLogger _logger;
        private Mock<ITestCaseFilter> _testCaseFilter;
        private Mock<ISuiteNameProvider> _suiteNameProvider;

        private static TestResultEventArgs CreateTestResult(
            TestOutcome outcome = TestOutcome.Passed,
            string fullyQualifiedName = "test1",
            string source = "assembly.dll",
            string errorMessage = null,
            string errorStackTrace = null,
            string extensionId = TeamCityTestLogger.ExtensionId)
        {
            return new TestResultEventArgs(
                new TestResult(
                    new TestCase(
                        fullyQualifiedName,
                        new Uri(extensionId),
                        source))
                {
                    Outcome = outcome,
                    Duration = TimeSpan.FromSeconds(1),
                    ErrorMessage = errorMessage,
                    ErrorStackTrace = errorStackTrace
                });
        }

        private static TestRunCompleteEventArgs CreateComplete()
        {
            return new TestRunCompleteEventArgs(
                Mock.Of<ITestRunStatistics>(),
                false,
                false,
                null,
                new Collection<AttachmentSet>(),
                TimeSpan.FromMinutes(1));
        }

        [Test]
        public void ShouldNotProduceAnyMessagesWhenTestCaseFilterFiltersAllMessages()
        {
            // Given
            var testResult = CreateTestResult(TestOutcome.Passed, "test2");

            // When
            _testCaseFilter.Setup(i => i.IsSupported(testResult.Result.TestCase)).Returns(false);
            _events.SendTestResult(testResult);
            _events.SendTestRunComplete(CreateComplete());

            // Then
            _lines.ShouldNotBeEmpty();
        }

        [Test]
        public void ShouldProcessWhenFailedTest()
        {
            // Given

            // When
            _events.SendTestResult(CreateTestResult(TestOutcome.Failed, "test1", "assembly.dll", "errorInfo", "stackTrace"));
            _events.SendTestRunComplete(CreateComplete());

            // Then
            _lines.ShouldBe(new[]
            {
                "+ root"
                , "+ suite assembly.dll"
                , "+ test assembly.dll/test1"
                , "# test assembly.dll/test1 duration 00:00:01"
                , "! test assembly.dll/test1 errorInfo stackTrace"
                , "- test assembly.dll/test1"
                , "- suite assembly.dll"
                , "- root"
            });
        }

        [Test]
        public void ShouldProcessWhenPassedTest()
        {
            // Given

            // When
            _events.SendTestResult(CreateTestResult());
            _events.SendTestRunComplete(CreateComplete());

            // Then
            _lines.ShouldBe(new[]
            {
                "+ root"
                , "+ suite assembly.dll"
                , "+ test assembly.dll/test1"
                , "# test assembly.dll/test1 duration 00:00:01"
                , "- test assembly.dll/test1"
                , "- suite assembly.dll"
                , "- root"
            });
        }

        [Test]
        public void ShouldProcessWhenPassedTestWithMessages()
        {
            // Given
            var testResult = CreateTestResult();

            // When
            testResult.Result.Messages.Add(new TestResultMessage(TestResultMessage.StandardOutCategory, "some text"));
            testResult.Result.Messages.Add(new TestResultMessage(TestResultMessage.AdditionalInfoCategory, "additional text"));
            testResult.Result.Messages.Add(new TestResultMessage(TestResultMessage.DebugTraceCategory, "trace text"));
            testResult.Result.Messages.Add(new TestResultMessage(TestResultMessage.StandardErrorCategory, "error text"));
            _events.SendTestResult(testResult);
            _events.SendTestRunComplete(CreateComplete());


            // Then
            _lines.ShouldBe(new[]
            {
                "+ root"
                , "+ suite assembly.dll"
                , "+ test assembly.dll/test1"
                , "# test assembly.dll/test1 duration 00:00:01"
                , "# test assembly.dll/test1 message some text"
                , "# test assembly.dll/test1 message additional text"
                , "# test assembly.dll/test1 message trace text"
                , "# test assembly.dll/test1 error error text"
                , "- test assembly.dll/test1"
                , "- suite assembly.dll"
                , "- root"
            });
        }

        [Test]
        public void ShouldProcessWhenSeveralPassedTests()
        {
            // Given

            // When
            _events.SendTestResult(CreateTestResult());
            _events.SendTestResult(CreateTestResult(TestOutcome.Passed, "test2"));
            _events.SendTestRunComplete(CreateComplete());

            // Then
            _lines.ShouldBe(new[]
            {
                "+ root"
                , "+ suite assembly.dll"
                , "+ test assembly.dll/test1"
                , "# test assembly.dll/test1 duration 00:00:01"
                , "- test assembly.dll/test1"
                , "+ test assembly.dll/test2"
                , "# test assembly.dll/test2 duration 00:00:01"
                , "- test assembly.dll/test2"
                , "- suite assembly.dll"
                , "- root"
            });
        }

        [Test]
        public void ShouldProcessWhenSeveralPassedTestsInDifferentSuites()
        {
            // Given

            // When
            _events.SendTestResult(CreateTestResult());
            _events.SendTestResult(CreateTestResult(TestOutcome.Passed, "test2"));
            _events.SendTestResult(CreateTestResult(TestOutcome.Passed, "test3", "assembly2.dll"));
            _events.SendTestResult(CreateTestResult(TestOutcome.Passed, "test4", "assembly2.dll"));
            _events.SendTestRunComplete(CreateComplete());

            // Then
            _lines.ShouldBe(new[]
            {
                "+ root"
                , "+ suite assembly.dll"
                , "+ test assembly.dll/test1"
                , "# test assembly.dll/test1 duration 00:00:01"
                , "- test assembly.dll/test1"
                , "+ test assembly.dll/test2"
                , "# test assembly.dll/test2 duration 00:00:01"
                , "- test assembly.dll/test2"
                , "- suite assembly.dll"
                , "+ suite assembly2.dll"
                , "+ test assembly2.dll/test3"
                , "# test assembly2.dll/test3 duration 00:00:01"
                , "- test assembly2.dll/test3"
                , "+ test assembly2.dll/test4"
                , "# test assembly2.dll/test4 duration 00:00:01"
                , "- test assembly2.dll/test4"
                , "- suite assembly2.dll"
                , "- root"
            });
        }

        [Test]
        public void ShouldProcessWhenSkippedTest()
        {
            // Given

            // When
            _events.SendTestResult(CreateTestResult(TestOutcome.Skipped, "test1", "assembly.dll", "reason"));
            _events.SendTestRunComplete(CreateComplete());

            // Then
            _lines.ShouldBe(new[]
            {
                "+ root"
                , "+ suite assembly.dll"
                , "+ test assembly.dll/test1"
                , "# test assembly.dll/test1 duration 00:00:01"
                , "? test assembly.dll/test1 reason"
                , "- test assembly.dll/test1"
                , "- suite assembly.dll"
                , "- root"
            });
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void ShouldProcessWhenSkippedTestWithoutReason(string reason)
        {
            // Given

            // When
            _events.SendTestResult(CreateTestResult(TestOutcome.Skipped, "test1", "assembly.dll", reason));
            _events.SendTestRunComplete(CreateComplete());

            // Then
            _lines.ShouldBe(new[]
            {
                "+ root"
                , "+ suite assembly.dll"
                , "+ test assembly.dll/test1"
                , "# test assembly.dll/test1 duration 00:00:01"
                , "? test assembly.dll/test1"
                , "- test assembly.dll/test1"
                , "- suite assembly.dll"
                , "- root"
            });
        }

        [Test]
        public void ShouldProduceMessagesWhenTestCaseFilterFiltersNotAllMessages()
        {
            // Given
            var testResult1 = CreateTestResult();
            var testResult2 = CreateTestResult(TestOutcome.Passed, "test2");

            // When
            _testCaseFilter.Setup(i => i.IsSupported(testResult1.Result.TestCase)).Returns(false);
            _testCaseFilter.Setup(i => i.IsSupported(testResult2.Result.TestCase)).Returns(true);
            _events.SendTestResult(testResult1);
            _events.SendTestResult(testResult2);
            _events.SendTestRunComplete(CreateComplete());

            // Then
            // Then
            _lines.ShouldBe(new[]
            {
                "+ root"
                , "+ suite assembly.dll"
                , "+ test assembly.dll/test2"
                , "# test assembly.dll/test2 duration 00:00:01"
                , "- test assembly.dll/test2"
                , "- suite assembly.dll"
                , "- root"
            });
        }

        [Test]
        public void ShouldUseSuiteNameProvider()
        {
            // Given

            // When
            _events.SendTestResult(CreateTestResult());
            _events.SendTestRunComplete(CreateComplete());

            // Then
            _suiteNameProvider.Verify(i => i.GetSuiteName(null, "assembly.dll"), Times.Once);
            _suiteNameProvider.Verify(i => i.Reset(), Times.Once);
        }
    }
}