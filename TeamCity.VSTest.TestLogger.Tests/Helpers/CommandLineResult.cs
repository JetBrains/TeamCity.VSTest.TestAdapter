namespace TeamCity.VSTest.TestLogger.Tests.Helpers
{
    using System;

    public class CommandLineResult
    {
        public CommandLineResult(
            CommandLine commandLine,
            int exitCode,
            string stdOut,
            string stdError)
        {
            CommandLine = commandLine ?? throw new ArgumentNullException(nameof(commandLine));
            ExitCode = exitCode;
            StdOut = stdOut ?? throw new ArgumentNullException(nameof(stdOut));
            StdError = stdError ?? throw new ArgumentNullException(nameof(stdError));
        }

        public CommandLine CommandLine { get; }

        public int ExitCode { get; }

        public string StdOut { get; }

        public string StdError { get; }
    }
}