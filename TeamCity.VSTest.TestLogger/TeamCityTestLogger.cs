﻿// ReSharper disable ClassNeverInstantiated.Global
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
        [NotNull] private readonly IMessageHandler _handler = ServiceLocatorNet35.CreateMessageHandler();
        [NotNull] private readonly IOptions _options = ServiceLocatorNet35;
#else
        private static readonly IMutableContainer Container = IoC.Container.Create().Using<IoCConfiguration>();
        [NotNull] private readonly IMessageHandler _handler = Container.Resolve<IMessageHandler>();
        [NotNull] private readonly IOptions _options = Container.Resolve<IOptions>();
#endif

        public void Initialize([NotNull] TestLoggerEvents events, [CanBeNull] string testRunDirectory)
        {
            if (events == null) throw new ArgumentNullException(nameof(events));
            _options.TestRunDirectory = testRunDirectory;
            events.TestRunMessage += (sender, args) => _handler.OnTestRunMessage(args);
            events.TestResult += (sender, args) => _handler.OnTestResult(args);
            events.TestRunComplete += (sender, args) => _handler.OnTestRunComplete();
        }
    }
}