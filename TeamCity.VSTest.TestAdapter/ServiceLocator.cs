namespace TeamCity.VSTest.TestAdapter
{
    using System;
    using JetBrains.TeamCity.ServiceMessages.Write.Special;

    internal class ServiceLocator
    {
        private static readonly ITeamCityWriter SharedTeamCityWriter = new TeamCityServiceMessages().CreateWriter(Console.WriteLine);
        private static readonly ITestCaseFilter SharedTestCaseFilter = new TestCaseFilter();

        public static ITeamCityWriter GetTeamCityWriter()
        {
            return SharedTeamCityWriter;
        }

        public static ITestCaseFilter GetTestCaseFilter()
        {
            return SharedTestCaseFilter;
        }
    }
}
