namespace TeamCity.VSTest.TestLogger.Tests;

using System;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Moq;
using Xunit;

public class FailedTestsReportWriterTests
{
    private readonly Mock<IOptions> _optionsMock = new();
    private readonly Mock<IBytesWriterFactory> _bytesWriterFactoryMock = new();
    private readonly Mock<IBytesWriter> _bytesWriterMock = new();

    public FailedTestsReportWriterTests()
    {
        _bytesWriterFactoryMock
            .Setup(x => x.Create(It.IsAny<string>()))
            .Returns(_bytesWriterMock.Object);
    }

    [Fact]
    public void ShouldNotReportWhenDisabled()
    {
        // Arrange
        _optionsMock
            .SetupGet(x => x.FailedTestsReportSavePath)
            .Returns(string.Empty);
        var writer = new FailedTestsReportWriter(_optionsMock.Object, _bytesWriterFactoryMock.Object);
        
        // Act
        writer.ReportFailedTest(CreateTestCase("Test"));
        
        // Assert
        _bytesWriterFactoryMock.Verify(x => x.Create(It.IsAny<string>()), Times.Never);
    }
    
    [Fact]
    public void ShouldNotReportSameParameterizedTestTwice()
    {
        // Arrange
        _optionsMock
            .SetupGet(x => x.FailedTestsReportSavePath)
            .Returns("path-to-report");
        var writer = new FailedTestsReportWriter(_optionsMock.Object, _bytesWriterFactoryMock.Object);
        
        // Act
        writer.ReportFailedTest(CreateTestCase("SameTest(1)"));
        writer.ReportFailedTest(CreateTestCase("SameTest(2)"));
        
        // Assert
        var expected = Encoding.UTF8.GetBytes("SameTest" + Environment.NewLine);
        _bytesWriterMock.Verify(x => x.Write(expected), Times.Once);
        _bytesWriterMock.Verify(x => x.Flush(), Times.Once);
        _bytesWriterMock.VerifyNoOtherCalls();
    }
    
    [Fact]
    public void ShouldRemoveTypeParameters()
    {
        // Arrange
        _optionsMock
            .SetupGet(x => x.FailedTestsReportSavePath)
            .Returns("path-to-report");
        var writer = new FailedTestsReportWriter(_optionsMock.Object, _bytesWriterFactoryMock.Object);
        
        // Act
        writer.ReportFailedTest(CreateTestCase("ParametrizedTest<Double,String>(1)"));
        
        // Assert
        var expected = Encoding.UTF8.GetBytes("ParametrizedTest" + Environment.NewLine);
        _bytesWriterMock.Verify(x => x.Write(expected), Times.Once);
        _bytesWriterMock.Verify(x => x.Flush(), Times.Once);
        _bytesWriterMock.VerifyNoOtherCalls();
    }

    private static TestCase CreateTestCase(string testName) => new(testName, new Uri("executor://NUnit3TestExecutor"), "Tests");
}