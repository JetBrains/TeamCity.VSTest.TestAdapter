namespace TeamCity.VSTest.TestLogger.Tests.Helpers
{
    using System;

    public class CommandLineResult
    {
        public CommandLineResult(
            [NotNull] CommandLine commandLine,
            int exitCode,
            [NotNull] string stdOut,
            [NotNull] string stdError)
        {
            if (commandLine == null) throw new ArgumentNullException(nameof(commandLine));
            if (stdOut == null) throw new ArgumentNullException(nameof(stdOut));
            if (stdError == null) throw new ArgumentNullException(nameof(stdError));
            CommandLine = commandLine;
            ExitCode = exitCode;
            StdOut = stdOut;
            StdError = stdError;
        }

        public CommandLine CommandLine { [NotNull] get; }

        public int ExitCode { get; }

        public string StdOut { [NotNull] get; }

        public string StdError { [NotNull] get; }
    }
}