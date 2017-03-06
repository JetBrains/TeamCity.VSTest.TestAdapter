namespace TeamCity.VSTestAdapter.Tests
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
            _logger.Initialize(_events, null);
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
                ,"+ test test1"
                ,"# test test1 duration 00:00:01"
                ,"- test test1"
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
                ,"+ test test1"
                ,"# test test1 duration 00:00:01"
                ,"- test test1"
                ,"+ test test2"
                ,"# test test2 duration 00:00:01"
                ,"- test test2"
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
                ,"+ test test1"
                ,"# test test1 message text 1"
                ,"# test test1 warning warningInfo"
                ,"# test test1 message text 2"
                ,"# test test1 error errorDetails"
                ,"# test test1 duration 00:00:01"
                ,"- test test1"
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
                ,"+ test test1"
                ,"# test test1 message text 1"
                ,"# test test1 warning warningInfo"
                ,"# test test1 duration 00:00:01"
                ,"- test test1"
                ,"+ test test2"
                ,"# test test2 message text 2"
                ,"# test test2 error errorDetails"
                ,"# test test2 duration 00:00:01"
                ,"- test test2"
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
                ,"+ test test1"
                ,"# test test1 message text 1"
                ,"# test test1 warning warningInfo"
                ,"# test test1 duration 00:00:01"
                ,"- test test1"
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
                    Duration = TimeSpan.FromSeconds(1)
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
