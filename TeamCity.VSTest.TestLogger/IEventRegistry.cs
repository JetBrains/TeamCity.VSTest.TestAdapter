namespace TeamCity.VSTest.TestLogger;

using System;

internal interface IEventRegistry
{
    IDisposable Register(TestEvent testEvent);
}