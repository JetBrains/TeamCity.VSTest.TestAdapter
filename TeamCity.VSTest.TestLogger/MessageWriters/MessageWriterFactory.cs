namespace TeamCity.VSTest.TestLogger.MessageWriters
{
    using System;
    using System.IO;

    internal class MessageWriterFactory
    {
        private readonly IOptions _options;

        public MessageWriterFactory(IOptions options)
        {
            _options = options;
        }

        public IMessageWriter GetMessageWriter()
        {
            if (_options.FallbackToStdOutTestReporting == false && EnsureServiceMessagesFileDirectoryExists())
            {
                return GetFileMessageWriter();
            }

            return _options.AllowServiceMessageBackup
                ? GetStdOutWriterWithBackup()
                : new StdOutMessageWriter();
        }

        private bool EnsureServiceMessagesFileDirectoryExists()
        {
            if (string.IsNullOrEmpty(_options.ServiceMessagesFileSavePath))
            {
                return false;
            }

            try
            {
                if (!Directory.Exists(_options.ServiceMessagesFileSavePath))
                {
                    Directory.CreateDirectory(_options.ServiceMessagesFileSavePath);
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
            var messagesFilePath = Path.Combine(_options.ServiceMessagesFileSavePath, Guid.NewGuid().ToString("n")) + ".msg";
            var messageBytesWriter = new BytesWriter(messagesFilePath);
            return new FileMessageWriter(messageBytesWriter);
        }

        private IMessageWriter GetStdOutWriterWithBackup()
        {
            var indicesFilePath = Path.Combine(_options.ServiceMessagesBackupPath, _options.ServiceMessagesBackupSource);
            var messagesFilePath = Path.Combine(_options.ServiceMessagesBackupPath, _options.ServiceMessagesBackupSource) + ".msg";
            var indicesBytesWriter = new BytesWriter(indicesFilePath);
            var messageBytesWriter = new BytesWriter(messagesFilePath);

            return new StdOutMessageWriterWithBackup(indicesBytesWriter, messageBytesWriter);
        }
    }
}
