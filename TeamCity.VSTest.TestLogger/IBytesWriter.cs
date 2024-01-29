namespace TeamCity.VSTest.TestLogger;

using System;

internal interface IBytesWriter : IDisposable
{
    void Write(byte[] bytes);

    void Flush();
}