namespace TeamCity.VSTest.TestAdapter.Tests
{
    using System.Collections.Generic;
    using JetBrains.TeamCity.ServiceMessages;
    using JetBrains.TeamCity.ServiceMessages.Write.Special;

    internal class Suite: ITeamCityTestsSubWriter, ITeamCityMessageWriter
    {
        private readonly List<string> _lines;
        private readonly string _source;

        public Suite(List<string> lines, string source)
        {
            _lines = lines;
            _source = source;
            _lines.Add($"+ suite {_source}");
        }

        public ITeamCityTestsSubWriter OpenTestSuite(string suiteName)
        {
            throw new System.NotImplementedException();
        }

        public ITeamCityTestWriter OpenTest(string testName)
        {
            return new Test(_lines, $"{_source}/{testName}" );
        }

        public ITeamCityTestsSubWriter OpenFlow()
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
            _lines.Add($"- suite {_source}");
        }

        public void WriteMessage(string text)
        {
            _lines.Add($"# suite {_source} message {text}");
        }

        public void WriteWarning(string text)
        {
            _lines.Add($"# suite {_source} warning {text}");
        }

        public void WriteError(string text, string errorDetails = null)
        {
            _lines.Add($"# suite {_source} error {text}");
        }
    }
}
