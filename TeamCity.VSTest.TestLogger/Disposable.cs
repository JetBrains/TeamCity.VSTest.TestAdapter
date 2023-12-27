// ReSharper disable RedundantUsingDirective
// ReSharper disable UnusedMember.Global
namespace TeamCity.VSTest.TestLogger;

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

internal static class Disposable
{
    public static readonly IDisposable Empty = EmptyDisposable.Shared;

    [MethodImpl((MethodImplOptions)0x100)]
    public static IDisposable Create(Action action)
    {
#if DEBUG
        if (action == null) throw new ArgumentNullException(nameof(action));
#endif
        return new DisposableAction(action);
    }
        
    [MethodImpl((MethodImplOptions)0x100)]
    public static IDisposable Create(IEnumerable<IDisposable?> disposables)
    {
#if DEBUG
        if (disposables == null) throw new ArgumentNullException(nameof(disposables));
#endif
        return new CompositeDisposable(disposables);
    }

    private sealed class DisposableAction(Action action, object? key = null) : IDisposable
    {
        private readonly object _key = key ?? action;
        private int _counter;

        public void Dispose()
        {
            if (Interlocked.Increment(ref _counter) != 1) return;
            action();
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is DisposableAction other && Equals(_key, other._key);
        }

        public override int GetHashCode() => _key.GetHashCode();
    }

    private sealed class CompositeDisposable(IEnumerable<IDisposable?> disposables) : IDisposable
    {
        private int _counter;

        public void Dispose()
        {
            if (Interlocked.Increment(ref _counter) != 1) return;
            foreach (var disposable in disposables)
            {
                disposable?.Dispose();
            }
        }
    }

    private sealed class EmptyDisposable : IDisposable
    {
        public static readonly IDisposable Shared = new EmptyDisposable();

        private EmptyDisposable() { }

        public void Dispose() { }
    }
}