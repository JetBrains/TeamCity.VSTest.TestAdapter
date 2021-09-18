// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.VSTest.TestLogger
{
    using System;
    using JetBrains.TeamCity.ServiceMessages.Write.Special;

    internal class FlowIdGenerator : IFlowIdGenerator
    {
        private readonly IIdGenerator _idGenerator;
        private readonly IOptions _options;
        private bool _isFirst = true;

        public FlowIdGenerator(IIdGenerator idGenerator, IOptions options)
        {
            _idGenerator = idGenerator ?? throw new ArgumentNullException(nameof(idGenerator));
            _options = options;
        }

        public string NewFlowId()
        {
            // ReSharper disable once InvertIf
            if (_isFirst)
            {
                _isFirst = false;
                var flowId = _options.RootFlowId;
                if (!string.IsNullOrEmpty(flowId))
                {
                    return flowId;
                }
            }
            
            return _idGenerator.NewId();
        }
    }
}
