namespace TeamCity.VSTest.TestLogger.Tests.Helpers
{
    using System;
    using System.Collections.Generic;
    using JetBrains.TeamCity.ServiceMessages;
    using JetBrains.TeamCity.ServiceMessages.Write.Special;

    internal class Root : ITeamCityWriter
    {
        private readonly List<string> _lines;
        private readonly string _name;

        public Root(List<string> lines, string name = "root")
        {
            if (lines == null) throw new ArgumentNullException(nameof(lines));
            _lines = lines;
            _name = name;
            _lines.Add($"+ {_name}");
        }

        public ITeamCityWriter OpenBlock(string blockName)
        {
            return new Root(_lines, "block");
        }

        public ITeamCityWriter OpenFlow()
        {
            return new Root(_lines, "flow");
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
            return new Test(_lines, testName);
        }

        public ITeamCityWriter OpenCompilationBlock(string compilerName)
        {
            throw new NotImplementedException();
        }

        public void PublishArtifact(string rules)
        {
            _lines.Add($"# publish {rules}");
        }

        public void WriteBuildNumber(string buildNumber)
        {
            throw new NotImplementedException();
        }

        public void WriteBuildProblem(string identity, string description)
        {
            throw new NotImplementedException();
        }

        public void WriteBuildParameter(string parameterName, string parameterValue)
        {
            throw new NotImplementedException();
        }

        public void WriteBuildStatistics(string statisticsKey, string statisticsValue)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _lines.Add($"- {_name}");
        }

        public void WriteRawMessage(IServiceMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
