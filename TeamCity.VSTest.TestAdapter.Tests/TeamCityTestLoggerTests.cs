namespace TeamCity.VSTest.TestAdapter.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
    using NUnit.Framework;

    using Shouldly;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
    using Moq;

    [TestFixture]
    [SuppressMessage("ReSharper", "ImplicitlyCapturedClosure")]
    public class TeamCityTestLoggerTests
    {
        private readonly List<string> _lines = new List<string>();
        private Events _events;
        private TeamCityTestLogger _logger;

        [SetUp]
        public void SetUp()
        {
            _lines.Clear();
            _events = new Events();
            var root = new Root(_lines);
            _logger = new TeamCityTestLogger(root);
            _logger.Initialize(_events, (string)null);
        }

        [Test]
        public void ShouldProcessWhenPassedTest()
        {
            // Given
            
            // When
            _events.SendTestResult(CreateTestResult());

            // Then
            _lines.ShouldBe(new[]
            {
                 "+ root"
                ,"+ suite assembly.dll"
                ,"+ test assembly.dll/test1"
                ,"# test assembly.dll/test1 duration 00:00:01"
                ,"- test assembly.dll/test1"
            });
        }

        [Test]
        public void ShouldProcessWhenFailedTest()
        {
            // Given

            // When
            _events.SendTestResult(CreateTestResult(TestOutcome.Failed, "test1", "assembly.dll", "errorInfo", "stackTrace"));

            // Then
            _lines.ShouldBe(new[]
            {
                 "+ root"
                ,"+ suite assembly.dll"
                ,"+ test assembly.dll/test1"
                ,"# test assembly.dll/test1 duration 00:00:01"
                ,"! test assembly.dll/test1 errorInfo stackTrace"
                ,"- test assembly.dll/test1"
            });
        }

        [Test]
        public void ShouldProcessWhenSkippedTest()
        {
            // Given

            // When
            _events.SendTestResult(CreateTestResult(TestOutcome.Skipped, "test1", "assembly.dll", "reason"));

            // Then
            _lines.ShouldBe(new[]
            {
                 "+ root"
                ,"+ suite assembly.dll"
                ,"+ test assembly.dll/test1"
                ,"# test assembly.dll/test1 duration 00:00:01"
                ,"? test assembly.dll/test1 reason"
                ,"- test assembly.dll/test1"
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

            // Then
            _lines.ShouldBe(new[]
            {
                 "+ root"
                ,"+ suite assembly.dll"
                ,"+ test assembly.dll/test1"
                ,"# test assembly.dll/test1 duration 00:00:01"
                ,"? test assembly.dll/test1"
                ,"- test assembly.dll/test1"
            });
        }

        [Test]
        public void ShouldProcessWhenSeveralPassedTests()
        {
            // Given

            // When
            _events.SendTestResult(CreateTestResult());
            _events.SendTestResult(CreateTestResult(TestOutcome.Passed, "test2"));

            // Then
            _lines.ShouldBe(new[]
            {
                 "+ root"
                ,"+ suite assembly.dll"
                ,"+ test assembly.dll/test1"
                ,"# test assembly.dll/test1 duration 00:00:01"
                ,"- test assembly.dll/test1"
                ,"+ test assembly.dll/test2"
                ,"# test assembly.dll/test2 duration 00:00:01"
                ,"- test assembly.dll/test2"
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

            // Then
            _lines.ShouldBe(new[]
            {
                 "+ root"
                ,"+ suite assembly.dll"
                ,"+ test assembly.dll/test1"
                ,"# test assembly.dll/test1 duration 00:00:01"
                ,"- test assembly.dll/test1"
                ,"+ test assembly.dll/test2"
                ,"# test assembly.dll/test2 duration 00:00:01"
                ,"- test assembly.dll/test2"
                ,"- suite assembly.dll"
                ,"+ suite assembly2.dll"
                ,"+ test assembly2.dll/test3"
                ,"# test assembly2.dll/test3 duration 00:00:01"
                ,"- test assembly2.dll/test3"
                ,"+ test assembly2.dll/test4"
                ,"# test assembly2.dll/test4 duration 00:00:01"
                ,"- test assembly2.dll/test4"
            });
        }

        [Test]
        public void ShouldProcessWhenMessageBeforePassedTest()
        {
            // Given

            // When
            _events.SendTestRunMessage(CreateMessage(TestMessageLevel.Informational, "text 1"));
            _events.SendTestRunMessage(CreateMessage(TestMessageLevel.Warning, "warningInfo"));
            _events.SendTestRunMessage(CreateMessage(TestMessageLevel.Informational, "text 2"));
            _events.SendTestRunMessage(CreateMessage(TestMessageLevel.Error, "errorDetails"));
            _events.SendTestResult(CreateTestResult());

            // Then
            _lines.ShouldBe(new[]
            {
                 "+ root"
                ,"+ suite assembly.dll"
                ,"+ test assembly.dll/test1"
                ,"# test assembly.dll/test1 message text 1"
                ,"# test assembly.dll/test1 warning warningInfo"
                ,"# test assembly.dll/test1 message text 2"
                ,"# test assembly.dll/test1 error errorDetails"
                ,"# test assembly.dll/test1 duration 00:00:01"
                ,"- test assembly.dll/test1"
            });
        }

        [Test]
        public void ShouldProcessWhenMessageForSeveralPassedTests()
        {
            // Given

            // When
            _events.SendTestRunMessage(CreateMessage(TestMessageLevel.Informational, "text 1"));
            _events.SendTestRunMessage(CreateMessage(TestMessageLevel.Warning, "warningInfo"));
            _events.SendTestResult(CreateTestResult());
            _events.SendTestRunMessage(CreateMessage(TestMessageLevel.Informational, "text 2"));
            _events.SendTestRunMessage(CreateMessage(TestMessageLevel.Error, "errorDetails"));
            _events.SendTestResult(CreateTestResult(TestOutcome.Passed, "test2"));

            // Then
            _lines.ShouldBe(new[]
            {
                 "+ root"
                ,"+ suite assembly.dll"
                ,"+ test assembly.dll/test1"
                ,"# test assembly.dll/test1 message text 1"
                ,"# test assembly.dll/test1 warning warningInfo"
                ,"# test assembly.dll/test1 duration 00:00:01"
                ,"- test assembly.dll/test1"
                ,"+ test assembly.dll/test2"
                ,"# test assembly.dll/test2 message text 2"
                ,"# test assembly.dll/test2 error errorDetails"
                ,"# test assembly.dll/test2 duration 00:00:01"
                ,"- test assembly.dll/test2"
            });
        }

        [Test]
        public void ShouldProcessWhenMessageAfterAllTestsSuiteы()
        {
            // Given

            // When
            _events.SendTestRunMessage(CreateMessage(TestMessageLevel.Informational, "text 1"));
            _events.SendTestRunMessage(CreateMessage(TestMessageLevel.Warning, "warningInfo"));
            _events.SendTestResult(CreateTestResult());
            _events.SendTestRunMessage(CreateMessage(TestMessageLevel.Informational, "text 2"));
            _events.SendTestRunMessage(CreateMessage(TestMessageLevel.Error, "errorDetails"));
            _events.SendTestRunComplete(CreateComplete());

            // Then
            _lines.ShouldBe(new[]
            {
                 "+ root"
                ,"+ suite assembly.dll"
                ,"+ test assembly.dll/test1"
                ,"# test assembly.dll/test1 message text 1"
                ,"# test assembly.dll/test1 warning warningInfo"
                ,"# test assembly.dll/test1 duration 00:00:01"
                ,"- test assembly.dll/test1"
                ,"# suite assembly.dll message text 2"
                ,"# suite assembly.dll error errorDetails"
                ,"- suite assembly.dll"
                ,"- root"
            });
        }

        private static TestRunMessageEventArgs CreateMessage(TestMessageLevel level, string message)
        {
            return new TestRunMessageEventArgs(level, message);
        }

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
    }
}
