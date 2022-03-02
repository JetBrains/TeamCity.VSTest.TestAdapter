namespace TeamCity.VSTest.TestLogger.Tests
{
    using JetBrains.TeamCity.ServiceMessages.Write;
    using Moq;
    using Shouldly;
    using Xunit;

    public class MessageBackupUpdaterTests
    {
        private readonly Mock<IOptions> _options = new Mock<IOptions>();

        public MessageBackupUpdaterTests()
        {
            _options.SetupGet(i => i.AllowServiceMessageBackup).Returns(true);
        }

        [Fact]
        public void ShouldUpdateMessage()
        {
            // Given
            var updater = CreateInstance();
            _options.SetupGet(i => i.ServiceMessagesSource).Returns("Src");

            // When
            var patchedMessage1 = updater.UpdateServiceMessage(new ServiceMessage("Abc"));
            var patchedMessage2 = updater.UpdateServiceMessage(new ServiceMessage("Abc"));

            // Then
            patchedMessage1.GetValue("source").ShouldBe("Src");
            patchedMessage1.GetValue("index").ShouldBe("0");
            
            patchedMessage2.GetValue("source").ShouldBe("Src");
            patchedMessage2.GetValue("index").ShouldBe("1");
        }

        private MessageBackupUpdater CreateInstance() =>
            new MessageBackupUpdater(_options.Object);
    }
}