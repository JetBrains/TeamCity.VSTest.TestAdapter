namespace dotNetCore.XUnit.Tests
{
    using Xunit;

    public class UnitTests
    {
        [Fact]
        public void TestPassed()
        {
            System.Console.WriteLine("some text");
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
