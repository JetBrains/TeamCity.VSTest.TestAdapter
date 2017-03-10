namespace TeamCity.VSTest.TestAdapter.Tests.Helpers
{
    using System;
    using System.Collections.Generic;
    using JetBrains.TeamCity.ServiceMessages.Write.Special;

    internal class Suite : ITeamCityTestsSubWriter, ITeamCityMessageWriter
    {
        private readonly List<string> _lines;
        private readonly string _source;

        public Suite(List<string> lines, string source)
        {
            _lines = lines;
            _source = source;
            _lines.Add($"+ suite {_source}");
        }

        public void WriteMessage(string text)
        {
            throw new NotImplementedException();
        }

        public void WriteWarning(string text)
        {
            throw new NotImplementedException();
        }

        public void WriteError(string text, string errorDetails = null)
        {
            throw new NotImplementedException();
        }

        public ITeamCityTestsSubWriter OpenTestSuite(string suiteName)
        {
            throw new NotImplementedException();
        }

        public ITeamCityTestWriter OpenTest(string testName)
        {
            return new Test(_lines, $"{_source}/{testName}");
        }

        public ITeamCityTestsSubWriter OpenFlow()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _lines.Add($"- suite {_source}");
        }
    }
}