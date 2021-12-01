namespace TeamCity.VSTest.TestLogger.Tests
{
    using Shouldly;
    using Xunit;

    public class TestNameFactoryTests
    {
        [Theory]
        [InlineData("NUnit.Tests.Test1", "Test1", "NUnit.Tests.Test1")]
        [InlineData("NUnit.Tests.Test2(\"Aa\", \"Bb\")", "Test2(\"Aa\", \"Bb\")", "NUnit.Tests.Test2(\"Aa\", \"Bb\")")]
        [InlineData("NUnit.Tests.Test2(\"A.a\", \"Bb\")", "Test2(\"A.a\", \"Bb\")", "NUnit.Tests.Test2(\"A.a\", \"Bb\")")]
        [InlineData("A.Test2(\"A.a\", \"Bb\")", "Test2(\"A.a\", \"Bb\")", "A.Test2(\"A.a\", \"Bb\")")]
        [InlineData("est2(\"A.a\", \"Bb\")", "Test2(\"A.a\", \"Bb\")", "Test2(\"A.a\", \"Bb\")")]
        
        [InlineData("XUnit.Tests.Test1", "XUnit.Tests.Test1", "XUnit.Tests.Test1")]
        [InlineData("XUnit.Tests.Test2", "XUnit.Tests.Test2(str1: \"Aaa\", str2: \"Bb\")", "XUnit.Tests.Test2(str1: \"Aaa\", str2: \"Bb\")")]
        
        [InlineData("MSTest.Tests.Test1", "Test1", "MSTest.Tests.Test1")]
        [InlineData("MSTest.Tests.Test2", "Test2 (Aa,Bb)", "MSTest.Tests.Test2(Aa,Bb)")]
        [InlineData("MSTest.Tests.Test2", "Test2 ()", "MSTest.Tests.Test2()")]
        [InlineData("MSTest.Tests.Test2", "Test2(Aa,Bb)", "MSTest.Tests.Test2(Aa,Bb)")]
        [InlineData("MSTest.Tests.Test2", "Test3 (Aa,Bb)", "MSTest.Tests.Test2")]
        [InlineData("MSTest.Tests.Test2", "Test2 (Aa,", "MSTest.Tests.Test2(Aa,")]
        [InlineData("A.Test2", "Test2 (Aa,Bb)", "A.Test2(Aa,Bb)")]
        [InlineData("A...Test2.", "Test2 (Aa,Bb)", "Test2 (Aa,Bb)")]

        [InlineData("Abc", null, "Abc")]
        [InlineData("Abc", "", "Abc")]
        [InlineData("Abc", "        ", "Abc")]
        [InlineData(null, "Abc", "Abc")]
        [InlineData("", "Abc", "Abc")]
        [InlineData("        ", "Abc", "Abc")]
        [InlineData(null, null, "")]
        public void Should(string fullyQualifiedName, string displayName, string expected)
        {
            // Given
            var nameFactory = CreateInstance();

            // When
            var actual = nameFactory.Create(fullyQualifiedName, displayName);

            // Then
            actual.ShouldBe(expected);
        }

        private TestNameFactory CreateInstance() =>
            new TestNameFactory();
    }
}