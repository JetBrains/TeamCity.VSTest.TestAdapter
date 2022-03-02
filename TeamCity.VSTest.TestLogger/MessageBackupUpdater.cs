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
            
            var patchedMessage = new PatchedServiceMessage(message);
            patchedMessage.Add("source", _options.ServiceMessagesSource);
            patchedMessage.Add("index", (_index++).ToString());
            return patchedMessage;
        }
    }
}