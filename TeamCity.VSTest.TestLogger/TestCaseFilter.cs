namespace TeamCity.VSTest.TestLogger
{
    using System;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;

    internal class TestCaseFilter : ITestCaseFilter
    {
        private const string TeamcityPrefix = "##teamcity[";
        private const string XUnitPrefix = "[xunit.net";
        private const string XUnut2LoggerName = "executor://xunit/vstestrunner2";
        private bool _alreadyProducesTeamCityServiceMessagesFromXUnit;

        public void RegisterOutputMessage(string outputLine)
        {
            if (outputLine == null) throw new ArgumentNullException(nameof(outputLine));
            if (_alreadyProducesTeamCityServiceMessagesFromXUnit)
            {
                return;
            }

            var message = outputLine.Trim().ToLowerInvariant();
            _alreadyProducesTeamCityServiceMessagesFromXUnit = message.Contains(TeamcityPrefix) && message.StartsWith(XUnitPrefix);
        }

        public bool IsSupported(TestCase testCase)
        {
            if (testCase == null) throw new ArgumentNullException(nameof(testCase));
            var isXUnit = XUnut2LoggerName == testCase.ExecutorUri.ToString().ToLowerInvariant();
            return !(isXUnit && _alreadyProducesTeamCityServiceMessagesFromXUnit);
        }
    }
}