namespace TeamCity.VSTest.TestAdapter.Tests.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Linq;

    public class CommandLine
    {
        private readonly Dictionary<string, string> _envitonmentVariables = new Dictionary<string, string>();

        public CommandLine(string executableFile, params string[] args)
        {
            ExecutableFile = executableFile;
            Args = args;
        }

        public string ExecutableFile { [NotNull]get; }

        public string[] Args { [NotNull] get; }

        public void AddEnvitonmentVariable([NotNull] string name, [NotNull] string value)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (value == null) throw new ArgumentNullException(nameof(value));
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
                process.StartInfo.EnvironmentVariables.Add(envVar.Key, envVar.Value);
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
