namespace TeamCity.VSTest.TestLogger.MessageWriters
{
    using System;
    using System.Text;

    internal class FileMessageWriter : IMessageWriter
    {
        private readonly IBytesWriter _messageBytesWriter;

        public FileMessageWriter(IBytesWriter messageBytesWriter)
        {
            _messageBytesWriter = messageBytesWriter;
        }

        public void Write(string message)
        {
            var messageToWrite = message + Environment.NewLine;
            var messageBytes = Encoding.UTF8.GetBytes(messageToWrite);
            _messageBytesWriter.Write(messageBytes);
        }

        public void Flush()
        {
            _messageBytesWriter.Flush();
        }
    }
}
