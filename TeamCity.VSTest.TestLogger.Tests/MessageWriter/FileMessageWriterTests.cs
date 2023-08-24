namespace TeamCity.VSTest.TestLogger.Tests.MessageWriter
{
    using System;
    using System.Linq;
    using System.Text;
    using MessageWriters;
    using Moq;
    using Xunit;

    public class FileMessageWriterTests
    {
        [Fact]
        public void ShouldAddNewLineAndWriteBytesForUtf8Encoding()
        {
            // Given
            var bytesWriterMock = new Mock<IBytesWriter>();
            var writer = new FileMessageWriter(bytesWriterMock.Object);
            var message = "msg";
            var expectedBytes = Encoding.UTF8.GetBytes(message + Environment.NewLine);

            // When
            writer.Write(message);

            // Then
            bytesWriterMock.Verify(x => x.Write(It.Is<byte[]>(b => b.SequenceEqual(expectedBytes))));
        }
    }
}
