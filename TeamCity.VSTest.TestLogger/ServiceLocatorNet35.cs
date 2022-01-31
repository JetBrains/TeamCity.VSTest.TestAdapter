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
            var indicesWriter = new BytesWriter(ServiceIndicesFile);
            var messagesWriter = new BytesWriter(ServiceMessagesFile);
            var messageWriter = new MessageWriter(this, indicesWriter, messagesWriter);
            var eventContext = new EventContext();

            var teamCityWriter = new TeamCityServiceMessages(
                new ServiceMessageFormatter(),
                new FlowIdGenerator(idGenerator, this),
                new IServiceMessageUpdater[]
                {
                    new TimestampUpdater(() => DateTime.Now),
                    new MessageBackupUpdater(this),
                    new TestInfoUpdater(eventContext)
                }).CreateWriter(message => messageWriter.Write(message));

            return new MessageHandler(
                teamCityWriter,
                new TestCaseFilter(),
                new SuiteNameProvider(),
                 this,
                new Attachments(this, idGenerator, teamCityWriter),
                new TestNameProvider(),
                eventContext);
        }
    }
}
#endif