namespace dotNetCore.MS.Tests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class UnitTests
    {
        [TestMethod]
        public void TestFailed()
        {
            Assert.IsTrue(false, "error details");
        }

        [TestMethod]
        public void TestIgnored()
        {
        }

        [TestMethod]
        public void TestPassed()
        {
            Console.WriteLine("some text");
        }
    }
}
