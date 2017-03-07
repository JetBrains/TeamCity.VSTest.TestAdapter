namespace TeamCity.VSTest.TestAdapter.Tests
{
    using System;
    using System.Collections.Generic;
    using JetBrains.TeamCity.ServiceMessages.Write.Special;

    internal class Test: ITeamCityTestWriter, ITeamCityMessageWriter
    {
        private readonly List<string> _lines;
        private readonly string _testName;

        public Test(List<string> lines, string testName)
        {
            _lines = lines;
            _testName = testName;
            _lines.Add($"+ test {_testName}");
        }

        public void Dispose()
        {
            _lines.Add($"- test {_testName}");
        }

        public void WriteStdOutput(string text)
        {
            throw new NotImplementedException();
        }

        public void WriteErrOutput(string text)
        {
            throw new NotImplementedException();
        }

        public void WriteIgnored(string ignoreReason)
        {
            _lines.Add($"? test {_testName} {ignoreReason}");
        }

        public void WriteIgnored()
        {
            _lines.Add($"? test {_testName}");
        }

        public void WriteFailed(string errorMessage, string errorDetails)
        {
            _lines.Add($"! test {_testName} {errorMessage} {errorDetails}");
        }

        public void WriteDuration(TimeSpan duration)
        {
            _lines.Add($"# test {_testName} duration {duration}");
        }

        public void WriteMessage(string text)
        {
            _lines.Add($"# test {_testName} message {text}");
        }

        public void WriteWarning(string text)
        {
            _lines.Add($"# test {_testName} warning {text}");
        }

        public void WriteError(string text, string errorDetails = null)
        {
            _lines.Add($"# test {_testName} error {text}");
        }
    }
}
