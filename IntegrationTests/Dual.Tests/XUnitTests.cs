using System.Diagnostics.CodeAnalysis;
using Xunit;

// ReSharper disable CheckNamespace

namespace DualUnitTest
{
    [SuppressMessage("Assertions", "xUnit2020:Do not use always-failing boolean assertions")]
    public class XUnitTests
    {
        [Fact]
        public void XUnitTest1()
        {
            Assert.True(false, "error details");
        }

        [Fact(Skip = "skip reason")]
        public void TestIgnored()
        {
        }
    }
}