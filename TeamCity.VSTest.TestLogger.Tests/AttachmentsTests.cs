namespace TeamCity.VSTest.TestLogger.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using JetBrains.TeamCity.ServiceMessages.Write.Special;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Moq;
    using TestLogger;
    using Xunit;

    [SuppressMessage("ReSharper", "ImplicitlyCapturedClosure")]
    public class AttachmentsTests
    {
        private readonly Mock<IOptions> _options;
        private readonly Mock<IIdGenerator> _idGenerator;
        private readonly Mock<ITeamCityWriter> _rootWriter;
        private readonly Mock<ITeamCityTestWriter> _testWriter;
        private readonly Mock<ITestAttachmentPathResolver> _testAttachmentPathResolver;
        private readonly Attachments _attachments;

        public AttachmentsTests()
        {
            _idGenerator = new Mock<IIdGenerator>();
            _options = new Mock<IOptions>();
            _rootWriter = new Mock<ITeamCityWriter>();
            _testWriter = new Mock<ITeamCityTestWriter>();
            _testAttachmentPathResolver = new Mock<ITestAttachmentPathResolver>();
            _attachments = new Attachments(_options.Object, _idGenerator.Object, _rootWriter.Object, _testAttachmentPathResolver.Object);
        }

        [Theory]
        [InlineData("file:///Images/My.jpg", "My image")]
        [InlineData("file:///Images/My.txt", "My file")]
        public void ShouldNotPublishAttachedFilesAsTestMetadataWhenNotAllowExperimental(string uri, string description)
        {
            // Given

            // When
            _options.SetupGet(i => i.AllowExperimental).Returns(false);
            _options.SetupGet(i => i.MetadataEnable).Returns(true);
            _attachments.SendAttachment("test1", new UriDataAttachment(new Uri(uri), description), _testWriter.Object);

            // Then
            _testWriter.Verify(i => i.WriteStdOutput($"Attachment \"{description}\": \"{uri}\""));
            _rootWriter.Verify(i => i.PublishArtifact(It.IsAny<string>()), Times.Never);
            _testWriter.Verify(i => i.WriteFile(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Theory]
        [InlineData("file:///Images/My.jpg", "My image")]
        [InlineData("file:///Images/My.txt", "My file")]
        public void ShouldNotPublishAttachedFilesAsTestMetadataWhenMetadataNotEnabled(string uri, string description)
        {
            // Given

            // When
            _options.SetupGet(i => i.AllowExperimental).Returns(true);
            _options.SetupGet(i => i.MetadataEnable).Returns(false);
            _attachments.SendAttachment("test1", new UriDataAttachment(new Uri(uri), description), _testWriter.Object);

            // Then
            _testWriter.Verify(i => i.WriteStdOutput($"Attachment \"{description}\": \"{uri}\""));
            _rootWriter.Verify(i => i.PublishArtifact(It.IsAny<string>()), Times.Never);
            _testWriter.Verify(i => i.WriteFile(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Theory]
        [InlineData("file:///Images/My.jpg", "My image")]
        [InlineData("file:///Images/My.txt", "My file")]
        public void ShouldNotPublishAttachedFilesAsTestMetadataWhenTeamCityVersionIsLessThen2018_2(string uri, string description)
        {
            // Given

            // When
            _options.SetupGet(i => i.AllowExperimental).Returns(true);
            _options.SetupGet(i => i.MetadataEnable).Returns(true);
            _options.SetupGet(i => i.TestMetadataSupportVersion).Returns(new TeamCityVersion("2018.2"));
            _options.SetupGet(i => i.Version).Returns(new TeamCityVersion("2018.1"));
            _attachments.SendAttachment("test1", new UriDataAttachment(new Uri(uri), description), _testWriter.Object);

            // Then
            _testWriter.Verify(i => i.WriteStdOutput($"Attachment \"{description}\": \"{uri}\""));
            _rootWriter.Verify(i => i.PublishArtifact(It.IsAny<string>()), Times.Never);
            _testWriter.Verify(i => i.WriteFile(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Theory]
        [InlineData("file:///Images/My.jpg", "My image", "/Images/My.jpg => .teamcity/VSTest/{0}/id", ".teamcity/VSTest/{0}/id/My.jpg", "My image")]
        [InlineData("file:///Images/My.jpg", "Images/My.jpg", "/Images/My.jpg => .teamcity/VSTest/{0}/id", ".teamcity/VSTest/{0}/id/My.jpg", "Images/My.jpg")]
        [InlineData("file:///c:/Images/My.jpg", "c:\\Images\\My.jpg", "c:\\Images\\My.jpg => .teamcity/VSTest/{0}/id", ".teamcity/VSTest/{0}/id/My.jpg", "")]
        public void ShouldPublishAttachedImage(string uri, string description, string publish, string destination, string imageDescription)
        {
            // Given
            const string testName = "test1";
            _idGenerator.Setup(i => i.NewId()).Returns("id");

            // When
            _options.SetupGet(i => i.AllowExperimental).Returns(true);
            _options.SetupGet(i => i.MetadataEnable).Returns(true);
            _options.SetupGet(i => i.TestMetadataSupportVersion).Returns(new TeamCityVersion("2018.2"));
            _options.SetupGet(i => i.Version).Returns(new TeamCityVersion("2018.3"));
            _testAttachmentPathResolver.Setup(i => i.Resolve(testName)).Returns(testName);
            _attachments.SendAttachment(testName, new UriDataAttachment(new Uri(uri), description), _testWriter.Object);

            // Then
            _testWriter.Verify(i => i.WriteStdOutput(It.IsAny<string>()), Times.Never);
            _rootWriter.Verify(i => i.PublishArtifact(String.Format(publish, testName)));
            _testWriter.Verify(i => i.WriteImage(String.Format(destination, testName), imageDescription));
        }

        [Theory]
        [InlineData("file:///Data/My.txt", "My data", "/Data/My.txt => .teamcity/VSTest/{0}/id", ".teamcity/VSTest/test1/id/My.txt")]
        public void ShouldPublishAttachedFile(string uri, string description, string publish, string destination)
        {
            // Given
            const string testName = "test1";
            _idGenerator.Setup(i => i.NewId()).Returns("id");

            // When
            _options.SetupGet(i => i.AllowExperimental).Returns(true);
            _options.SetupGet(i => i.MetadataEnable).Returns(true);
            _options.SetupGet(i => i.TestMetadataSupportVersion).Returns(new TeamCityVersion("2018.2"));
            _options.SetupGet(i => i.Version).Returns(new TeamCityVersion("2018.3"));
            _testAttachmentPathResolver.Setup(i => i.Resolve(testName)).Returns(testName);
            _attachments.SendAttachment("test1", new UriDataAttachment(new Uri(uri), description), _testWriter.Object);

            // Then
            _testWriter.Verify(i => i.WriteStdOutput(It.IsAny<string>()), Times.Never);
            _rootWriter.Verify(i => i.PublishArtifact(String.Format(publish, testName)));
            _testWriter.Verify(i => i.WriteFile(destination, description));
        }

        [Theory]
        [InlineData("test1", "test1")]
        [InlineData("test1(\"abc\", 10)", "test100280022abc0022002c 100029")]
        [InlineData("test1(!@#$%^&*(){}[]`~)", "test1002800210040002300240025005e0026002a00280029007b007d005b005d0060007e0029")]
        public void ShouldPublishAttachedImageWhenTestHasSpecialChars(string testName, string dirName)
        {
            // Given
            _idGenerator.Setup(i => i.NewId()).Returns("id");

            // When
            _options.SetupGet(i => i.AllowExperimental).Returns(true);
            _options.SetupGet(i => i.MetadataEnable).Returns(true);
            _options.SetupGet(i => i.TestMetadataSupportVersion).Returns(new TeamCityVersion("2018.2"));
            _options.SetupGet(i => i.Version).Returns(new TeamCityVersion("2018.3"));
            _testAttachmentPathResolver.Setup(i => i.Resolve(dirName)).Returns(dirName);
            _attachments.SendAttachment(testName, new UriDataAttachment(new Uri("file:///Images/My.jpg"), "My image"), _testWriter.Object);

            // Then
            _testWriter.Verify(i => i.WriteStdOutput(It.IsAny<string>()), Times.Never);
            _rootWriter.Verify(i => i.PublishArtifact($"/Images/My.jpg => .teamcity/VSTest/{dirName}/id"));
            _testWriter.Verify(i => i.WriteImage($".teamcity/VSTest/{dirName}/id/My.jpg", "My image"));
        }
    }
}