namespace TeamCity.VSTest.TestLogger.Tests
{
    using Shouldly;
    using Xunit;

    public class TestNameProviderTests
    {
        [Theory]
        // NUnit
        [InlineData("NUnit.Tests.Test1", "Test1", "NUnit.Tests.Test1")]
        // NUnit with parameters
        [InlineData("NUnit.Tests.Test2(\"Aa\", \"Bb\")", "Test2(\"Aa\", \"Bb\")", "NUnit.Tests.Test2(\"Aa\", \"Bb\")")]
        [InlineData("NUnit.Tests.Test2(\"A.a\", \"Bb\")", "Test2(\"A.a\", \"Bb\")", "NUnit.Tests.Test2(\"A.a\", \"Bb\")")]
        // NUnit with parameters and short name
        [InlineData("A.Test2(\"A.a\", \"Bb\")", "Test2(\"A.a\", \"Bb\")", "A.Test2(\"A.a\", \"Bb\")")]

        // XUnit
        [InlineData("XUnit.Tests.Test1", "XUnit.Tests.Test1", "XUnit.Tests.Test1")]
        // XUnit with type args
        [InlineData("XUnit.Tests.Test1", "XUnit.Tests.Test1<int>", "XUnit.Tests.Test1<int>")]
        // XUnit [Fact(DisplayName = "Abc")]
        [InlineData("XUnit.Tests.Test1", "Abc", "XUnit.Tests.Test1")]
        // XUnit [Fact(DisplayName = "Abc")] with type args
        [InlineData("XUnit.Tests.Test1", "Abc<int, String>", "XUnit.Tests.Test1<int, String>")]
        // XUnit with parameters
        [InlineData("XUnit.Tests.Test2", "XUnit.Tests.Test2 (str1: \"Aaa\", str2: \"Bb\")", "XUnit.Tests.Test2(str1: \"Aaa\", str2: \"Bb\")")]
        [InlineData("XUnit.Tests.Test2", "XUnit.Tests.Test2(str1: \"Aaa<string, Int>\", str2: \"Bb...", "XUnit.Tests.Test2(str1: \"Aaa<string, Int>\", str2: \"Bb...)")]
        // XUnit with parameters with type args
        [InlineData("XUnit.Tests.Test2", "XUnit.Tests.Test2<int, String>(str1: \"Aaa\", str2: \"Bb\")", "XUnit.Tests.Test2<int, String>(str1: \"Aaa\", str2: \"Bb\")")]
        // XUnit with long parameters
        [InlineData("XUnit.Tests.Test2", "XUnit.Tests.Test2 (str1: \"Aaa\", str2: \"Bb...", "XUnit.Tests.Test2(str1: \"Aaa\", str2: \"Bb...)")]
        // XUnit with long parameters with type args
        [InlineData("XUnit.Tests.Test2", "XUnit.Tests.Test2<string, Int>(str1: \"Aaa\", str2: \"Bb...", "XUnit.Tests.Test2<string, Int>(str1: \"Aaa\", str2: \"Bb...)")]
        // [Theory(DisplayName = "Abc")]
        [InlineData("XUnit.Tests.Test2", "Abc (str1: \"Aaa\", str2: \"Bb\")", "XUnit.Tests.Test2(str1: \"Aaa\", str2: \"Bb\")")]
        [InlineData("XUnit.Tests.Test2", "Abc (str1: \"Aaa\", str2: \"Bb\")  ", "XUnit.Tests.Test2(str1: \"Aaa\", str2: \"Bb\")")]
        [InlineData("XUnit.Tests.Test2", "Abc ()", "XUnit.Tests.Test2()")]

        // MSTest
        [InlineData("MSTest.Tests.Test1", "Test1", "MSTest.Tests.Test1")]
        // MSTest [TestMethod("Abc")]
        [InlineData("MSTest.Tests.Test1", "Abc", "MSTest.Tests.Test1")]
        // MSTest with parameters
        [InlineData("MSTest.Tests.Test2", "Test2(Aa,Bb)", "MSTest.Tests.Test2(Aa,Bb)")]
        [InlineData("MSTest.Tests.Test2", "Test2 (Aa,Bb)", "MSTest.Tests.Test2(Aa,Bb)")]
        [InlineData("MSTest.Tests.Test2", "Test2 ()", "MSTest.Tests.Test2()")]
        [InlineData("MSTest.Tests.Test2", "Test3 (Aa,Bb)", "MSTest.Tests.Test2(Aa,Bb)")]
        [InlineData("MSTest.Tests.Test2", "Test2 (Aa,", "MSTest.Tests.Test2(Aa,)")]
        [InlineData("A.Test2", "Test2 (Aa,Bb)", "A.Test2(Aa,Bb)")]
        [InlineData("A...Test2.", "Test2 (Aa,Bb)", "A...Test2.(Aa,Bb)")]

        [InlineData("Abc", null, "Abc")]
        [InlineData("Abc", "", "Abc")]
        [InlineData("Abc", "        ", "Abc")]
        [InlineData(null, "Abc", "Abc")]
        [InlineData("", "Abc", "Abc")]
        [InlineData("        ", "Abc", "Abc")]
        [InlineData(null, null, "")]
        public void ShouldProvideName(string fullyQualifiedName, string displayName, string expected)
        {
            // Given
            var nameFactory = CreateInstance();

            // When
            var actual = nameFactory.GetTestName(fullyQualifiedName, displayName);

            // Then
            actual.ShouldBe(expected);
        }

        private TestNameProvider CreateInstance() =>
            new TestNameProvider();
    }
}