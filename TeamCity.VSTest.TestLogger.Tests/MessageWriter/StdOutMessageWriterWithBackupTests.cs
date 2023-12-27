namespace TeamCity.VSTest.TestLogger.Tests.MessageWriter;

using System;
using System.Linq;
using System.Text;
using MessageWriters;
using Moq;
using Xunit;

public class StdOutMessageWriterWithBackupTests
{
    private readonly Mock<IOptions> _options = new();
    private readonly Mock<IBytesWriter> _indicesWriter = new();
    private readonly Mock<IBytesWriter> _messagesWriter = new();

    public StdOutMessageWriterWithBackupTests()
    {
        _options.SetupGet(i => i.AllowServiceMessageBackup).Returns(true);
    }

    [Fact]
    public void ShouldWriteIndicesAndMessages()
    {
        // Given
        var writer = CreateInstance();
        const string firstMessage = "Hi";
        const string secondMessage = "Abc";
        var firstMessageBytes = Encoding.UTF8.GetBytes(firstMessage + Environment.NewLine);
        var secondMessageBytes = Encoding.UTF8.GetBytes(secondMessage + Environment.NewLine);
        var firstMessagePositionBytes = BitConverter.GetBytes((ulong)firstMessageBytes.Length).Reverse().ToArray();
        var secondMessagePositionBytes = BitConverter.GetBytes((ulong)(firstMessageBytes.Length + secondMessageBytes.Length)).Reverse().ToArray();

        // When
        writer.Write(firstMessage);
        writer.Write(secondMessage);

        // Then
        // "Hi"
        _indicesWriter.Verify(i => i.Write(firstMessagePositionBytes));
        _messagesWriter.Verify(i => i.Write(firstMessageBytes));
            
        // Abc
        _indicesWriter.Verify(i => i.Write(secondMessagePositionBytes));
        _messagesWriter.Verify(i => i.Write(secondMessageBytes));
    }
        
    [Fact]
    public void ShouldStopWritingWhenExceptionWasRegisteredOnce()
    {
        // Given
        var writer = CreateInstance();

        // When
        _indicesWriter.Setup(i => i.Write(It.IsAny<byte[]>())).Throws<Exception>();
        writer.Write("Hi");
        writer.Write("Abc");

        // Then
        _indicesWriter.Verify(i => i.Write(It.IsAny<byte[]>()), Times.Exactly(1));
    }

    private StdOutMessageWriterWithBackup CreateInstance() => new(_indicesWriter.Object, _messagesWriter.Object);
}