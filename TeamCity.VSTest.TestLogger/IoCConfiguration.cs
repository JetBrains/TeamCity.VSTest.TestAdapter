#if !NET35
namespace TeamCity.VSTest.TestLogger
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
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
                .Bind<IAutowiringStrategy>().To<CustomAutowiringStrategy>()
                .Bind<IMessageHandler>().To<MessageHandler>()
                .Bind<IOptions>().As(Singleton).To<Options>()
                .Bind<ITestCaseFilter>().To<TestCaseFilter>()
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

        [SuppressMessage("ReSharper", "IdentifierTypo")]
        private class CustomAutowiringStrategy : IAutowiringStrategy
        {
            private readonly IAutowiringStrategy _baseStrategy;

            public CustomAutowiringStrategy([NotNull] [IoC.NotNull] IAutowiringStrategy baseStrategy) =>
                _baseStrategy = baseStrategy ?? throw new ArgumentNullException(nameof(baseStrategy));

            // Preferable constructor with maximum number of parameters
            public bool TryResolveConstructor(IEnumerable<IMethod<ConstructorInfo>> constructors, out IMethod<ConstructorInfo> constructor) =>
                (constructor = constructors.OrderByDescending(i => i.Info.GetParameters().Length).FirstOrDefault()) != null;

            public bool TryResolveType(Type registeredType, Type resolvingType, out Type instanceType) =>
                _baseStrategy.TryResolveType(registeredType, resolvingType, out instanceType);

            public bool TryResolveInitializers(IEnumerable<IMethod<MethodInfo>> methods, out IEnumerable<IMethod<MethodInfo>> initializers) =>
                _baseStrategy.TryResolveInitializers(methods, out initializers);
        }
    }
}
#endif
