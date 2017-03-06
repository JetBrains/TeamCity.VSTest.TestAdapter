namespace TeamCity.VSTestAdapter.Tests
{
    using System.Collections.Generic;
    using JetBrains.TeamCity.ServiceMessages;
    using JetBrains.TeamCity.ServiceMessages.Write.Special;

    internal class Root: ITeamCityWriter
    {
        private readonly List<string> _lines;

        public Root(List<string> lines)
        {
            _lines = lines;
            _lines.Add("+ root");
        }

        public ITeamCityWriter OpenBlock(string blockName)
        {
            throw new System.NotImplementedException();
        }

        public ITeamCityWriter OpenFlow()
        {
            throw new System.NotImplementedException();
        }

        public void WriteMessage(string text)
        {
            throw new System.NotImplementedException();
        }

        public void WriteWarning(string text)
        {
            throw new System.NotImplementedException();
        }

        public void WriteError(string text, string errorDetails = null)
        {
            throw new System.NotImplementedException();
        }

        public ITeamCityTestsSubWriter OpenTestSuite(string suiteName)
        {
            return new Suite(_lines, suiteName);
        }

        public ITeamCityTestWriter OpenTest(string testName)
        {
            throw new System.NotImplementedException();
        }

        public ITeamCityWriter OpenCompilationBlock(string compilerName)
        {
            throw new System.NotImplementedException();
        }

        public void PublishArtifact(string rules)
        {
            throw new System.NotImplementedException();
        }

        public void WriteBuildNumber(string buildNumber)
        {
            throw new System.NotImplementedException();
        }

        public void WriteBuildProblem(string identity, string description)
        {
            throw new System.NotImplementedException();
        }

        public void WriteBuildParameter(string parameterName, string parameterValue)
        {
            throw new System.NotImplementedException();
        }

        public void WriteBuildStatistics(string statisticsKey, string statisticsValue)
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
            _lines.Add("- root");
        }

        public void WriteRawMessage(IServiceMessage message)
        {
            throw new System.NotImplementedException();
        }
    }
}
