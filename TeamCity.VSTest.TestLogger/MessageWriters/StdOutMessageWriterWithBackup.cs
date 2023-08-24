// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.VSTest.TestLogger.MessageWriters
{
    using System;
    using System.Text;

    internal class StdOutMessageWriterWithBackup : IMessageWriter
    {
        private readonly IBytesWriter _indicesWriter;
        private readonly IBytesWriter _messagesWriter;
        private bool _allowServiceMessageBackup = true;
        private ulong _position;

        public StdOutMessageWriterWithBackup(IBytesWriter indicesWriter, IBytesWriter messagesWriter)
        {
            _indicesWriter = indicesWriter;
            _messagesWriter = messagesWriter;
        }

        public void Write(string message)
        {
            var messageToWrite = message + Environment.NewLine;
            if (_allowServiceMessageBackup)
            {
                try
                {
                    var messageBytes = Encoding.UTF8.GetBytes(messageToWrite);
                    _messagesWriter.Write(messageBytes);
                    _messagesWriter.Flush();
                    _position += (ulong)messageBytes.Length;

                    // Write ulong in Java style
                    var positionBytes = BitConverter.GetBytes(_position);
                    Array.Reverse(positionBytes);
                    _indicesWriter.Write(positionBytes);
                    _indicesWriter.Flush();
                }
                catch
                {
                    _allowServiceMessageBackup = false;
                }
            }
            
            Console.Write(messageToWrite);
        }

        public void Flush() { }
    }
}
