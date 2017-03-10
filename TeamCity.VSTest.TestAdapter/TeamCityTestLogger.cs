namespace TeamCity.VSTest.TestAdapter
{
    using System;
    using System.Collections.Generic;
    using JetBrains.TeamCity.ServiceMessages.Write.Special;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

    [ExtensionUri(ExtensionId)]
    [FriendlyName(FriendlyName)]
    public class TeamCityTestLogger : ITestLoggerWithParameters
    {
        public const string FriendlyName = "teamcity";
        public const string ExtensionId = "logger://" + FriendlyName;
        [NotNull] private readonly ITeamCityWriter _rootWriter;
        [NotNull] private readonly ITestCaseFilter _testCaseFilter;
        [CanBeNull] private ITeamCityTestsSubWriter _testSuiteWriter;
        [CanBeNull] private string _testSuiteSource;     

        public TeamCityTestLogger()
            :this(ServiceLocator.GetTeamCityWriter(), ServiceLocator.GetTestCaseFilter())
        {
        }

        internal TeamCityTestLogger(
            [NotNull] ITeamCityWriter rootWriter,
            [NotNull] ITestCaseFilter testCaseFilter)
        {
            if (rootWriter == null) throw new ArgumentNullException(nameof(rootWriter));
            if (testCaseFilter == null) throw new ArgumentNullException(nameof(testCaseFilter));
            _rootWriter = rootWriter;
            _testCaseFilter = testCaseFilter;
        }

        public void Initialize([NotNull] TestLoggerEvents events, Dictionary<string, string> parameters)
        {
            if (events == null) throw new ArgumentNullException(nameof(events));
            SubscribeToEvets(events);
        }

        public void Initialize([NotNull] TestLoggerEvents events, string testRunDirectory)
        {
            if (events == null) throw new ArgumentNullException(nameof(events));
            SubscribeToEvets(events);
        }

        private void SubscribeToEvets(TestLoggerEvents events)
        {
            events.TestResult += OnTestResult;
            events.TestRunComplete += OnTestRunComplete;
        }

        private void OnTestResult(object sender, [NotNull] TestResultEventArgs ev)
        {
            if (ev == null) throw new ArgumentNullException(nameof(ev));
            var result = ev.Result;
            var testCase = result.TestCase;
            if (!_testCaseFilter.IsSupported(testCase))
            {
                return;
            }

            var testSuiteWriter = GetTestSuiteWriter(testCase.Source ?? "VSTest");
            using (var testWriter = testSuiteWriter.OpenTest(testCase.FullyQualifiedName ?? "Test"))
            {
                // ReSharper disable once SuspiciousTypeConversion.Global
                testWriter.WriteDuration(result.Duration);
                if (result.Messages != null && result.Messages.Count > 0)
                {
                    var messageWriter = testWriter as ITeamCityMessageWriter;
                    if (messageWriter != null)
                    {
                        foreach (var message in result.Messages)
                        {
                            if (
                                TestResultMessage.StandardOutCategory.Equals(message.Category, StringComparison.CurrentCultureIgnoreCase)
                                || TestResultMessage.AdditionalInfoCategory.Equals(message.Category, StringComparison.CurrentCultureIgnoreCase)
                                || TestResultMessage.DebugTraceCategory.Equals(message.Category, StringComparison.CurrentCultureIgnoreCase))
                            {
                                messageWriter.WriteMessage(message.Text);
                                continue;
                            }

                            if (TestResultMessage.StandardErrorCategory.Equals(message.Category, StringComparison.CurrentCultureIgnoreCase))
                            {
                                messageWriter.WriteError(message.Text);
                            }
                        }
                    }
                }

                switch (result.Outcome)
                {
                    case TestOutcome.Passed:
                        break;

                    case TestOutcome.Failed:
                        testWriter.WriteFailed(result.ErrorMessage ?? string.Empty, result.ErrorStackTrace ?? string.Empty);
                        break;

                    case TestOutcome.Skipped:
                        if (string.IsNullOrEmpty(result.ErrorMessage))
                        {
                            testWriter.WriteIgnored();
                        }
                        else
                        {
                            testWriter.WriteIgnored(result.ErrorMessage);
                        }
                        
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
            _testSuiteWriter?.Dispose();
            _rootWriter.Dispose();
        }

        [NotNull]
        private ITeamCityTestsSubWriter GetTestSuiteWriter([NotNull] string source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (_testSuiteWriter != null && _testSuiteSource == source)
            {
                return _testSuiteWriter;
            }

            _testSuiteWriter?.Dispose();
            _testSuiteSource = source;
            var testSuiteWriter = _rootWriter.OpenTestSuite(source);
            _testSuiteWriter = testSuiteWriter;
            return testSuiteWriter;
        }
    }
}
