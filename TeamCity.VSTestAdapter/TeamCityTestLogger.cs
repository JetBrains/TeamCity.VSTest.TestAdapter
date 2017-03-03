namespace TeamCity.VSTestAdapter
{
    using System;
    using JetBrains.TeamCity.ServiceMessages.Write.Special;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

    [ExtensionUri("logger://TeamCityLogger")]
    [FriendlyName("TeamCity")]
    public class TeamCityTestLogger : ITestLogger
    {
        [NotNull] private readonly ITeamCityWriter _rootWriter;
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
        }

        private void OnTestResult(object sender, TestResultEventArgs ev)
        {
            var result = ev.Result;
            var testCase = result.TestCase;
            var testSuiteWriter = GetTestSuiteWriter(testCase.Source);
            using (var testWriter = testSuiteWriter.OpenTest(testCase.FullyQualifiedName))
            {
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
            _testSuiteWriter?.Dispose();
            _rootWriter.Dispose();
        }

        private ITeamCityTestsSubWriter GetTestSuiteWriter(string source)
        {
            if (_testSuiteSource != null && _testSuiteSource == source)
            {
                return _testSuiteWriter;
            }

            _testSuiteWriter?.Dispose();
            _testSuiteSource = source;
            _testSuiteWriter = _rootWriter.OpenTestSuite(source);
            return _testSuiteWriter;
        }
    }
}
