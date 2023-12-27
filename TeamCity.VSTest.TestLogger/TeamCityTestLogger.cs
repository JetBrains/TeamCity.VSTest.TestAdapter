// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable RedundantUsingDirective
namespace TeamCity.VSTest.TestLogger;

using System;
using System.Reflection;
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
        private static readonly ServiceLocatorNet35 ServiceLocatorNet35 = new();
        private readonly IMessageHandler _handler = ServiceLocatorNet35.MessageHandler;
        private readonly IOptions _options = ServiceLocatorNet35;
        private readonly IMessageWriter _messageWriter = ServiceLocatorNet35.MessageWriter;
#else
    private static readonly IMutableContainer Container = IoC.Container.Create().Using<IoCConfiguration>();
    private readonly IMessageHandler _handler = Container.Resolve<IMessageHandler>();
    private readonly IOptions _options = Container.Resolve<IOptions>();
    private readonly IMessageWriter _messageWriter = Container.Resolve<IMessageWriter>();
#endif

    public void Initialize(TestLoggerEvents events, string testRunDirectory)
    {
        if (events == null) throw new ArgumentNullException(nameof(events));

        OutputLoggerInitializedMessage();

        _options.TestRunDirectory = testRunDirectory;

        var shouldOpenNewFlowForServiceMessages = _options.FallbackToStdOutTestReporting;

        events.TestRunMessage += (_, args) => _handler.OnTestRunMessage(args);

        events.TestResult += (_, args) => _handler.OnTestResult(args);

        events.TestRunComplete += (_, _) =>
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

    private void OutputLoggerInitializedMessage()
    {
        // ReSharper disable once JoinDeclarationAndInitializer
        Version currentAssemblyVersion;

#if (NET35 || NET40)
        currentAssemblyVersion = GetType().Assembly.GetName().Version;
#else
        currentAssemblyVersion = GetType().GetTypeInfo().Assembly.GetName().Version;
#endif

        Console.WriteLine($"TeamCity test logger version {currentAssemblyVersion} is initialized");
    }
}