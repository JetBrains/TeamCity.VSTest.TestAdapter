namespace TeamCity.VSTest.TestLogger
{
    using System;
    using JetBrains.TeamCity.ServiceMessages.Write;
    using JetBrains.TeamCity.ServiceMessages.Write.Special;
    using JetBrains.TeamCity.ServiceMessages.Write.Special.Impl.Updater;

    internal static class ServiceLocator
    {
        [CanBeNull] private static ITeamCityWriter _sharedTeamCityWriter = new TeamCityServiceMessages().CreateWriter(Console.WriteLine);

        public static bool Initialize()
        {
            try
            {
                _sharedTeamCityWriter = new TeamCityServiceMessages(
                    new ServiceMessageFormatter(),
                    new FlowIdGenerator(),
                    new IServiceMessageUpdater[] { new TimestampUpdater(() => DateTime.Now) }).CreateWriter(Console.WriteLine);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static ITeamCityWriter GetTeamCityWriter()
            => _sharedTeamCityWriter ?? throw new InvalidOperationException("Not initialized");

        public static ITestCaseFilter GetTestCaseFilter()
            => new TestCaseFilter();

        public static ISuiteNameProvider GetSuiteNameProvider()
            => new SuiteNameProvider();
    }
}