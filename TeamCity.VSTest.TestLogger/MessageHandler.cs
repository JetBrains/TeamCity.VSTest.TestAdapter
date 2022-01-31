namespace TeamCity.VSTest.TestLogger
{
    using System;
    using JetBrains.TeamCity.ServiceMessages.Write.Special;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

    internal class MessageHandler : IMessageHandler
    {
        [NotNull] private readonly ITeamCityWriter _rootWriter;
        [NotNull] private readonly ISuiteNameProvider _suiteNameProvider;
        [NotNull] private readonly IOptions _options;
        private readonly IAttachments _attachments;
        private readonly ITestNameProvider _testNameProvider;
        private readonly IEventRegistry _eventRegistry;
        [NotNull] private readonly ITestCaseFilter _testCaseFilter;
        [CanBeNull] private string _testSuiteSource;
        [CanBeNull] private ITeamCityWriter _flowWriter;
        [CanBeNull] private ITeamCityTestsSubWriter _testSuiteWriter;

        internal MessageHandler(
            [NotNull] ITeamCityWriter rootWriter,
            [NotNull] ITestCaseFilter testCaseFilter,
            [NotNull] ISuiteNameProvider suiteNameProvider,
            [NotNull] IOptions options,
            [NotNull] IAttachments attachments,
            [NotNull] ITestNameProvider testNameProvider,
            [NotNull] IEventRegistry eventRegistry)
        {
            _rootWriter = rootWriter ?? throw new ArgumentNullException(nameof(rootWriter));
            _testCaseFilter = testCaseFilter ?? throw new ArgumentNullException(nameof(testCaseFilter));
            _suiteNameProvider = suiteNameProvider ?? throw new ArgumentNullException(nameof(suiteNameProvider));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _attachments = attachments ?? throw new ArgumentNullException(nameof(attachments));
            _testNameProvider = testNameProvider ?? throw new ArgumentNullException(nameof(testNameProvider));
            _eventRegistry = eventRegistry ?? throw new ArgumentNullException(nameof(eventRegistry));
        }

        public void OnTestRunMessage(TestRunMessageEventArgs ev)
        {
            if (ev == null) throw new ArgumentNullException(nameof(ev));
            if (ev.Level == TestMessageLevel.Informational && !Strings.IsNullOrWhiteSpace(ev.Message))
            {
                _testCaseFilter.RegisterOutputMessage(ev.Message);
            }
        }

        public void OnTestResult(TestResultEventArgs ev)
        {
            if (ev == null) throw new ArgumentNullException(nameof(ev));

            var result = ev.Result;
            var testCase = result.TestCase;
            if (!_testCaseFilter.IsSupported(testCase))
            {
                return;
            }

            var suiteName = _suiteNameProvider.GetSuiteName(_options.TestRunDirectory, testCase.Source);
            var testSuiteWriter = GetTestSuiteWriter(suiteName);
            var testName = _testNameProvider.GetTestName(testCase.FullyQualifiedName, testCase.DisplayName);
            if (string.IsNullOrEmpty(testName))
            {
                testName = testCase.Id.ToString();
            }
            
            using (_eventRegistry.Register(new TestEvent(suiteName, testCase)))
            using (var testWriter = testSuiteWriter.OpenTest(testName))
            {
                // ReSharper disable once SuspiciousTypeConversion.Global
                testWriter.WriteDuration(result.Duration);
                if (result.Messages != null && result.Messages.Count > 0)
                {
                    foreach (var message in result.Messages)
                    {
                        if (TestResultMessage.StandardOutCategory.Equals(message.Category, StringComparison.CurrentCultureIgnoreCase)
                            || TestResultMessage.AdditionalInfoCategory.Equals(message.Category, StringComparison.CurrentCultureIgnoreCase)
                            || TestResultMessage.DebugTraceCategory.Equals(message.Category, StringComparison.CurrentCultureIgnoreCase))
                        {
                            testWriter.WriteStdOutput(message.Text);
                            continue;
                        }

                        if (TestResultMessage.StandardErrorCategory.Equals(message.Category, StringComparison.CurrentCultureIgnoreCase))
                        {
                            testWriter.WriteErrOutput(message.Text);
                        }
                    }
                }

                foreach (var attachments in result.Attachments)
                {
                    foreach (var attachment in attachments.Attachments)
                    {
                        _attachments.SendAttachment(testName, attachment, testWriter);
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
                    case TestOutcome.None: // https://github.com/JetBrains/TeamCity.VSTest.TestAdapter/issues/23
                    case TestOutcome.NotFound:
                        if (string.IsNullOrEmpty(result.ErrorMessage))
                            testWriter.WriteIgnored();
                        else
                            testWriter.WriteIgnored(result.ErrorMessage);

                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(result.Outcome), result.Outcome, "Invalid value");
                }
            }
        }

        public void OnTestRunComplete()
        {
            _testSuiteWriter?.Dispose();
            _flowWriter?.Dispose();
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

            _flowWriter ??= _rootWriter.OpenFlow();
            var testSuiteWriter = _flowWriter.OpenTestSuite(source);
            _testSuiteWriter = testSuiteWriter;
            return testSuiteWriter;
        }
    }
}
