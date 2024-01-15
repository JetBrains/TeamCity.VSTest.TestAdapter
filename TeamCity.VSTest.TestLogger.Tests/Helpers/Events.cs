﻿#pragma warning disable CS0067
namespace TeamCity.VSTest.TestLogger.Tests.Helpers
{
    using System;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

    internal class Events : TestLoggerEvents
    {
        public override event EventHandler<TestRunMessageEventArgs>? TestRunMessage;
        public override event EventHandler<TestResultEventArgs>? TestResult;
        public override event EventHandler<TestRunCompleteEventArgs>? TestRunComplete;
        public override event EventHandler<TestRunStartEventArgs>? TestRunStart;
        public override event EventHandler<DiscoveryStartEventArgs>? DiscoveryStart;
        public override event EventHandler<TestRunMessageEventArgs>? DiscoveryMessage;
        public override event EventHandler<DiscoveredTestsEventArgs>? DiscoveredTests;
        public override event EventHandler<DiscoveryCompleteEventArgs>? DiscoveryComplete;

        public void SendTestRunMessage(TestRunMessageEventArgs ev)
        {
            if (ev == null) throw new ArgumentNullException(nameof(ev));
            TestRunMessage?.Invoke(this, ev);
        }

        public void SendTestRunComplete(TestRunCompleteEventArgs ev)
        {
            if (ev == null) throw new ArgumentNullException(nameof(ev));
            TestRunComplete?.Invoke(this, ev);
        }

        public void SendTestResult(TestResultEventArgs ev)
        {
            if (ev == null) throw new ArgumentNullException(nameof(ev));
            TestResult?.Invoke(this, ev);
        }
    }
}