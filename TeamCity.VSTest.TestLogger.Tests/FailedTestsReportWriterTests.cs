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
        _optionsMock
            .SetupGet(x => x.Version)
            .Returns(new TeamCityVersion("2024.03"));
        var writer = new FailedTestsReportWriter(_optionsMock.Object, _bytesWriterFactoryMock.Object);

        // Act
        writer.ReportFailedTest(CreateTestCase("Test"));

        // Assert
        _bytesWriterFactoryMock.Verify(x => x.Create(It.IsAny<string>()), Times.Never);
    }

    [Theory]
    [InlineData("2024.12.1", new[] { "SameTest(1)", "SameTest(2)" })]
    [InlineData("2024.12", new[] { "SameTest" })]
    public void ShouldStripTestNameParametersWhenTeamCityVersionIsLessThan2024_12_1(string teamcityVersion, string[] expectedTestNames)
    {
        // Arrange
        _optionsMock
            .SetupGet(x => x.FailedTestsReportSavePath)
            .Returns("path-to-report");
        _optionsMock
            .SetupGet(x => x.Version)
            .Returns(new TeamCityVersion(teamcityVersion));
        var writer = new FailedTestsReportWriter(_optionsMock.Object, _bytesWriterFactoryMock.Object);

        // Act
        writer.ReportFailedTest(CreateTestCase("SameTest(1)"));
        writer.ReportFailedTest(CreateTestCase("SameTest(2)"));

        // Assert
        foreach (var expectedTestName in expectedTestNames)
        {
            var expected = Encoding.UTF8.GetBytes(expectedTestName + Environment.NewLine);
            _bytesWriterMock.Verify(x => x.Write(expected), Times.Once);
        }
        _bytesWriterMock.Verify(x => x.Flush(), Times.Exactly(expectedTestNames.Length));
        _bytesWriterMock.VerifyNoOtherCalls();
    }

    private static TestCase CreateTestCase(string testName) => new(testName, new Uri("executor://NUnit3TestExecutor"), "Tests");
}