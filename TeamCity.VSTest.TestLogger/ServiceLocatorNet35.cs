#if NET35
namespace TeamCity.VSTest.TestLogger
{
    using System;
    using JetBrains.TeamCity.ServiceMessages.Write;
    using JetBrains.TeamCity.ServiceMessages.Write.Special;
    using JetBrains.TeamCity.ServiceMessages.Write.Special.Impl.Updater;

    internal class ServiceLocatorNet35: IOptions
    {
        public IMessageHandler CreateMessageHandler()
        {
            var teamCityWriter = new TeamCityServiceMessages(
                new ServiceMessageFormatter(),
                new FlowIdGenerator(),
                new IServiceMessageUpdater[] {new TimestampUpdater(() => DateTime.Now)}).CreateWriter(Console.WriteLine);

            return new MessageHandler(
                teamCityWriter,
                new TestCaseFilter(),
                new SuiteNameProvider(),
                this);
        }

        public string TestRunDirectory { get; set; }
    }
}
#endif