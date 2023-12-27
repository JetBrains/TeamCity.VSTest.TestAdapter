// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ReturnTypeCanBeEnumerable.Global
namespace TeamCity.VSTest.TestLogger.Tests.Helpers;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

public class CommandLine(string executableFile, params string[] args)
{
    private readonly Dictionary<string, string?> _environmentVariables = new();

    public CommandLine(CommandLine? commandLine, params string[] additionalArgs)
        :this(
            commandLine?.ExecutableFile ?? throw new ArgumentNullException(nameof(commandLine)),
            commandLine.Args.Concat(additionalArgs ?? throw new ArgumentNullException(nameof(additionalArgs))).ToArray())
    {
    }

    public string ExecutableFile { get; } = executableFile ?? throw new ArgumentNullException(nameof(executableFile));

    public string[] Args { get; } = args ?? throw new ArgumentNullException(nameof(args));

    public static string WorkingDirectory => Path.GetFullPath(Path.Combine(typeof(CommandLine).GetTypeInfo().Assembly.Location, "../../../../../"));

    public void AddEnvironmentVariable(string name, string value)
    {
        ArgumentNullException.ThrowIfNull(name);
        _environmentVariables.Add(name, value);
    }

    public bool TryExecute(out CommandLineResult? result)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                WorkingDirectory = WorkingDirectory,
                FileName = ExecutableFile,
                Arguments = string.Join(" ", Args.Select(i => $"\"{i}\"").ToArray()),
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                UseShellExecute = false
            }
        };

        foreach (var envVar in _environmentVariables)
        {
#if NET45
                if (envVar.Value == null)
                {
                    process.StartInfo.EnvironmentVariables.Remove(envVar.Key);
                }
                else
                {
                    process.StartInfo.EnvironmentVariables[envVar.Key] = envVar.Value;
                }
#else
            if (envVar.Value == null)
            {
                process.StartInfo.Environment.Remove(envVar.Key);
            }
            else
            {
                process.StartInfo.Environment[envVar.Key] = envVar.Value;
            }
#endif
        }

        var stdOut = new StringBuilder();
        var stdError = new StringBuilder();
        process.OutputDataReceived += (_, args) => { stdOut.AppendLine(args.Data); };
        process.ErrorDataReceived += (_, args) => { stdError.AppendLine(args.Data); };

        Console.WriteLine($"Run: {process.StartInfo.FileName} {process.StartInfo.Arguments}");
        if (!process.Start())
        {
            result = default;
            return false;
        }

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.WaitForExit();
        result = new CommandLineResult(this, process.ExitCode, stdOut.ToString(), stdError.ToString());
        return true;
    }

    public override string ToString()
    {
        return string.Join(" ", Enumerable.Repeat(ExecutableFile, 1).Concat(Args).Select(i => $"\"{i}\""));
    }
}