namespace TeamCity.VSTest.TestAdapter.Tests
{
    using System;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

    internal class Events: TestLoggerEvents
    {
        public override event EventHandler<TestRunMessageEventArgs> TestRunMessage;

        public override event EventHandler<TestResultEventArgs> TestResult;

        public override event EventHandler<TestRunCompleteEventArgs> TestRunComplete;

        public void SendTestRunMessage(TestRunMessageEventArgs ev)
        {
            TestRunMessage?.Invoke(this, ev);
        }

        public void SendTestRunComplete(TestRunCompleteEventArgs ev)
        {
            TestRunComplete?.Invoke(this, ev);
        }

        public void SendTestResult(TestResultEventArgs ev)
        {
            TestResult?.Invoke(this, ev);
        }
    }
}
