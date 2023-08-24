﻿namespace TeamCity.VSTest.TestLogger
{
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

    internal interface IMessageHandler
    {
        void OnTestRunStart(string testRunDescription, bool shouldOpenNewFlow);

        void OnTestRunMessage([NotNull] TestRunMessageEventArgs ev);
        
        void OnTestResult([NotNull] TestResultEventArgs ev);

        void OnTestRunComplete();
    }
}
