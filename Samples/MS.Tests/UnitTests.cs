namespace MS.Tests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class UnitTests
    {
        [TestMethod, Ignore]
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
