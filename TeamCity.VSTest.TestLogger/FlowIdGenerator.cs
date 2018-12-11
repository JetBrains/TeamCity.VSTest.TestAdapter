namespace TeamCity.VSTest.TestLogger
{
    using System;
    using JetBrains.TeamCity.ServiceMessages.Write.Special;

    internal class FlowIdGenerator : IFlowIdGenerator
    {
        private readonly IIdGenerator _idGenerator;

        public FlowIdGenerator(IIdGenerator idGenerator)
        {
            _idGenerator = idGenerator ?? throw new ArgumentNullException(nameof(idGenerator));
        }

        public string NewFlowId() => _idGenerator.NewId();
    }
}
