namespace TeamCity.VSTest.TestLogger.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Helpers;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
    using Moq;
    using Shouldly;
    using TestLogger;
    using Xunit;

    [SuppressMessage("ReSharper", "ImplicitlyCapturedClosure")]
    public class MessageHandlerTests
    {
        private readonly List<string> _lines = new List<string>();
        private readonly IMessageHandler _events;
        private readonly Mock<ITestCaseFilter> _testCaseFilter;
        private readonly Mock<ISuiteNameProvider> _suiteNameProvider;
        private readonly Mock<IOptions> _options;
        private readonly Mock<IIdGenerator> _idGenerator;

        public MessageHandlerTests()
        {
            _lines.Clear();

            _testCaseFilter = new Mock<ITestCaseFilter>();
            _testCaseFilter.Setup(i => i.IsSupported(It.IsAny<TestCase>())).Returns(true);

            _suiteNameProvider = new Mock<ISuiteNameProvider>();
            _suiteNameProvider.Setup(i => i.GetSuiteName(It.IsAny<string>(), It.IsAny<string>())).Returns<string, string>((baseDir, source) => source);

            _idGenerator = new Mock<IIdGenerator>();
            _options = new Mock<IOptions>();

            var root = new Root(_lines);
            _events = new MessageHandler(root, _testCaseFilter.Object, _suiteNameProvider.Object, _idGenerator.Object, _options.Object);
        }

        private static TestResultEventArgs CreateTestResult(
            TestOutcome outcome = TestOutcome.Passed,
            string fullyQualifiedName = "test1",
            string source = "assembly.dll",
            string errorMessage = null,
            string errorStackTrace = null,
            string extensionId = TeamCityTestLogger.ExtensionId)
        {
            return new TestResultEventArgs(
                new TestResult(
                    new TestCase(
                        fullyQualifiedName,
                        new Uri(extensionId),
                        source))
                {
                    Outcome = outcome,
                    Duration = TimeSpan.FromSeconds(1),
                    ErrorMessage = errorMessage,
                    ErrorStackTrace = errorStackTrace
                });
        }

        [Fact]
        public void ShouldNotProduceAnyMessagesWhenTestCaseFilterFiltersAllMessages()
        {
            // Given
            var testResult = CreateTestResult(TestOutcome.Passed, "test2");

            // When
            _testCaseFilter.Setup(i => i.IsSupported(testResult.Result.TestCase)).Returns(false);
            _events.OnTestResult(testResult);
            _events.OnTestRunComplete();

            // Then
            _lines.ShouldNotBeEmpty();
        }

        [Fact]
        public void ShouldRegisterOutputMessageInTestCaseFilterWhenReceiveTestRunMessageEventArgs()
        {
            // Given
            var testResult = CreateTestResult();

            // When
            _testCaseFilter.Setup(i => i.IsSupported(testResult.Result.TestCase)).Returns(false);
            _events.OnTestRunMessage(new TestRunMessageEventArgs(TestMessageLevel.Error, "err"));
            _events.OnTestRunMessage(new TestRunMessageEventArgs(TestMessageLevel.Informational, "abc"));
            _events.OnTestRunMessage(new TestRunMessageEventArgs(TestMessageLevel.Warning, "warn"));
            _events.OnTestResult(testResult);
            _events.OnTestRunComplete();

            // Then
            _testCaseFilter.Verify(i => i.RegisterOutputMessage("err"), Times.Never);
            _testCaseFilter.Verify(i => i.RegisterOutputMessage("abc"), Times.Once);
            _testCaseFilter.Verify(i => i.RegisterOutputMessage("warn"), Times.Never);
        }

        [Fact]
        public void ShouldProcessWhenFailedTest()
        {
            // Given

            // When
            _events.OnTestResult(CreateTestResult(TestOutcome.Failed, "test1", "assembly.dll", "errorInfo", "stackTrace"));
            _events.OnTestRunComplete();

            // Then
            _lines.ShouldBe(new[]
            {
                "+ root"
                , "+ suite assembly.dll"
                , "+ test assembly.dll/test1"
                , "# test assembly.dll/test1 duration 00:00:01"
                , "! test assembly.dll/test1 errorInfo stackTrace"
                , "- test assembly.dll/test1"
                , "- suite assembly.dll"
                , "- root"
            });
        }

        [Fact]
        public void ShouldProcessWhenPassedTest()
        {
            // Given

            // When
            _events.OnTestResult(CreateTestResult());
            _events.OnTestRunComplete();

            // Then
            _lines.ShouldBe(new[]
            {
                "+ root"
                , "+ suite assembly.dll"
                , "+ test assembly.dll/test1"
                , "# test assembly.dll/test1 duration 00:00:01"
                , "- test assembly.dll/test1"
                , "- suite assembly.dll"
                , "- root"
            });
        }

        [Fact]
        public void ShouldProcessWhenPassedTestWithMessages()
        {
            // Given
            var testResult = CreateTestResult();

            // When
            testResult.Result.Messages.Add(new TestResultMessage(TestResultMessage.StandardOutCategory, "some text"));
            testResult.Result.Messages.Add(new TestResultMessage(TestResultMessage.AdditionalInfoCategory, "additional text"));
            testResult.Result.Messages.Add(new TestResultMessage(TestResultMessage.DebugTraceCategory, "trace text"));
            testResult.Result.Messages.Add(new TestResultMessage(TestResultMessage.StandardErrorCategory, "error text"));
            _events.OnTestResult(testResult);
            _events.OnTestRunComplete();


            // Then
            _lines.ShouldBe(new[]
            {
                "+ root"
                , "+ suite assembly.dll"
                , "+ test assembly.dll/test1"
                , "# test assembly.dll/test1 duration 00:00:01"
                , "# test assembly.dll/test1 message some text"
                , "# test assembly.dll/test1 message additional text"
                , "# test assembly.dll/test1 message trace text"
                , "# test assembly.dll/test1 error error text"
                , "- test assembly.dll/test1"
                , "- suite assembly.dll"
                , "- root"
            });
        }

        [Fact]
        public void ShouldProcessWhenSeveralPassedTests()
        {
            // Given

            // When
            _events.OnTestResult(CreateTestResult());
            _events.OnTestResult(CreateTestResult(TestOutcome.Passed, "test2"));
            _events.OnTestRunComplete();

            // Then
            _lines.ShouldBe(new[]
            {
                "+ root"
                , "+ suite assembly.dll"
                , "+ test assembly.dll/test1"
                , "# test assembly.dll/test1 duration 00:00:01"
                , "- test assembly.dll/test1"
                , "+ test assembly.dll/test2"
                , "# test assembly.dll/test2 duration 00:00:01"
                , "- test assembly.dll/test2"
                , "- suite assembly.dll"
                , "- root"
            });
        }

        [Fact]
        public void ShouldProcessWhenSeveralPassedTestsInDifferentSuites()
        {
            // Given

            // When
            _events.OnTestResult(CreateTestResult());
            _events.OnTestResult(CreateTestResult(TestOutcome.Passed, "test2"));
            _events.OnTestResult(CreateTestResult(TestOutcome.Passed, "test3", "assembly2.dll"));
            _events.OnTestResult(CreateTestResult(TestOutcome.Passed, "test4", "assembly2.dll"));
            _events.OnTestRunComplete();

            // Then
            _lines.ShouldBe(new[]
            {
                "+ root"
                , "+ suite assembly.dll"
                , "+ test assembly.dll/test1"
                , "# test assembly.dll/test1 duration 00:00:01"
                , "- test assembly.dll/test1"
                , "+ test assembly.dll/test2"
                , "# test assembly.dll/test2 duration 00:00:01"
                , "- test assembly.dll/test2"
                , "- suite assembly.dll"
                , "+ suite assembly2.dll"
                , "+ test assembly2.dll/test3"
                , "# test assembly2.dll/test3 duration 00:00:01"
                , "- test assembly2.dll/test3"
                , "+ test assembly2.dll/test4"
                , "# test assembly2.dll/test4 duration 00:00:01"
                , "- test assembly2.dll/test4"
                , "- suite assembly2.dll"
                , "- root"
            });
        }

        [Fact]
        public void ShouldProcessWhenSkippedTest()
        {
            // Given

            // When
            _events.OnTestResult(CreateTestResult(TestOutcome.Skipped, "test1", "assembly.dll", "reason"));
            _events.OnTestRunComplete();

            // Then
            _lines.ShouldBe(new[]
            {
                "+ root"
                , "+ suite assembly.dll"
                , "+ test assembly.dll/test1"
                , "# test assembly.dll/test1 duration 00:00:01"
                , "? test assembly.dll/test1 reason"
                , "- test assembly.dll/test1"
                , "- suite assembly.dll"
                , "- root"
            });
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ShouldProcessWhenSkippedTestWithoutReason(string reason)
        {
            // Given

            // When
            _events.OnTestResult(CreateTestResult(TestOutcome.Skipped, "test1", "assembly.dll", reason));
            _events.OnTestRunComplete();

            // Then
            _lines.ShouldBe(new[]
            {
                "+ root"
                , "+ suite assembly.dll"
                , "+ test assembly.dll/test1"
                , "# test assembly.dll/test1 duration 00:00:01"
                , "? test assembly.dll/test1"
                , "- test assembly.dll/test1"
                , "- suite assembly.dll"
                , "- root"
            });
        }

        [Fact]
        public void ShouldProduceMessagesWhenTestCaseFilterFiltersNotAllMessages()
        {
            // Given
            var testResult1 = CreateTestResult();
            var testResult2 = CreateTestResult(TestOutcome.Passed, "test2");

            // When
            _testCaseFilter.Setup(i => i.IsSupported(testResult1.Result.TestCase)).Returns(false);
            _testCaseFilter.Setup(i => i.IsSupported(testResult2.Result.TestCase)).Returns(true);
            _events.OnTestResult(testResult1);
            _events.OnTestResult(testResult2);
            _events.OnTestRunComplete();

            // Then
            // Then
            _lines.ShouldBe(new[]
            {
                "+ root"
                , "+ suite assembly.dll"
                , "+ test assembly.dll/test2"
                , "# test assembly.dll/test2 duration 00:00:01"
                , "- test assembly.dll/test2"
                , "- suite assembly.dll"
                , "- root"
            });
        }

        [Fact]
        public void ShouldUseSuiteNameProvider()
        {
            // Given

            // When
            _events.OnTestResult(CreateTestResult());
            _events.OnTestRunComplete();

            // Then
            _suiteNameProvider.Verify(i => i.GetSuiteName(null, "assembly.dll"), Times.Once);
            _suiteNameProvider.Verify(i => i.Reset(), Times.Once);
        }

        [Theory]
        [InlineData("file:///Images/My.jpg", "My image", "# test assembly.dll/test1 message Attachment \"My image\": \"file:///Images/My.jpg\"")]
        [InlineData("file:///Images/My.txt", "My file", "# test assembly.dll/test1 message Attachment \"My file\": \"file:///Images/My.txt\"")]
        public void ShouldNotPublishAttachedFilesAsTestMetadataWhenNotAllowExperimental(string uri, string description, string expectedMessage)
        {
            // Given
            var testResult = CreateTestResult();
            var attachmentSet = new AttachmentSet(new Uri("file:///abc"), "attachments");
            attachmentSet.Attachments.Add(new UriDataAttachment(new Uri(uri), description));
            testResult.Result.Attachments.Add(attachmentSet);

            // When
            _options.SetupGet(i => i.AllowExperimental).Returns(false);
            _options.SetupGet(i => i.MetadataEnable).Returns(true);
            _events.OnTestResult(testResult);
            _events.OnTestRunComplete();

            // Then
            _lines.ShouldBe(new[]
            {
                "+ root"
                , "+ suite assembly.dll"
                , "+ test assembly.dll/test1"
                , "# test assembly.dll/test1 duration 00:00:01"
                , expectedMessage
                , "- test assembly.dll/test1"
                , "- suite assembly.dll"
                , "- root"
            });
        }

        [Theory]
        [InlineData("file:///Images/My.jpg", "My image", "# test assembly.dll/test1 message Attachment \"My image\": \"file:///Images/My.jpg\"")]
        [InlineData("file:///Images/My.txt", "My file", "# test assembly.dll/test1 message Attachment \"My file\": \"file:///Images/My.txt\"")]
        public void ShouldNotPublishAttachedFilesAsTestMetadataWhenMetadataNotEnabled(string uri, string description, string expectedMessage)
        {
            // Given
            var testResult = CreateTestResult();
            var attachmentSet = new AttachmentSet(new Uri("file:///abc"), "attachments");
            attachmentSet.Attachments.Add(new UriDataAttachment(new Uri(uri), description));
            testResult.Result.Attachments.Add(attachmentSet);

            // When
            _options.SetupGet(i => i.AllowExperimental).Returns(true);
            _options.SetupGet(i => i.MetadataEnable).Returns(false);
            _events.OnTestResult(testResult);
            _events.OnTestRunComplete();

            // Then
            _lines.ShouldBe(new[]
            {
                "+ root"
                , "+ suite assembly.dll"
                , "+ test assembly.dll/test1"
                , "# test assembly.dll/test1 duration 00:00:01"
                , expectedMessage
                , "- test assembly.dll/test1"
                , "- suite assembly.dll"
                , "- root"
            });
        }

        [Theory]
        [InlineData("file:///Images/My.jpg", "My image", "# test assembly.dll/test1 message Attachment \"My image\": \"file:///Images/My.jpg\"")]
        [InlineData("file:///Images/My.txt", "My file", "# test assembly.dll/test1 message Attachment \"My file\": \"file:///Images/My.txt\"")]
        public void ShouldNotPublishAttachedFilesAsTestMetadataWhenTeamCityVersionIsLessThen2018_2(string uri, string description, string expectedMessage)
        {
            // Given
            var testResult = CreateTestResult();
            var attachmentSet = new AttachmentSet(new Uri("file:///abc"), "attachments");
            attachmentSet.Attachments.Add(new UriDataAttachment(new Uri(uri), description));
            testResult.Result.Attachments.Add(attachmentSet);

            // When
            _options.SetupGet(i => i.AllowExperimental).Returns(true);
            _options.SetupGet(i => i.MetadataEnable).Returns(true);
            _options.SetupGet(i => i.TestMetadataSupportVersion).Returns(new TeamCityVersion("2018.2"));
            _options.SetupGet(i => i.Version).Returns(new TeamCityVersion("2018.1"));
            _events.OnTestResult(testResult);
            _events.OnTestRunComplete();

            // Then
            _lines.ShouldBe(new[]
            {
                "+ root"
                , "+ suite assembly.dll"
                , "+ test assembly.dll/test1"
                , "# test assembly.dll/test1 duration 00:00:01"
                , expectedMessage
                , "- test assembly.dll/test1"
                , "- suite assembly.dll"
                , "- root"
            });
        }

        [Theory]
        [InlineData("file:///Images/My.jpg", "My image", "# publish /Images/My.jpg => .teamcity/VSTest/test1/id", "# test assembly.dll/test1 image .teamcity/VSTest/test1/id/My.jpg as My image")]
        [InlineData("file:///Data/My.txt", "My data", "# publish /Data/My.txt => .teamcity/VSTest/test1/id", "# test assembly.dll/test1 artifact .teamcity/VSTest/test1/id/My.txt as My data")]
        [InlineData("file:///Images/My.jpg", "/Images/My.jpg", "# publish /Images/My.jpg => .teamcity/VSTest/test1/id", "# test assembly.dll/test1 image .teamcity/VSTest/test1/id/My.jpg as ")]
        [InlineData("file:///c:/Images/My.jpg", "c:\\Images\\My.jpg", "# publish c:\\Images\\My.jpg => .teamcity/VSTest/test1/id", "# test assembly.dll/test1 image .teamcity/VSTest/test1/id/My.jpg as ")]
        public void ShouldPublishAttachedFilesAsTestMetadataWhenAllowsExperimentalAndMetadataEnabled(string uri, string description, string expectedPublishMessage, string expectedMetadataMessage)
        {
            // Given
            var testResult = CreateTestResult();
            var attachmentSet = new AttachmentSet(new Uri("file:///abc"), "attachments");
            attachmentSet.Attachments.Add(new UriDataAttachment(new Uri(uri), description));
            testResult.Result.Attachments.Add(attachmentSet);
            _idGenerator.Setup(i => i.NewId()).Returns("id");

            // When
            _options.SetupGet(i => i.AllowExperimental).Returns(true);
            _options.SetupGet(i => i.MetadataEnable).Returns(true);
            _options.SetupGet(i => i.TestMetadataSupportVersion).Returns(new TeamCityVersion("2018.2"));
            _options.SetupGet(i => i.Version).Returns(new TeamCityVersion("2018.3"));
            _events.OnTestResult(testResult);
            _events.OnTestRunComplete();

            // Then
            _lines.ShouldBe(new[]
            {
                "+ root"
                , "+ suite assembly.dll"
                , "+ test assembly.dll/test1"
                , "# test assembly.dll/test1 duration 00:00:01"
                , expectedPublishMessage
                , expectedMetadataMessage
                , "- test assembly.dll/test1"
                , "- suite assembly.dll"
                , "- root"
            });
        }
    }
}