namespace TeamCity.VSTestAdapter.Tests
{
    using System;
    using JetBrains.TeamCity.ServiceMessages.Write.Special;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
    using Moq;

    using NUnit.Framework;

    using Shouldly;

    [TestFixture]
    public class TeamCityTestLoggerTests
    {
        private Mock<ITeamCityWriter> _rootWriter;
        private Events _events;

        [SetUp]
        public void SetUp()
        {
            _events = new Events();
            _rootWriter = new Mock<ITeamCityWriter>();
        }

        [Test]
        public void ShouldProcessWhenPassedTest()
        {
            // Given
            CreateInstance();
            var suiteWriter = new Mock<ITeamCityTestsSubWriter>();
            var testWriter = new Mock<ITeamCityTestWriter>();
            _rootWriter.Setup(i => i.OpenTestSuite("assembly.dll")).Returns(suiteWriter.Object);
            suiteWriter.Setup(i => i.OpenTest("test1")).Returns(testWriter.Object);

            // When
            _events.SendTestResult(
                new TestResultEventArgs(
                    new TestResult(
                        new TestCase(
                            "test1",
                            new Uri("executor://custom"), "assembly.dll"))
                    {
                        Outcome = TestOutcome.Passed,
                        Duration = TimeSpan.FromSeconds(1)
                    }));

            // Then
            testWriter.Verify(i => i.WriteDuration(TimeSpan.FromSeconds(1)));
            testWriter.Verify(i => i.Dispose());
        }

        private void CreateInstance()
        {
            var logger = new TeamCityTestLogger(_rootWriter.Object);
            logger.Initialize(_events, null);
        }
    }
}
