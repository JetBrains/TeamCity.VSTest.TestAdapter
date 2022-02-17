#if !NET35
namespace TeamCity.VSTest.TestLogger
{
    using System;
    using System.Collections.Generic;
    using IoC;
    using JetBrains.TeamCity.ServiceMessages.Write;
    using JetBrains.TeamCity.ServiceMessages.Write.Special;
    using JetBrains.TeamCity.ServiceMessages.Write.Special.Impl.Updater;
    using static IoC.Lifetime;

    internal class IoCConfiguration: IConfiguration
    {
        public IEnumerable<IToken> Apply(IMutableContainer container)
        {
            var autowiringStrategy = AutowiringStrategies.AspectOriented()
                .Tag<TagAttribute>(attribute => attribute.Tag);
            
            yield return container
                .Bind<IAutowiringStrategy>().To(ctx => autowiringStrategy)
                .Bind<IMessageHandler>().To<MessageHandler>()
                .Bind<IOptions>().As(Singleton).To<Options>()
                .Bind<ITestNameProvider>().As(Singleton).To<TestNameProvider>()
                .Bind<ISuiteNameProvider>().As(Singleton).To<SuiteNameProvider>()
                .Bind<IEventRegistry>().Bind<IEventContext>().As(Singleton).To<EventContext>()
                .Bind<ITeamCityServiceMessages>().To<TeamCityServiceMessages>()
                .Bind<IServiceMessageFormatter>().To<ServiceMessageFormatter>()
                .Bind<IFlowIdGenerator>().To<FlowIdGenerator>()
                .Bind<IIdGenerator>().To<IdGenerator>()
                .Bind<DateTime>().To(ctx => DateTime.Now)
                .Bind<IServiceMessageUpdater>().Tag(typeof(TimestampUpdater)).As(Singleton).To<TimestampUpdater>()
                .Bind<IServiceMessageUpdater>().Tag(typeof(MessageBackupUpdater)).As(Singleton).To<MessageBackupUpdater>()
                .Bind<IServiceMessageUpdater>().Tag(typeof(TestInfoUpdater)).As(Singleton).To<TestInfoUpdater>()
                .Bind<IAttachments>().As(Singleton).To<Attachments>()
                .Bind<IBytesWriter>().Tag("Indices").To(ctx => new BytesWriter(ctx.Container.Inject<IOptions>().ServiceIndicesFile))
                .Bind<IBytesWriter>().Tag("Messages").To(ctx => new BytesWriter(ctx.Container.Inject<IOptions>().ServiceMessagesFile))
                .Bind<IMessageWriter>().As(Singleton).To<MessageWriter>()
                .Bind<ITeamCityWriter>().To(ctx => ctx.Container.Inject<ITeamCityServiceMessages>().CreateWriter(message => ctx.Container.Inject<IMessageWriter>().Write(message)));
        }
    }
}
#endif