// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.VSTest.TestLogger
{
    using System;
#if !NET35
    using IoC;
#endif
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
    
    [ExtensionUri(ExtensionId)]
    [FriendlyName(FriendlyName)]
    public class TeamCityTestLogger : ITestLogger
    {
        internal const string ExtensionId = "logger://" + FriendlyName;
        private const string FriendlyName = "TeamCity";

#if NET35
        private static readonly ServiceLocatorNet35 ServiceLocatorNet35 = new ServiceLocatorNet35();
        [NotNull] private readonly IMessageHandler _handler = ServiceLocatorNet35.MessageHandler;
        [NotNull] private readonly IOptions _options = ServiceLocatorNet35;
        [NotNull] private readonly IMessageWriter _messageWriter = ServiceLocatorNet35.MessageWriter;
#else
        private static readonly IMutableContainer Container = IoC.Container.Create().Using<IoCConfiguration>();
        [NotNull] private readonly IMessageHandler _handler = Container.Resolve<IMessageHandler>();
        [NotNull] private readonly IOptions _options = Container.Resolve<IOptions>();
        [NotNull] private readonly IMessageWriter _messageWriter = Container.Resolve<IMessageWriter>();
#endif

        public void Initialize([NotNull] TestLoggerEvents events, [CanBeNull] string testRunDirectory)
        {
            if (events == null) throw new ArgumentNullException(nameof(events));

            _options.TestRunDirectory = testRunDirectory;

            var shouldOpenNewFlowForServiceMessages = _options.FallbackToStdOutTestReporting;

            events.TestRunMessage += (sender, args) => _handler.OnTestRunMessage(args);

            events.TestResult += (sender, args) => _handler.OnTestResult(args);

            events.TestRunComplete += (sender, args) =>
            {
                _handler.OnTestRunComplete();
                _messageWriter.Flush();
            };

            #if (NET35 || NET40)
            // .Net Framework version of TestLoggerEvents does not have a TestRunStart event handler,
            // so test run directory is used as an acceptable substitute for test run sources
            _handler.OnTestRunStart(
                $"Unit tests from for test run directory {testRunDirectory}",
                shouldOpenNewFlowForServiceMessages);
            #else
            events.TestRunStart += (sender, args) => _handler.OnTestRunStart(
                "Unit tests from " + string.Join(" ", args.TestRunCriteria.Sources),
                shouldOpenNewFlowForServiceMessages);
            #endif
        }
    }
}
