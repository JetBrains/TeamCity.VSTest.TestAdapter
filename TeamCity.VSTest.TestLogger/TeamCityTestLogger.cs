namespace TeamCity.VSTest.TestLogger
{
    using System;
    using JetBrains.TeamCity.ServiceMessages.Write.Special;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

    [ExtensionUri(ExtensionId)]
    [FriendlyName(FriendlyName)]
    public class TeamCityTestLogger : ITestLogger
    {
        internal const string ExtensionId = "logger://" + FriendlyName;
        private const string FriendlyName = "teamcity";

        private readonly ITeamCityWriter _rootWriter;
        private readonly ISuiteNameProvider _suiteNameProvider;
        private readonly ITestCaseFilter _testCaseFilter;
        [CanBeNull] private string _testRunDirectory;
        [CanBeNull] private string _testSuiteSource;
        [CanBeNull] private ITeamCityTestsSubWriter _testSuiteWriter;
        private readonly bool _initialized;

        // ReSharper disable once UnusedMember.Global
        public TeamCityTestLogger()
        {
            _initialized = ServiceLocator.Initialize();
            if (_initialized)
            {
                _rootWriter = ServiceLocator.GetTeamCityWriter();
                _testCaseFilter = ServiceLocator.GetTestCaseFilter();
                _suiteNameProvider = ServiceLocator.GetSuiteNameProvider();
                if (_rootWriter == null) throw new InvalidOperationException();
                if (_testCaseFilter == null) throw new InvalidOperationException();
                if (_suiteNameProvider == null) throw new InvalidOperationException();
            }
        }

        internal TeamCityTestLogger(
            [NotNull] ITeamCityWriter rootWriter,
            [NotNull] ITestCaseFilter testCaseFilter,
            [NotNull] ISuiteNameProvider suiteNameProvider)
        {
            _rootWriter = rootWriter ?? throw new ArgumentNullException(nameof(rootWriter));
            _testCaseFilter = testCaseFilter ?? throw new ArgumentNullException(nameof(testCaseFilter));
            _suiteNameProvider = suiteNameProvider ?? throw new ArgumentNullException(nameof(suiteNameProvider));
            _initialized = true;
        }

        public void Initialize([NotNull] TestLoggerEvents events, [CanBeNull] string testRunDirectory)
        {
            if (events == null) throw new ArgumentNullException(nameof(events));
            _testRunDirectory = testRunDirectory;
            if (!_initialized)
            {
                return;
            }

            SubscribeToEvets(events);
        }

        private void SubscribeToEvets(TestLoggerEvents events)
        {
            events.TestRunMessage += OnTestRunMessage;
            events.TestResult += OnTestResult;
            events.TestRunComplete += OnTestRunComplete;
        }

        private void OnTestRunMessage(object sender, TestRunMessageEventArgs ev)
        {
            if (ev == null) throw new ArgumentNullException(nameof(ev));
            if (ev.Level == TestMessageLevel.Informational && !Strings.IsNullOrWhiteSpace(ev.Message))
            {
                _testCaseFilter.RegisterOutputMessage(ev.Message);
            }
        }

        private void OnTestResult(object sender, [NotNull] TestResultEventArgs ev)
        {
            if (ev == null) throw new ArgumentNullException(nameof(ev));
            var result = ev.Result;
            var testCase = result.TestCase;
            if (!_testCaseFilter.IsSupported(testCase))
                return;

            var suiteName = _suiteNameProvider.GetSuiteName(_testRunDirectory, testCase.Source);
            var testSuiteWriter = GetTestSuiteWriter(suiteName);
            using (var testWriter = testSuiteWriter.OpenTest(testCase.FullyQualifiedName ?? testCase.DisplayName ?? testCase.Id.ToString()))
            {
                // ReSharper disable once SuspiciousTypeConversion.Global
                testWriter.WriteDuration(result.Duration);
                if (result.Messages != null && result.Messages.Count > 0)
                {
                    if (testWriter is ITeamCityMessageWriter messageWriter)
                    {
                        foreach (var message in result.Messages)
                        {
                            if (TestResultMessage.StandardOutCategory.Equals(message.Category, StringComparison.CurrentCultureIgnoreCase)
                                || TestResultMessage.AdditionalInfoCategory.Equals(message.Category, StringComparison.CurrentCultureIgnoreCase)
                                || TestResultMessage.DebugTraceCategory.Equals(message.Category, StringComparison.CurrentCultureIgnoreCase))
                            {
                                messageWriter.WriteMessage(message.Text);
                                continue;
                            }

                            if (TestResultMessage.StandardErrorCategory.Equals(message.Category, StringComparison.CurrentCultureIgnoreCase))
                                messageWriter.WriteError(message.Text);
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
                            testWriter.WriteIgnored();
                        else
                            testWriter.WriteIgnored(result.ErrorMessage);

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
            _suiteNameProvider.Reset();
        }

        [NotNull]
        private ITeamCityTestsSubWriter GetTestSuiteWriter([NotNull] string source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (_testSuiteWriter != null && _testSuiteSource == source)
                return _testSuiteWriter;

            _testSuiteWriter?.Dispose();
            _testSuiteSource = source;
            var testSuiteWriter = _rootWriter.OpenTestSuite(source);
            _testSuiteWriter = testSuiteWriter;
            return testSuiteWriter;
        }
    }
}