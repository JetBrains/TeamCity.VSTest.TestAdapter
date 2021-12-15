namespace TeamCity.VSTest.TestLogger
{
    using System;
    using System.Linq;
    using JetBrains.TeamCity.ServiceMessages;
    using JetBrains.TeamCity.ServiceMessages.Write;
    using JetBrains.TeamCity.ServiceMessages.Write.Special;

    internal class MessageBackupUpdater: IServiceMessageUpdater
    {
        private readonly IOptions _options;
        private long _index;
        private readonly bool _allowServiceMessageBackup;

        public MessageBackupUpdater(IOptions options)
        {
            _options = options;
            _allowServiceMessageBackup = options.AllowServiceMessageBackup;
        }

        public IServiceMessage UpdateServiceMessage(IServiceMessage message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (!_allowServiceMessageBackup)
            {
                return message;
            }
            
            return new PatchedServiceMessage(message)
            {
                { "source", _options.ServiceMessagesSource },
                { "index", (_index++).ToString() }
            };
        }
        
        private class PatchedServiceMessage : ServiceMessage
        {
            public PatchedServiceMessage([NotNull] IServiceMessage message)
                : base(message.Name)
            {
                if (message == null) throw new ArgumentNullException(nameof (message));
                AddRange(message.Keys.ToDictionary(x => x, message.GetValue));
            }
        }
    }
}