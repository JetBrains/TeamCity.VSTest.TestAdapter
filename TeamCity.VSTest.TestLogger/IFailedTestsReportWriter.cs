namespace TeamCity.VSTest.TestLogger;

using System;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

internal interface IFailedTestsReportWriter : IDisposable
{
    void ReportFailedTest(TestCase testName);
}