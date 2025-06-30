namespace TeamCity.VSTest.TestLogger.MessageWriters;

using TeamCity.VSTest.TestLogger;
using System;
using System.Threading;

public class StdOutMessageWriter : IMessageWriter
{
	private const string _stdOutMutexId = "TeamCity.VSTest:Global+StdOutMutex";
	private readonly Mutex _stdOutMutex = new(false, _stdOutMutexId);
	private const int _stdOutMutexTimeoutMillisecs = 7_000;

	private bool _disposed;

	public void Write(string message)
	{
		string messageToWrite = message + Environment.NewLine;
		try
		{
			bool result = _stdOutMutex.WaitOne(_stdOutMutexTimeoutMillisecs);
			if (result)
				Console.WriteLine(messageToWrite);
			else
			{
				string errorMessage = $"Cannot obtain stdout mutex within {_stdOutMutexTimeoutMillisecs} milliseconds.";
				throw new InvalidOperationException(errorMessage);
			}
		}
		finally
		{
			_stdOutMutex.ReleaseMutex();
		}
	}

	public void Dispose()
	{
		if (!_disposed)
		{
			((IDisposable)_stdOutMutex).Dispose();
			_disposed = true;
		}
	}
}