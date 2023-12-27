// ReSharper disable UnusedMember.Global
namespace TeamCity.VSTest.TestLogger.Tests.Helpers;

using System;

public class CommandLineResult(
    CommandLine commandLine,
    int exitCode,
    string stdOut,
    string stdError)
{
    public CommandLine CommandLine { get; } = commandLine ?? throw new ArgumentNullException(nameof(commandLine));

    public int ExitCode { get; } = exitCode;

    public string StdOut { get; } = stdOut ?? throw new ArgumentNullException(nameof(stdOut));

    public string StdError { get; } = stdError ?? throw new ArgumentNullException(nameof(stdError));
}