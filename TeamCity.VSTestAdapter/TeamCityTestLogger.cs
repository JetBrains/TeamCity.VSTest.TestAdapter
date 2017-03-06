namespace TeamCity.VSTestAdapter
{
    using System;
    using System.Collections.Generic;
    using JetBrains.TeamCity.ServiceMessages.Write.Special;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

    [ExtensionUri(ExtensionId)]
    [FriendlyName("TeamCity")]
    public class TeamCityTestLogger : ITestLogger
    {
        public const string ExtensionId = "logger://TeamCityLogger";
        [NotNull] private readonly List<TestRunMessageEventArgs> _messages = new List<TestRunMessageEventArgs>();
        [NotNull] private readonly ITeamCityWriter _rootWriter;
        private readonly Uri _extensionUri;
        [CanBeNull] private ITeamCityTestsSubWriter _testSuiteWriter;
        [CanBeNull] private string _testSuiteSource;

        public TeamCityTestLogger()
            :this(Factory.CreateTeamCityWriter())
        {
        }

        internal TeamCityTestLogger(
            [NotNull] ITeamCityWriter rootWriter)
        {
            if (rootWriter == null) throw new ArgumentNullException(nameof(rootWriter));
            _extensionUri = new Uri(ExtensionId);
            _rootWriter = rootWriter;
        }

        public void Initialize(TestLoggerEvents events, string testRunDirectory)
        {
            if (events == null) throw new ArgumentNullException(nameof(events));
            events.TestRunMessage += OnTestRunMessage;
            events.TestResult += OnTestResult;
            events.TestRunComplete += OnTestRunComplete;
        }

        private void OnTestRunMessage(object sender, TestRunMessageEventArgs ev)
        {
            _messages.Add(ev);
        }

        private void OnTestResult(object sender, TestResultEventArgs ev)
        {
            if (ev.Result.TestCase.ExecutorUri != _extensionUri)
            {
                return;
            }

            var result = ev.Result;
            var testCase = result.TestCase;
            var testSuiteWriter = GetTestSuiteWriter(testCase.Source);
            using (var testWriter = testSuiteWriter.OpenTest(testCase.FullyQualifiedName))
            {
                // ReSharper disable once SuspiciousTypeConversion.Global
                var messageWriter = testWriter as ITeamCityMessageWriter;
                if (messageWriter != null)
                {
                    SendMessages(messageWriter);
                }

                switch (result.Outcome)
                {
                    case TestOutcome.Passed:
                        testWriter.WriteDuration(result.Duration);
                        break;

                    case TestOutcome.Failed:
                        break;

                    case TestOutcome.Skipped:
                        break;

                    case TestOutcome.NotFound:
                        break;

                    case TestOutcome.None:
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(result.Outcome), result.Outcome, "Invalid value");
                }
            }
        }

        private void OnTestRunComplete(object sender, TestRunCompleteEventArgs ev)
        {
            ReleaseTestSuite();
            _rootWriter.Dispose();
        }

        private void SendMessages(ITeamCityMessageWriter messageWriter)
        {
            foreach (var messageItem in _messages)
            {
                switch (messageItem.Level)
                {
                    case TestMessageLevel.Informational:
                        messageWriter.WriteMessage(messageItem.Message);
                        break;

                    case TestMessageLevel.Warning:
                        messageWriter.WriteWarning(messageItem.Message);
                        break;

                    case TestMessageLevel.Error:
                        messageWriter.WriteError(messageItem.Message);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            _messages.Clear();
        }

        [NotNull]
        private ITeamCityTestsSubWriter GetTestSuiteWriter([NotNull] string source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (_testSuiteWriter != null && _testSuiteSource == source)
            {
                return _testSuiteWriter;
            }

            ReleaseTestSuite();
            _testSuiteSource = source;
            var testSuiteWriter = _rootWriter.OpenTestSuite(source);
            _testSuiteWriter = testSuiteWriter;
            return testSuiteWriter;
        }

        private void ReleaseTestSuite()
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            var messageWriter = _testSuiteWriter as ITeamCityMessageWriter;
            if (messageWriter != null)
            {
                SendMessages(messageWriter);
            }

            _testSuiteWriter?.Dispose();
        }
    }
}
