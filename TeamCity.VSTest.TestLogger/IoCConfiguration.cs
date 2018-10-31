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
        public IEnumerable<IDisposable> Apply(IContainer container)
        {
            yield return container.Bind<IMessageHandler>().To<MessageHandler>();
            yield return container.Bind<IOptions>().As(Singleton).To<Options>();
            yield return container.Bind<ITestCaseFilter>().To<TestCaseFilter>();
            yield return container.Bind<ISuiteNameProvider>().To<SuiteNameProvider>();
            yield return container.Bind<ITeamCityServiceMessages>().To<TeamCityServiceMessages>();
            yield return container.Bind<IServiceMessageFormatter>().To<ServiceMessageFormatter>();
            yield return container.Bind<IFlowIdGenerator>().To<FlowIdGenerator>();
            yield return container.Bind<IServiceMessageUpdater>().To(ctx => new TimestampUpdater(() => DateTime.Now));
            yield return container.Bind<ITeamCityWriter>().To(ctx => ctx.Container.Inject<ITeamCityServiceMessages>().CreateWriter(Console.WriteLine));
        }
    }
}
#endif
