namespace TeamCity.VSTest.TestLogger
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using JetBrains.TeamCity.ServiceMessages.Write.Special;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;

    internal class Attachments : IAttachments
    {
        private static readonly Regex AttachmentDescriptionRegex = new Regex("(.*)=>(.+)", RegexOptions.Compiled);
        private readonly IOptions _options;
        private readonly IIdGenerator _idGenerator;
        private readonly ITeamCityWriter _rootWriter;

        public Attachments(
            [NotNull] IOptions options,
            [NotNull] IIdGenerator idGenerator,
            [NotNull] ITeamCityWriter rootWriter)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _idGenerator = idGenerator ?? throw new ArgumentNullException(nameof(idGenerator));
            _rootWriter = rootWriter ?? throw new ArgumentNullException(nameof(rootWriter));
        }

        public void SendAttachment(string testName, UriDataAttachment attachment, ITeamCityTestWriter testWriter)
        {
            if (testName == null) throw new ArgumentNullException(nameof(testName));
            if (attachment == null) throw new ArgumentNullException(nameof(attachment));
            if (testWriter == null) throw new ArgumentNullException(nameof(testWriter));

            if (!_options.MetadataEnable || !_options.AllowExperimental || _options.Version.CompareTo(_options.TestMetadataSupportVersion) < 0)
            {
                testWriter.WriteStdOutput($"Attachment \"{attachment.Description}\": \"{attachment.Uri}\"");
                return;
            }

            if (!attachment.Uri.IsFile)
            {
                return;
            }

            var filePath = attachment.Uri.LocalPath;
            if (string.IsNullOrEmpty(filePath))
            {
                return;
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
                var testDirName = new string(NormalizeTestName(testName).ToArray());
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

        private static IEnumerable<char> NormalizeTestName(IEnumerable<char> testName)
        {
            foreach (var ch in testName)
            {
                if (char.IsLetterOrDigit(ch) || char.IsWhiteSpace(ch))
                {
                    yield return ch;
                }
                else
                {
                    foreach (var chr in $"{(ulong)ch:x4}")
                    {
                        yield return chr;
                    }
                }
            }
        }
    }
}
