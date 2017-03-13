namespace TeamCity.VSTest.TestAdapter.Tests.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;

    public class CommandLine
    {
        private readonly Dictionary<string, string> _envitonmentVariables = new Dictionary<string, string>();

        public CommandLine(string executableFile, params string[] args)
        {
            if (executableFile == null) throw new ArgumentNullException(nameof(executableFile));
            if (args == null) throw new ArgumentNullException(nameof(args));
            ExecutableFile = executableFile;
            Args = args;
        }

        public string ExecutableFile { [NotNull] get; }

        public string[] Args { [NotNull] get; }

        public void AddEnvitonmentVariable([NotNull] string name, [CanBeNull] string value)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            _envitonmentVariables.Add(name, value);
        }

        public bool TryExecute(out CommandLineResult result)
        {
            var locationUri = new Uri(GetType().Assembly.CodeBase);
            var baseDir = Path.GetFullPath(Path.Combine(locationUri.AbsolutePath, "../../../../"));
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    WorkingDirectory = baseDir,
                    FileName = ExecutableFile,
                    Arguments = string.Join(" ", Args.Select(i => $"\"{i}\"").ToArray()),
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden
                }
            };

            foreach (var envVar in _envitonmentVariables)
            {
                var remove = envVar.Value == null;
                if (process.StartInfo.EnvironmentVariables.ContainsKey(envVar.Key))
                {
                    process.StartInfo.EnvironmentVariables.Remove(envVar.Key);
                    if (!remove)
                    {
                        process.StartInfo.EnvironmentVariables.Add(envVar.Key, envVar.Value);
                    }
                }
                else
                {
                    if (!remove)
                    {
                        process.StartInfo.EnvironmentVariables.Add(envVar.Key, envVar.Value);
                    }
                }
            }

            var stdOut = new StringBuilder();
            var stdError = new StringBuilder();
            process.OutputDataReceived += (sender, args) => { stdOut.AppendLine(args.Data); };
            process.ErrorDataReceived += (sender, args) => { stdError.AppendLine(args.Data); };
            if (!process.Start())
            {
                result = default(CommandLineResult);
                return false;
            }

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit(10000);
            result = new CommandLineResult(this, process.ExitCode, stdOut.ToString(), stdError.ToString());
            return true;
        }
    }
}