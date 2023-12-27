namespace TeamCity.VSTest.TestLogger
{
    using System;
    using JetBrains.TeamCity.ServiceMessages.Write.Special;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

    internal class MessageHandler : IMessageHandler
    {
        private readonly ITeamCityWriter _rootWriter;
        private readonly ISuiteNameProvider _suiteNameProvider;
        private readonly IAttachments _attachments;
        private readonly ITestNameProvider _testNameProvider;
        private readonly IEventRegistry _eventRegistry;
        private ITeamCityWriter? _flowWriter;
        private ITeamCityWriter? _blockWriter;

        private ITeamCityWriter CurrentWriter => _blockWriter ?? _flowWriter ?? _rootWriter;

        internal MessageHandler(
            ITeamCityWriter rootWriter,
            ISuiteNameProvider suiteNameProvider,
            IAttachments attachments,
            ITestNameProvider testNameProvider,
            IEventRegistry eventRegistry)
        {
            _rootWriter = rootWriter ?? throw new ArgumentNullException(nameof(rootWriter));
            _suiteNameProvider = suiteNameProvider ?? throw new ArgumentNullException(nameof(suiteNameProvider));
            _attachments = attachments ?? throw new ArgumentNullException(nameof(attachments));
            _testNameProvider = testNameProvider ?? throw new ArgumentNullException(nameof(testNameProvider));
            _eventRegistry = eventRegistry ?? throw new ArgumentNullException(nameof(eventRegistry));
        }

        public void OnTestRunMessage(TestRunMessageEventArgs? ev)
        { }

        public void OnTestResult(TestResultEventArgs? ev)
        {
            if (ev == null)
            {
                return;
            }

            var result = ev.Result;
            var testCase = result.TestCase;
            var suiteName = _suiteNameProvider.GetSuiteName(testCase.Source);
            var testName = _testNameProvider.GetTestName(testCase.FullyQualifiedName, testCase.DisplayName);
            if (string.IsNullOrEmpty(testName))
            {
                testName = testCase.Id.ToString();
            }
            
            if (!string.IsNullOrEmpty(suiteName))
            {
                testName = suiteName + ": " + testName;
            }

            using (_eventRegistry.Register(new TestEvent(suiteName, ev.Result.DisplayName, testCase)))
            using (var testWriter = CurrentWriter.OpenTest(testName))
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

        public void OnTestRunStart(string testRunDescription, bool shouldOpenNewFlow)
        {
            if (shouldOpenNewFlow)
            {
                _flowWriter = _rootWriter.OpenFlow();
            }

            _blockWriter = (_flowWriter ?? _rootWriter).OpenBlock(testRunDescription);
        }

        public void OnTestRunComplete()
        {
            _blockWriter?.Dispose();
            _flowWriter?.Dispose();
            _rootWriter.Dispose();
        }
    }
}
