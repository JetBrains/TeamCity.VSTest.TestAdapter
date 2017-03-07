namespace dotNetCore.XUnit.Tests
{
    using Xunit;

    public class UnitTests
    {
        [Fact]
        public void TestPassed()
        {
        }

        [Fact]
        public void TestFailed()
        {
            Assert.True(false, "error details");
        }

        [Fact(Skip = "skip reason")]
        public void TestIgnored()
        {
        }
    }
}
