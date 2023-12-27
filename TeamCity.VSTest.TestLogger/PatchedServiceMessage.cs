namespace TeamCity.VSTest.TestLogger
{
    using System.Collections.Generic;
    using JetBrains.TeamCity.ServiceMessages;

    internal class PatchedServiceMessage : IServiceMessage
    {
        private readonly IServiceMessage _message;
        private readonly Dictionary<string, string> _values = new Dictionary<string, string>();

        public PatchedServiceMessage(IServiceMessage message)
        {
            _message = message;
            foreach (var key in _message.Keys)
            {
                _values[key] = message.GetValue(key);
            }
        }

        public string? GetValue(string key) => _values.TryGetValue(key, out var value) ? value : default;

        public string Name => _message.Name;

        public string DefaultValue => _message.DefaultValue;

        public IEnumerable<string> Keys => _values.Keys;

        public void Add(string name, string value) => _values[name] = value;
    }
}