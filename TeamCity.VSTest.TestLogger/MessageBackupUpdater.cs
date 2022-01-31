namespace TeamCity.VSTest.TestLogger
{
    using System;
    using JetBrains.TeamCity.ServiceMessages;
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
    }
}