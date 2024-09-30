#if NET35
namespace TeamCity.VSTest.TestLogger
{
    using System;
    using JetBrains.TeamCity.ServiceMessages.Write;
    using JetBrains.TeamCity.ServiceMessages.Write.Special;
    using JetBrains.TeamCity.ServiceMessages.Write.Special.Impl.Updater;
    using MessageWriters;

    internal class ServiceLocatorNet35: Options
    {
        public ServiceLocatorNet35()
        {
            var idGenerator = new IdGenerator();
            var messageWriterFactory = new MessageWriterFactory(this);
            var eventContext = new EventContext();
            var testAttachmentPathResolver = new TestAttachmentPathResolver();

            MessageWriter = messageWriterFactory.GetMessageWriter();

            var teamCityWriter = new TeamCityServiceMessages(
                new ServiceMessageFormatter(),
                new FlowIdGenerator(idGenerator, this),
                new IServiceMessageUpdater[]
                {
                    new TimestampUpdater(() => DateTime.Now),
                    new MessageBackupUpdater(this),
                    new TestInfoUpdater(eventContext)
                }).CreateWriter(message => MessageWriter.Write(message), FallbackToStdOutTestReporting);

            MessageHandler = new MessageHandler(
                teamCityWriter,
                new SuiteNameProvider(),
                new Attachments(this, idGenerator, teamCityWriter, testAttachmentPathResolver),
                new TestNameProvider(),
                eventContext,
                new FailedTestsReportWriter(this, new BytesWriterFactory()),
                this);
        }

        public IMessageWriter MessageWriter { get; }

        public IMessageHandler MessageHandler { get; }
    }
}
#endif
