namespace DevTeam.Tests
{
    using System;
    using TestFramework;

    public class UnitTests
    {
        [Test]
        public void TestFailed()
        {
            throw new Exception("error");
        }

        [Test.Ignore("reason")]
        public void TestIgnored()
        {
        }

        [Test]
        public void TestPassed()
        {
            Console.WriteLine("some text");
        }
    }
}
