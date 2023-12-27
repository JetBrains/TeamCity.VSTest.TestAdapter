// ReSharper disable UnusedParameter.Global
namespace TeamCity.VSTest.TestLogger;

using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

internal interface IMessageHandler
{
    void OnTestRunStart(string testRunDescription, bool shouldOpenNewFlow);

    void OnTestRunMessage(TestRunMessageEventArgs ev);
        
    void OnTestResult(TestResultEventArgs ev);

    void OnTestRunComplete();
}