namespace TeamCity.VSTest.TestAdapter
{
    using System;
    using JetBrains.TeamCity.ServiceMessages.Write.Special;

    internal class ServiceLocator
    {
        private static readonly IEnvironmentInfo SharedEnvironmentInfo = new EnvironmentInfo();
        private static readonly ITeamCityWriter SharedTeamCityWriter = new TeamCityServiceMessages().CreateWriter(Console.WriteLine);
        private static readonly ITestCaseFilter SharedTestCaseFilter = new TestCaseFilter(SharedEnvironmentInfo);

        public static ITeamCityWriter GetTeamCityWriter()
        {
            return SharedTeamCityWriter;
        }

        public static ITestCaseFilter GetTestCaseFilter()
        {
            return SharedTestCaseFilter;
        }

        public static ISuiteNameProvider GetSuiteNameProvider()
        {
            return new SuiteNameProvider();
        }
    }
}