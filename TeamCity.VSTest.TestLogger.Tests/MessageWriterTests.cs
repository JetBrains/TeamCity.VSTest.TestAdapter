namespace TeamCity.VSTest.TestLogger.Tests
{
    using System;
    using Moq;
    using Xunit;

    public class MessageWriterTests
    {
        /*
        Add this to MessageWriter Write for corrupted messages
        
        if (message.Length > 10 && Environment.GetEnvironmentVariable("AAA") == "A" && !message.Contains("flowFinished"))
        {
            messageToWrite = _rnd.Next(3) switch
            {
                1 => message.Substring(8) + Environment.NewLine,
                2 => message.Substring(8, message.Length - 8) + Environment.NewLine,
                _ => messageToWrite
            };
        }
        
        private readonly Random _rnd = new Random(2);
        */
        
        private readonly Mock<IOptions> _options = new Mock<IOptions>();
        private readonly Mock<IBytesWriter> _indicesWriter = new Mock<IBytesWriter>();
        private readonly Mock<IBytesWriter> _messagesWriter = new Mock<IBytesWriter>();

        public MessageWriterTests()
        {
            _options.SetupGet(i => i.AllowServiceMessageBackup).Returns(true);
        }

        [Fact]
        public void ShouldWriteIndicesAndMessages()
        {
            // Given
            var writer = CreateInstance();

            // When
            writer.Write("Hi");
            writer.Write("Abc");

            // Then
            // "Hi"
            _indicesWriter.Verify(i => i.Write(new byte[] {0, 0, 0, 0, 0, 0, 0, 4}));
            _messagesWriter.Verify(i => i.Write(new byte[] {72, 105, 13, 10}));
            
            // Abc
            _indicesWriter.Verify(i => i.Write(new byte[] {0, 0, 0, 0, 0, 0, 0, 9}));
            _messagesWriter.Verify(i => i.Write(new byte[] {65, 98, 99, 13, 10}));
        }
        
        [Fact]
        public void ShouldStopWritingWhenExceptionWasRegisteredOnce()
        {
            // Given
            var writer = CreateInstance();

            // When
            _indicesWriter.Setup(i => i.Write(new byte[] {0, 0, 0, 0, 0, 0, 0, 4})).Throws<Exception>();
            writer.Write("Hi");
            writer.Write("Abc");

            // Then
            _indicesWriter.Verify(i => i.Write(It.IsAny<byte[]>()), Times.Exactly(1));
        }

        private MessageWriter CreateInstance() =>
            new MessageWriter(_options.Object, _indicesWriter.Object, _messagesWriter.Object);
    }
}