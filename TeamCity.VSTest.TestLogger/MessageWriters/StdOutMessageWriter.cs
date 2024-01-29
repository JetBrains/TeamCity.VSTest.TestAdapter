namespace TeamCity.VSTest.TestLogger.MessageWriters;

using System;

public class StdOutMessageWriter : IMessageWriter
{
    public void Write(string message)
    {
        var messageToWrite = message + Environment.NewLine;
        Console.WriteLine(messageToWrite);
    }

    public void Dispose() {}
}