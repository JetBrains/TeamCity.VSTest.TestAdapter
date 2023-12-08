using System.Diagnostics.CodeAnalysis;
// ReSharper disable Xunit.XunitTestWithConsoleOutput
// ReSharper disable CheckNamespace

namespace dotNetCore.XUnit.Tests
{
    using System;
    using Xunit;

    [SuppressMessage("Assertions", "xUnit2020:Do not use always-failing boolean assertions")]
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