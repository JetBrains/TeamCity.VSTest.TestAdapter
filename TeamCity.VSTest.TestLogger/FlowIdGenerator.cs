// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.VSTest.TestLogger;

using System;
using JetBrains.TeamCity.ServiceMessages.Write.Special;

internal class FlowIdGenerator(IIdGenerator idGenerator, IOptions options) : IFlowIdGenerator
{
    private readonly IIdGenerator _idGenerator = idGenerator ?? throw new ArgumentNullException(nameof(idGenerator));
    private bool _isFirst = true;

    public string NewFlowId()
    {
        // ReSharper disable once InvertIf
        if (_isFirst)
        {
            _isFirst = false;
            var flowId = options.RootFlowId;
            if (!string.IsNullOrEmpty(flowId))
            {
                return flowId;
            }
        }
            
        return _idGenerator.NewId();
    }
}