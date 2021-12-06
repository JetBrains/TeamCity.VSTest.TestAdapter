#if NET35
namespace TeamCity.VSTest.TestLogger
{
    using System;
    using JetBrains.TeamCity.ServiceMessages.Write;
    using JetBrains.TeamCity.ServiceMessages.Write.Special;
    using JetBrains.TeamCity.ServiceMessages.Write.Special.Impl.Updater;

    internal class ServiceLocatorNet35: Options
    {
        public IMessageHandler CreateMessageHandler()
        {
            var idGenerator = new IdGenerator();

            var teamCityWriter = new TeamCityServiceMessages(
                new ServiceMessageFormatter(),
                new FlowIdGenerator(idGenerator, this),
                new IServiceMessageUpdater[] {new TimestampUpdater(() => DateTime.Now)}).CreateWriter(Console.WriteLine);

            return new MessageHandler(
                teamCityWriter,
                new TestCaseFilter(),
                new SuiteNameProvider(),
                 this,
                new Attachments(this, idGenerator, teamCityWriter),
                new TestNameProvider());
        }
    }
}
#endif