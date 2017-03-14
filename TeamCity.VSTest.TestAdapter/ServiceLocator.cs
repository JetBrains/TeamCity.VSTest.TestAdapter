namespace TeamCity.VSTest.TestAdapter
{
    using System;
    using JetBrains.TeamCity.ServiceMessages.Write.Special;

    internal class ServiceLocator
    {
        private static readonly ITeamCityWriter SharedTeamCityWriter = new TeamCityServiceMessages().CreateWriter(Console.WriteLine);
        
        public static ITeamCityWriter GetTeamCityWriter()
        {
            return SharedTeamCityWriter;
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