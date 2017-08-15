namespace TeamCity.VSTest.TestLogger
{
    using System;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;

    internal class TestCaseFilter : ITestCaseFilter
    {
        internal const string TeamcityPrefix = "##teamcity";
        private bool _alreadyProducesTeamCityServiceMessages;

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
            return !_alreadyProducesTeamCityServiceMessages;
        }
    }
}