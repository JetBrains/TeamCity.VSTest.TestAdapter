// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.VSTest.TestLogger
{
    using System;

    internal class EventContext : IEventRegistry, IEventContext
    {
        private TestEvent? _event;

        public IDisposable Register(TestEvent testEvent)
        {
            var prevEvent = _event;
            _event = testEvent;
            return Disposable.Create(() => { _event = prevEvent; });
        }

        public bool TryGetEvent(out TestEvent? testEvent)
        {
            if (_event != null)
            {
                testEvent = _event;
                return true;
            }

            testEvent = default;
            return false;
        }
    }
}