namespace TeamCity.VSTest.TestLogger.Tests
{
    using System;
    using JetBrains.TeamCity.ServiceMessages.Write;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Moq;
    using Shouldly;
    using Xunit;

    public class TestInfoUpdaterTests
    {
        private readonly Mock<IEventContext> _eventContext = new Mock<IEventContext>();

        [Fact]
        public void ShouldUpdateMessage()
        {
            // Given
            var updater = CreateInstance();
            var testCase = new TestCase
            {
                Source = "My Source",
                DisplayName = "My DisplayName",
                CodeFilePath = "My CodeFilePath",
                FullyQualifiedName = "My FullyQualifiedName",
                Id = Guid.NewGuid(),
                ExecutorUri = new Uri("http://my"),
                LineNumber = 35
            };

            var testEvent = new TestEvent("test name", testCase);
            _eventContext.Setup(i => i.TryGetEvent(out testEvent)).Returns(true);

            // When
            var patchedMessage = updater.UpdateServiceMessage(new ServiceMessage("Abc"));

            // Then
            patchedMessage.GetValue("suiteName").ShouldBe("test name");
            patchedMessage.GetValue("testSource").ShouldBe(testCase.Source);
            patchedMessage.GetValue("displayName").ShouldBe(testCase.DisplayName);
            patchedMessage.GetValue("codeFilePath").ShouldBe(testCase.CodeFilePath);
            patchedMessage.GetValue("fullyQualifiedName").ShouldBe(testCase.FullyQualifiedName);
            patchedMessage.GetValue("id").ShouldBe(testCase.Id.ToString());
            patchedMessage.GetValue("executorUri").ShouldBe(testCase.ExecutorUri.ToString());
            patchedMessage.GetValue("lineNumber").ShouldBe(testCase.LineNumber.ToString());
        }

        private TestInfoUpdater CreateInstance() =>
            new TestInfoUpdater(_eventContext.Object);
    }
}