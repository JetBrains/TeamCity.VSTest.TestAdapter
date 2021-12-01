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
            yield return container
                .Bind<IMessageHandler>().To<MessageHandler>()
                .Bind<IOptions>().As(Singleton).To<Options>()
                .Bind<ITestCaseFilter>().To<TestCaseFilter>()
                .Bind<ITestNameFactory>().As(Singleton).To<TestNameFactory>()
                .Bind<ISuiteNameProvider>().As(Singleton).To<SuiteNameProvider>()
                .Bind<ITeamCityServiceMessages>().To<TeamCityServiceMessages>()
                .Bind<IServiceMessageFormatter>().To<ServiceMessageFormatter>()
                .Bind<IFlowIdGenerator>().To<FlowIdGenerator>()
                .Bind<IIdGenerator>().To<IdGenerator>()
                .Bind<DateTime>().To(ctx => DateTime.Now)
                .Bind<IServiceMessageUpdater>().To<TimestampUpdater>()
                .Bind<IAttachments>().As(Singleton).To<Attachments>()
                .Bind<ITeamCityWriter>().To(ctx => ctx.Container.Inject<ITeamCityServiceMessages>().CreateWriter(Console.WriteLine));
        }
    }
}
#endif