namespace TeamCity.VSTestAdapter.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using JetBrains.TeamCity.ServiceMessages.Write.Special;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
    using Moq;

    using NUnit.Framework;

    using Shouldly;

    [TestFixture]
    [SuppressMessage("ReSharper", "ImplicitlyCapturedClosure")]
    public class TeamCityTestLoggerTests
    {
        private Mock<ITeamCityWriter> _rootWriter;
        private Events _events;

        [SetUp]
        public void SetUp()
        {
            _events = new Events();
            _rootWriter = new Mock<ITeamCityWriter>();
        }

        [Test]
        public void ShouldProcessWhenPassedTest()
        {
            // Given
            CreateInstance();
            var suiteWriter = new Mock<ITeamCityTestsSubWriter>();
            var testWriter = new Mock<ITeamCityTestWriter>();
            int? order = 0;
            _rootWriter.Setup(i => i.OpenTestSuite("assembly.dll")).Returns(() => { order++.ShouldBe(0); return suiteWriter.Object; });
            suiteWriter.Setup(i => i.OpenTest("test1")).Returns(() => { order++.ShouldBe(1); return testWriter.Object; });
            testWriter.Setup(i => i.WriteDuration(TimeSpan.FromSeconds(1))).Callback(() => { order++.ShouldBe(2); });
            testWriter.Setup(i => i.Dispose()).Callback(() => { order++.ShouldBe(3); });
            suiteWriter.Setup(i => i.Dispose()).Callback(() => { order++.ShouldBe(4); });

            // When
            _events.SendTestResult(CreateTestResult());

            // Then
            testWriter.Verify(i => i.WriteDuration(TimeSpan.FromSeconds(1)));
            testWriter.Verify(i => i.Dispose());
            suiteWriter.Verify(i => i.Dispose(), Times.Never);
        }

        [Test]
        public void ShouldProcessWhenSeveralPassedTest()
        {
            // Given
            CreateInstance();
            var suiteWriter = new Mock<ITeamCityTestsSubWriter>();
            var testWriter1 = new Mock<ITeamCityTestWriter>();
            var testWriter2 = new Mock<ITeamCityTestWriter>();
            int? order = 0;
            _rootWriter.Setup(i => i.OpenTestSuite("assembly.dll")).Returns(() => { order++.ShouldBe(0); return suiteWriter.Object; });

            suiteWriter.Setup(i => i.OpenTest("test1")).Returns(() => { order++.ShouldBe(1); return testWriter1.Object; });
            testWriter1.Setup(i => i.Dispose()).Callback(() => { order++.ShouldBe(2); });

            suiteWriter.Setup(i => i.OpenTest("test2")).Returns(() => { order++.ShouldBe(3); return testWriter2.Object; });
            testWriter2.Setup(i => i.Dispose()).Callback(() => { order++.ShouldBe(4); });

            suiteWriter.Setup(i => i.Dispose()).Callback(() => { order++.ShouldBe(5); });

            // When
            _events.SendTestResult(CreateTestResult(TestOutcome.Passed, "test1"));
            _events.SendTestResult(CreateTestResult(TestOutcome.Passed, "test2"));

            // Then
            testWriter1.Verify(i => i.Dispose());
            testWriter2.Verify(i => i.Dispose());
            suiteWriter.Verify(i => i.Dispose(), Times.Never);
        }

        [Test]
        public void ShouldProcessWhenMessageDuringPassedTest()
        {
            // Given
            CreateInstance();
            var suiteWriter = new Mock<ITeamCityTestsSubWriter>();
            var testWriter = new Mock<ITeamCityTestWriter>();
            var messageWriter = testWriter.As<ITeamCityMessageWriter>();
            var order = 0;
            _rootWriter.Setup(i => i.OpenTestSuite("assembly.dll")).Returns(() => suiteWriter.Object);
            suiteWriter.Setup(i => i.OpenTest("test1")).Returns(() => { order++.ShouldBe(0); return testWriter.Object; });
            messageWriter.Setup(i => i.WriteMessage("text 1")).Callback(() => { order++.ShouldBe(1); });
            messageWriter.Setup(i => i.WriteWarning("warning")).Callback(() => { order++.ShouldBe(2); });
            messageWriter.Setup(i => i.WriteMessage("text 2")).Callback(() => { order++.ShouldBe(3); });
            messageWriter.Setup(i => i.WriteError("error", null)).Callback(() => { order++.ShouldBe(4); });
            testWriter.Setup(i => i.Dispose()).Callback(() => { order++.ShouldBe(5); });

            // When
            _events.SendTestRunMessage(CreateMessage(TestMessageLevel.Informational, "text 1"));
            _events.SendTestRunMessage(CreateMessage(TestMessageLevel.Warning, "warning"));
            _events.SendTestRunMessage(CreateMessage(TestMessageLevel.Informational, "text 2"));
            _events.SendTestRunMessage(CreateMessage(TestMessageLevel.Error, "error"));
            _events.SendTestResult(CreateTestResult());

            // Then
            messageWriter.Verify(i => i.WriteMessage("text 1"));
            messageWriter.Verify(i => i.WriteWarning("warning"));
            messageWriter.Verify(i => i.WriteMessage("text 2"));
            messageWriter.Verify(i => i.WriteError("error", null));
        }

        [Test]
        public void ShouldProcessWhenMessageForSeveralTests()
        {
            // Given
            CreateInstance();
            var suiteWriter = new Mock<ITeamCityTestsSubWriter>();
            var testWriter1 = new Mock<ITeamCityTestWriter>();
            var messageWriter1 = testWriter1.As<ITeamCityMessageWriter>();
            var testWriter2 = new Mock<ITeamCityTestWriter>();
            var messageWriter2 = testWriter2.As<ITeamCityMessageWriter>();
            var order = 0;
            _rootWriter.Setup(i => i.OpenTestSuite("assembly.dll")).Returns(() => suiteWriter.Object);

            suiteWriter.Setup(i => i.OpenTest("test1")).Returns(() => testWriter1.Object);
            messageWriter1.Setup(i => i.WriteMessage("text 1")).Callback(() => { order++.ShouldBe(0); });
            messageWriter1.Setup(i => i.WriteWarning("warning")).Callback(() => { order++.ShouldBe(1); });
            testWriter1.Setup(i => i.Dispose()).Callback(() => { order++.ShouldBe(2); });

            suiteWriter.Setup(i => i.OpenTest("test2")).Returns(() => testWriter2.Object);
            messageWriter2.Setup(i => i.WriteMessage("text 2")).Callback(() => { order++.ShouldBe(3); });
            messageWriter2.Setup(i => i.WriteError("error", null)).Callback(() => { order++.ShouldBe(4); });
            testWriter2.Setup(i => i.Dispose()).Callback(() => { order++.ShouldBe(5); });

            // When
            _events.SendTestRunMessage(CreateMessage(TestMessageLevel.Informational, "text 1"));
            _events.SendTestRunMessage(CreateMessage(TestMessageLevel.Warning, "warning"));
            _events.SendTestResult(CreateTestResult(TestOutcome.Passed, "test1"));

            _events.SendTestRunMessage(CreateMessage(TestMessageLevel.Informational, "text 2"));
            _events.SendTestRunMessage(CreateMessage(TestMessageLevel.Error, "error"));
            _events.SendTestResult(CreateTestResult(TestOutcome.Passed, "test2"));

            // Then
            messageWriter1.Verify(i => i.WriteMessage("text 1"));
            messageWriter1.Verify(i => i.WriteWarning("warning"));
            messageWriter2.Verify(i => i.WriteMessage("text 2"));
            messageWriter2.Verify(i => i.WriteError("error", null));
        }

        [Test]
        public void ShouldProcessWhenMessageAfterTestInSuite()
        {
            // Given
            CreateInstance();
            var suiteWriter = new Mock<ITeamCityTestsSubWriter>();
            var messageSuiteWriter = suiteWriter.As<ITeamCityMessageWriter>();
            var testWriter1 = new Mock<ITeamCityTestWriter>();
            var testWriter2 = new Mock<ITeamCityTestWriter>();
            var messageWriter1 = testWriter1.As<ITeamCityMessageWriter>();
            var suiteWriter2 = new Mock<ITeamCityTestsSubWriter>();

            var order = 0;
            _rootWriter.Setup(i => i.OpenTestSuite("assembly.dll")).Returns(() => suiteWriter.Object);
            suiteWriter.Setup(i => i.OpenTest("test1")).Returns(() => testWriter1.Object);
            _rootWriter.Setup(i => i.OpenTestSuite("assembly2.dll")).Returns(() => suiteWriter2.Object);
            suiteWriter2.Setup(i => i.OpenTest("test2")).Returns(() => testWriter2.Object);

            messageWriter1.Setup(i => i.WriteMessage("text 1")).Callback(() => { order++.ShouldBe(0); });
            messageWriter1.Setup(i => i.WriteWarning("warning")).Callback(() => { order++.ShouldBe(1); });
            testWriter1.Setup(i => i.Dispose()).Callback(() => { order++.ShouldBe(2); });

            messageSuiteWriter.Setup(i => i.WriteMessage("text 2")).Callback(() => { order++.ShouldBe(3); });
            messageSuiteWriter.Setup(i => i.WriteError("error", null)).Callback(() => { order++.ShouldBe(4); });
            suiteWriter.Setup(i => i.Dispose()).Callback(() => { order++.ShouldBe(5); });

            // When
            _events.SendTestRunMessage(CreateMessage(TestMessageLevel.Informational, "text 1"));
            _events.SendTestRunMessage(CreateMessage(TestMessageLevel.Warning, "warning"));
            _events.SendTestResult(CreateTestResult(TestOutcome.Passed, "test1"));

            _events.SendTestRunMessage(CreateMessage(TestMessageLevel.Informational, "text 2"));
            _events.SendTestRunMessage(CreateMessage(TestMessageLevel.Error, "error"));
            _events.SendTestResult(CreateTestResult(TestOutcome.Passed, "test2", "assembly2.dll"));

            // Then
            messageWriter1.Verify(i => i.WriteMessage("text 1"));
            messageWriter1.Verify(i => i.WriteWarning("warning"));
            messageSuiteWriter.Verify(i => i.WriteMessage("text 2"));
            messageSuiteWriter.Verify(i => i.WriteError("error", null));
        }

        private void CreateInstance()
        {
            var logger = new TeamCityTestLogger(_rootWriter.Object);
            logger.Initialize(_events, null);
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
    }
}
