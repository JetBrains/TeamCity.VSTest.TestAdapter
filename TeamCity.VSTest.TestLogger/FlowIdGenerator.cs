namespace TeamCity.VSTest.TestLogger
{
    using System;
    using JetBrains.TeamCity.ServiceMessages.Write.Special;

    internal class FlowIdGenerator : IFlowIdGenerator
    {
        public string NewFlowId()
        {
            return Guid.NewGuid().ToString().Replace("-", string.Empty);
        }
    }
}
