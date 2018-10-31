namespace TeamCity.VSTest.TestLogger
{
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

    internal interface IMessageHandler
    {
        void OnTestRunMessage([NotNull] TestRunMessageEventArgs ev);
        
        void OnTestResult([NotNull] TestResultEventArgs ev);

        void OnTestRunComplete();
    }
}