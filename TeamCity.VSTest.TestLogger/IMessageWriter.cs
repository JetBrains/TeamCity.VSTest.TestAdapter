namespace TeamCity.VSTest.TestLogger;

using System;

internal interface IMessageWriter : IDisposable
{
    void Write(string message);
}