namespace TeamCity.VSTest.TestLogger;

using System;
using JetBrains.TeamCity.ServiceMessages;
using JetBrains.TeamCity.ServiceMessages.Write.Special;

internal class MessageBackupUpdater(IOptions options) : IServiceMessageUpdater
{
    private long _index;
    private readonly bool _allowServiceMessageBackup = options.AllowServiceMessageBackup;

    public IServiceMessage UpdateServiceMessage(IServiceMessage message)
    {
        if (message == null) throw new ArgumentNullException(nameof(message));
        if (!_allowServiceMessageBackup)
        {
            return message;
        }
            
        var patchedMessage = new PatchedServiceMessage(message);
        patchedMessage.Add("source", options.ServiceMessagesBackupSource);
        patchedMessage.Add("index", (_index++).ToString());
        return patchedMessage;
    }
}