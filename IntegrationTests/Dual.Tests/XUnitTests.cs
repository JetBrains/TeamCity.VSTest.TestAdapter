using System;
using Xunit;

namespace DualUnitTest
{
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