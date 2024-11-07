namespace TeamCity.VSTest.TestLogger;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

internal class FailedTestsReportWriter : IFailedTestsReportWriter
{
    private readonly IOptions _options;
    private readonly IBytesWriterFactory _bytesWriterFactory;
    private readonly IBytesWriter? _reportWriter;
    private readonly HashSet<string> _reportedTests;

    public FailedTestsReportWriter(IOptions options, IBytesWriterFactory bytesWriterFactory)
    {
        _options = options;
        _bytesWriterFactory = bytesWriterFactory;
        _reportWriter = InitFileMessageWriter();
        _reportedTests = new HashSet<string>();
    }


    private bool EnsureFailedTestsFileSavePathDirectoryExists()
    {
        if (string.IsNullOrEmpty(_options.FailedTestsReportSavePath))
            return false;

        try
        {
            if (!Directory.Exists(_options.FailedTestsReportSavePath))
                Directory.CreateDirectory(_options.FailedTestsReportSavePath);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public void ReportFailedTest(TestCase testCase)
    {
        if (_reportWriter == null)
            return;

        if (Strings.IsNullOrWhiteSpace(testCase.FullyQualifiedName))
            return;

        var testName = testCase.FullyQualifiedName;
        if (!_reportedTests.Add(testName))
            return;
        
        var bytesToWrite = Encoding.UTF8.GetBytes(testName + Environment.NewLine);
        _reportWriter.Write(bytesToWrite);
        _reportWriter.Flush();
    }

    private IBytesWriter? InitFileMessageWriter()
    {
        if (!EnsureFailedTestsFileSavePathDirectoryExists())
            return null;

        var messagesFilePath = Path.Combine(_options.FailedTestsReportSavePath,
            Guid.NewGuid().ToString("n")) + ".txt";
        return _bytesWriterFactory.Create(messagesFilePath);
    }

    public void Dispose() => _reportWriter?.Dispose();
}