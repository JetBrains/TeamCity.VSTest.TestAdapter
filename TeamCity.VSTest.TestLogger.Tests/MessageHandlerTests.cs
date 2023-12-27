namespace TeamCity.VSTest.TestLogger.Tests;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Helpers;
using JetBrains.TeamCity.ServiceMessages.Write.Special;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Moq;
using Shouldly;
using TestLogger;
using Xunit;

[SuppressMessage("ReSharper", "ImplicitlyCapturedClosure")]
[SuppressMessage("Performance", "CA1861:Avoid constant arrays as arguments")]
public class MessageHandlerTests
{
    private readonly List<string> _lines = [];
    private readonly MessageHandler _events;
    private readonly Mock<ISuiteNameProvider> _suiteNameProvider;
    private readonly Mock<IIdGenerator> _idGenerator;
    private readonly Mock<IAttachments> _attachments;

    public MessageHandlerTests()
    {
        _lines.Clear();

        _suiteNameProvider = new Mock<ISuiteNameProvider>();
        _suiteNameProvider.Setup(i => i.GetSuiteName(It.IsAny<string>())).Returns<string>(source => source);

        _idGenerator = new Mock<IIdGenerator>();
        _attachments = new Mock<IAttachments>();

        var testNameProvider = new Mock<ITestNameProvider>();
        testNameProvider.Setup(i => i.GetTestName(It.IsAny<string>(), It.IsAny<string>())).Returns<string, string>((fullyQualifiedName, _) => fullyQualifiedName);

        var eventRegistry = new Mock<IEventRegistry>();

        var root = new Root(_lines);
        _events = new MessageHandler(root, _suiteNameProvider.Object, _attachments.Object, testNameProvider.Object, eventRegistry.Object);
    }

    private static TestResultEventArgs CreateTestResult(
        TestOutcome outcome = TestOutcome.Passed,
        string fullyQualifiedName = "test1",
        string source = "assembly.dll",
        string? errorMessage = default,
        string? errorStackTrace = default,
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
            , "+ test assembly.dll: test1"
            , "# test assembly.dll: test1 duration 00:00:01"
            , "! test assembly.dll: test1 errorInfo stackTrace"
            , "- test assembly.dll: test1"
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
            , "+ test assembly.dll: test1"
            , "# test assembly.dll: test1 duration 00:00:01"
            , "- test assembly.dll: test1"
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
            , "+ test assembly.dll: test1"
            , "# test assembly.dll: test1 duration 00:00:01"
            , "# test assembly.dll: test1 message some text"
            , "# test assembly.dll: test1 message additional text"
            , "# test assembly.dll: test1 message trace text"
            , "# test assembly.dll: test1 error error text"
            , "- test assembly.dll: test1"
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
            , "+ test assembly.dll: test1"
            , "# test assembly.dll: test1 duration 00:00:01"
            , "- test assembly.dll: test1"
            , "+ test assembly.dll: test2"
            , "# test assembly.dll: test2 duration 00:00:01"
            , "- test assembly.dll: test2"
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
            , "+ test assembly.dll: test1"
            , "# test assembly.dll: test1 duration 00:00:01"
            , "- test assembly.dll: test1"
            , "+ test assembly.dll: test2"
            , "# test assembly.dll: test2 duration 00:00:01"
            , "- test assembly.dll: test2"
            , "+ test assembly2.dll: test3"
            , "# test assembly2.dll: test3 duration 00:00:01"
            , "- test assembly2.dll: test3"
            , "+ test assembly2.dll: test4"
            , "# test assembly2.dll: test4 duration 00:00:01"
            , "- test assembly2.dll: test4"
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
            , "+ test assembly.dll: test1"
            , "# test assembly.dll: test1 duration 00:00:01"
            , "? test assembly.dll: test1 reason"
            , "- test assembly.dll: test1"
            , "- root"
        });
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ShouldProcessWhenSkippedTestWithoutReason(string? reason)
    {
        // Given

        // When
        _events.OnTestResult(CreateTestResult(TestOutcome.Skipped, "test1", "assembly.dll", reason));
        _events.OnTestRunComplete();

        // Then
        _lines.ShouldBe(new[]
        {
            "+ root"
            , "+ test assembly.dll: test1"
            , "# test assembly.dll: test1 duration 00:00:01"
            , "? test assembly.dll: test1"
            , "- test assembly.dll: test1"
            , "- root"
        });
    }

    [Fact]
    public void ShouldWrapTestOutputInBlockWithDescriptionWhenOnTestRunStartIsCalled()
    {
        // Given

        // When
        _events.OnTestRunStart("foo", false);
        _events.OnTestResult(CreateTestResult());
        _events.OnTestRunComplete();

        // Then
        _lines.ShouldBe(new[]
        {
            "+ root"
            , "+ block"
            , "+ test assembly.dll: test1"
            , "# test assembly.dll: test1 duration 00:00:01"
            , "- test assembly.dll: test1"
            , "- block"
            , "- root"
        });
    }

    [Fact]
    public void ShouldOpenFlowWhenOnTestRunStartIsCalledWithTheCorrespondingParameter()
    {
        // Given

        // When
        _events.OnTestRunStart("foo", true);
        _events.OnTestResult(CreateTestResult());
        _events.OnTestRunComplete();

        // Then
        _lines.ShouldBe(new[]
        {
            "+ root"
            , "+ flow"
            , "+ block"
            , "+ test assembly.dll: test1"
            , "# test assembly.dll: test1 duration 00:00:01"
            , "- test assembly.dll: test1"
            , "- block"
            , "- flow"
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
        _suiteNameProvider.Verify(i => i.GetSuiteName("assembly.dll"), Times.Once);
    }

    [Fact]
    public void ShouldPublishAttachedFiles()
    {
        // Given
        var attachment = new UriDataAttachment(new Uri("file:///Images/My.jpg"), "My image");
        var testResult = CreateTestResult();
        var attachmentSet = new AttachmentSet(new Uri("file:///abc"), "attachments");
        attachmentSet.Attachments.Add(attachment);
        testResult.Result.Attachments.Add(attachmentSet);
        _idGenerator.Setup(i => i.NewId()).Returns("id");

        // When
        _events.OnTestResult(testResult);
        _events.OnTestRunComplete();

        // Then
        _attachments.Verify(i => i.SendAttachment("assembly.dll: test1", attachment, It.IsAny<ITeamCityTestWriter>() ));
    }
}