// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.VSTest.TestLogger.MessageWriters;

using System;
using System.Text;

internal class StdOutMessageWriterWithBackup(IBytesWriter indicesWriter, IBytesWriter messagesWriter) : IMessageWriter
{
    private bool _allowServiceMessageBackup = true;
    private ulong _position;

    public void Write(string message)
    {
        var messageToWrite = message + Environment.NewLine;
        if (_allowServiceMessageBackup)
        {
            try
            {
                var messageBytes = Encoding.UTF8.GetBytes(messageToWrite);
                messagesWriter.Write(messageBytes);
                messagesWriter.Flush();
                _position += (ulong)messageBytes.Length;

                // Write ulong in Java style
                var positionBytes = BitConverter.GetBytes(_position);
                Array.Reverse(positionBytes);
                indicesWriter.Write(positionBytes);
                indicesWriter.Flush();
            }
            catch
            {
                _allowServiceMessageBackup = false;
            }
        }

        Console.Write(messageToWrite);
    }

    public void Dispose() { }
}