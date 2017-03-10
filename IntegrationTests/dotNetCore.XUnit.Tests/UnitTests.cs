namespace dotNetCore.XUnit.Tests
{
    using System;
    using Xunit;

    public class UnitTests
    {
        [Fact]
        public void TestFailed()
        {
            Assert.True(false, "error details");
        }

        [Fact(Skip = "skip reason")]
        public void TestIgnored()
        {
        }

        [Fact]
        public void TestPassed()
        {
            Console.WriteLine("some text");
        }
    }
}