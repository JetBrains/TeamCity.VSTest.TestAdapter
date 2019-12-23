namespace TeamCity.VSTest.TestLogger
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using JetBrains.TeamCity.ServiceMessages.Write.Special;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

    internal class MessageHandler : IMessageHandler
    {
        private static readonly Regex AttachmentDescriptionRegex = new Regex("(.*)=>(.+)", RegexOptions.Compiled);
        private static readonly HashSet<char> InvalidPathChars = new HashSet<char>(new[] {'{', '}'}.Concat(Path.GetInvalidPathChars().Concat(Path.GetInvalidFileNameChars())));
        [NotNull] private readonly ITeamCityWriter _rootWriter;
        [NotNull] private readonly ISuiteNameProvider _suiteNameProvider;
        private readonly IIdGenerator _idGenerator;
        [NotNull] private readonly IOptions _options;
        [NotNull] private readonly ITestCaseFilter _testCaseFilter;
        [CanBeNull] private string _testSuiteSource;
        [CanBeNull] private ITeamCityTestsSubWriter _testSuiteWriter;

        internal MessageHandler(
            [NotNull] ITeamCityWriter rootWriter,
            [NotNull] ITestCaseFilter testCaseFilter,
            [NotNull] ISuiteNameProvider suiteNameProvider,
            [NotNull] IIdGenerator idGenerator,
            [NotNull] IOptions options)
        {
            _rootWriter = rootWriter ?? throw new ArgumentNullException(nameof(rootWriter));
            _testCaseFilter = testCaseFilter ?? throw new ArgumentNullException(nameof(testCaseFilter));
            _suiteNameProvider = suiteNameProvider ?? throw new ArgumentNullException(nameof(suiteNameProvider));
            _idGenerator = idGenerator ?? throw new ArgumentNullException(nameof(idGenerator));
            _options = options;
        }

        public void OnTestRunMessage(TestRunMessageEventArgs ev)
        {
            if (ev == null) throw new ArgumentNullException(nameof(ev));
            if (ev.Level == TestMessageLevel.Informational && !Strings.IsNullOrWhiteSpace(ev.Message))
            {
                _testCaseFilter.RegisterOutputMessage(ev.Message);
            }
        }

        public void OnTestResult(TestResultEventArgs ev)
        {
            if (ev == null) throw new ArgumentNullException(nameof(ev));

            var result = ev.Result;
            var testCase = result.TestCase;
            if (!_testCaseFilter.IsSupported(testCase))
            {
                return;
            }

            var suiteName = _suiteNameProvider.GetSuiteName(_options.TestRunDirectory, testCase.Source);
            var testSuiteWriter = GetTestSuiteWriter(suiteName);
            var testName = testCase.FullyQualifiedName ?? testCase.DisplayName ?? testCase.Id.ToString();
            using (var testWriter = testSuiteWriter.OpenTest(testName))
            {
                // ReSharper disable once SuspiciousTypeConversion.Global
                testWriter.WriteDuration(result.Duration);
                if (result.Messages != null && result.Messages.Count > 0)
                {
                    foreach (var message in result.Messages)
                    {
                        if (TestResultMessage.StandardOutCategory.Equals(message.Category, StringComparison.CurrentCultureIgnoreCase)
                            || TestResultMessage.AdditionalInfoCategory.Equals(message.Category, StringComparison.CurrentCultureIgnoreCase)
                            || TestResultMessage.DebugTraceCategory.Equals(message.Category, StringComparison.CurrentCultureIgnoreCase))
                        {
                            testWriter.WriteStdOutput(message.Text);
                            continue;
                        }

                        if (TestResultMessage.StandardErrorCategory.Equals(message.Category, StringComparison.CurrentCultureIgnoreCase))
                        {
                            testWriter.WriteErrOutput(message.Text);
                        }
                    }
                }

                foreach (var attachments in result.Attachments)
                {
                    foreach (var attachment in attachments.Attachments)
                    {
                        if (!_options.MetadataEnable ||!_options.AllowExperimental || _options.Version.CompareTo(_options.TestMetadataSupportVersion) < 0)
                        {
                            testWriter.WriteStdOutput($"Attachment \"{attachment.Description}\": \"{attachment.Uri}\"");
                            continue;
                        }

                        if (!attachment.Uri.IsFile)
                        {
                            continue;
                        }

                        var filePath = attachment.Uri.LocalPath;
                        if (string.IsNullOrEmpty(filePath))
                        {
                            continue;
                        }

                        var description = attachment.Description ?? string.Empty;
                        if (description == filePath)
                        {
                            description = string.Empty;
                        }

                        var fileName = Path.GetFileName(filePath);
                        var fileExtension = Path.GetExtension(fileName).ToLowerInvariant();
                        string artifactDir = null;
                        if (!string.IsNullOrEmpty(description))
                        {
                            var match = AttachmentDescriptionRegex.Match(description);
                            if (match.Success)
                            {
                                description = match.Groups[1].Value.Trim();
                                artifactDir = match.Groups[2].Value.Trim();
                            }
                        }

                        if (artifactDir == null)
                        {
                            var testDirName = new string(testName.Select(c => InvalidPathChars.Contains(c) ? '_' : c).ToArray());
                            artifactDir = ".teamcity/VSTest/" + testDirName + "/" + _idGenerator.NewId();
                        }

                        _rootWriter.PublishArtifact(filePath + " => " + artifactDir);
                        var artifact = artifactDir + "/" + fileName;
                        switch (fileExtension)
                        {
                            case ".bmp":
                            case ".gif":
                            case ".ico":
                            case ".jng":
                            case ".jpeg":
                            case ".jpg":
                            case ".jfif":
                            case ".jp2":
                            case ".jps":
                            case ".tga":
                            case ".tiff":
                            case ".tif":
                            case ".svg":
                            case ".wmf":
                            case ".emf":
                            case ".png":
                                testWriter.WriteImage(artifact, description);
                                break;

                            default:
                                testWriter.WriteFile(artifact, description);
                                break;
                        }
                    }
                }

                switch (result.Outcome)
                {
                    case TestOutcome.Passed:
                        break;

                    case TestOutcome.Failed:
                        testWriter.WriteFailed(result.ErrorMessage ?? string.Empty, result.ErrorStackTrace ?? string.Empty);
                        break;

                    case TestOutcome.Skipped:
                    case TestOutcome.None: // https://github.com/JetBrains/TeamCity.VSTest.TestAdapter/issues/23
                    case TestOutcome.NotFound:
                        if (string.IsNullOrEmpty(result.ErrorMessage))
                            testWriter.WriteIgnored();
                        else
                            testWriter.WriteIgnored(result.ErrorMessage);

                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(result.Outcome), result.Outcome, "Invalid value");
                }
            }
        }

        public void OnTestRunComplete()
        {
            _testSuiteWriter?.Dispose();
            _rootWriter.Dispose();
            _suiteNameProvider.Reset();
        }

        [NotNull]
        private ITeamCityTestsSubWriter GetTestSuiteWriter([NotNull] string source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (_testSuiteWriter != null && _testSuiteSource == source)
                return _testSuiteWriter;

            _testSuiteWriter?.Dispose();
            _testSuiteSource = source;
            var testSuiteWriter = _rootWriter.OpenTestSuite(source);
            _testSuiteWriter = testSuiteWriter;
            return testSuiteWriter;
        }
    }
}
