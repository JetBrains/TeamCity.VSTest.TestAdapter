namespace TeamCity.VSTest.TestLogger;

internal interface IEventContext
{
    bool TryGetEvent(out TestEvent? testEvent);
}