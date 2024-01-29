namespace TeamCity.VSTest.TestLogger.MessageWriters;

using System;
using System.Text;

internal class FileMessageWriter(IBytesWriter messageBytesWriter) : IMessageWriter
{
    public void Write(string message)
    {
        var messageToWrite = message + Environment.NewLine;
        var messageBytes = Encoding.UTF8.GetBytes(messageToWrite);
        messageBytesWriter.Write(messageBytes);
    }

    public void Dispose()
    {
        messageBytesWriter.Dispose();
    }
}