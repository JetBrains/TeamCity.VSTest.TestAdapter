namespace TeamCity.VSTest.TestLogger.MessageWriters;

using System;
using System.IO;

internal class MessageWriterFactory(IOptions options)
{
    public IMessageWriter GetMessageWriter()
    {
        if (options.FallbackToStdOutTestReporting == false && EnsureServiceMessagesFileDirectoryExists())
        {
            return GetFileMessageWriter();
        }

        return options.AllowServiceMessageBackup
            ? GetStdOutWriterWithBackup()
            : new StdOutMessageWriter();
    }

    private bool EnsureServiceMessagesFileDirectoryExists()
    {
        if (string.IsNullOrEmpty(options.ServiceMessagesFileSavePath))
        {
            return false;
        }

        try
        {
            if (!Directory.Exists(options.ServiceMessagesFileSavePath))
            {
                Directory.CreateDirectory(options.ServiceMessagesFileSavePath);
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    private IMessageWriter GetFileMessageWriter()
    {
        var messagesFilePath = Path.Combine(options.ServiceMessagesFileSavePath, Guid.NewGuid().ToString("n")) + ".msg";
        var messageBytesWriter = new BytesWriter(messagesFilePath);
        return new FileMessageWriter(messageBytesWriter);
    }

    private IMessageWriter GetStdOutWriterWithBackup()
    {
        var indicesFilePath = Path.Combine(options.ServiceMessagesBackupPath, options.ServiceMessagesBackupSource);
        var messagesFilePath = Path.Combine(options.ServiceMessagesBackupPath, options.ServiceMessagesBackupSource) + ".msg";
        var indicesBytesWriter = new BytesWriter(indicesFilePath);
        var messageBytesWriter = new BytesWriter(messagesFilePath);

        return new StdOutMessageWriterWithBackup(indicesBytesWriter, messageBytesWriter);
    }
}