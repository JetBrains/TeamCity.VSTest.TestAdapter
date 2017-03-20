namespace TeamCity.VSTest.TestAdapter
{
    using System;
    using JetBrains.TeamCity.ServiceMessages.Write.Special;

    internal class ServiceLocator
    {
        [CanBeNull] private static ITeamCityWriter _sharedTeamCityWriter = new TeamCityServiceMessages().CreateWriter(Console.WriteLine);

        public static bool Initialize()
        {
            try
            {
                _sharedTeamCityWriter = new TeamCityServiceMessages().CreateWriter(Console.WriteLine);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static ITeamCityWriter GetTeamCityWriter()
        {
            if (_sharedTeamCityWriter == null)
            {
                throw new InvalidOperationException("Not initialized");
            }

            return _sharedTeamCityWriter;
        }

        public static ITestCaseFilter GetTestCaseFilter()
        {
            return new TestCaseFilter();
        }

        public static ISuiteNameProvider GetSuiteNameProvider()
        {
            return new SuiteNameProvider();
        }
    }
}