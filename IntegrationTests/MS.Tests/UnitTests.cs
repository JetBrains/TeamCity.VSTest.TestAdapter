namespace MS.Tests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class UnitTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void TestFailed()
        {
            Console.WriteLine("some error stdOut text");
            Console.Error.WriteLine("some error stdErr text");
            Assert.IsTrue(false, "error details");
        }

        [TestMethod, Ignore]
        public void TestIgnored()
        {
        }

        [TestMethod]
        public void TestPassed()
        {
            Console.WriteLine("some stdOut text");
            Console.Error.WriteLine("some stdErr text");
            TestContext.WriteLine("message from TestContext");
        }
    }
}
