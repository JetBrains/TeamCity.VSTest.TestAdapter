namespace TeamCity.VSTest.TestAdapter
{
    using System;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;

    internal class TestCaseFilter : ITestCaseFilter
    {
        internal const string TeamcityPrefix = "##teamcity";
        private readonly IEnvironmentInfo _environmentInfo;
        private bool _alreadyProducesTeamCityServiceMessages;

        public TestCaseFilter([NotNull] IEnvironmentInfo environmentInfo)
        {
            if (environmentInfo == null) throw new ArgumentNullException(nameof(environmentInfo));
            _environmentInfo = environmentInfo;
        }

        public void RegisterOutputMessage(string outputLine)
        {
            if (outputLine == null) throw new ArgumentNullException(nameof(outputLine));
            if (_alreadyProducesTeamCityServiceMessages)
            {
                return;
            }

            _alreadyProducesTeamCityServiceMessages = outputLine.Trim().ToLowerInvariant().Contains(TeamcityPrefix);
        }

        public bool IsSupported(TestCase testCase)
        {
            if (testCase == null) throw new ArgumentNullException(nameof(testCase));
            if (!_environmentInfo.IsUnderTeamCity)
            {
                return false;
            }

            return !_alreadyProducesTeamCityServiceMessages;
        }
    }
}