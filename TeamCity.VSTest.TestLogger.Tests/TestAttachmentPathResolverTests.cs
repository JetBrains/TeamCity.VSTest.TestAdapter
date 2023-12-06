namespace TeamCity.VSTest.TestLogger.Tests
{
    using Shouldly;
    using TestLogger;
    using Xunit;

    public class TestAttachmentPathResolverTests
    {
        private readonly ITestAttachmentPathResolver _testAttachmentPathResolver = new TestAttachmentPathResolver();

        [Theory]
        [InlineData("", "")]
        [InlineData("TestName", "TestName")]
        [InlineData("Namespace1.Namespace22.Namespace333.TestName.", "Namespace1/Namespace22/Namespace333/TestName")]
        [InlineData("..Namespace1.Namespace22......Namespace333.TestName...", "Namespace1/Namespace22/Namespace333/TestName")]
        [InlineData("Namespace1002eNamespace22002eNamespace333002eClassName002eTestName", "Namespace1/Namespace22/Namespace333/ClassName/TestName")]
        [InlineData(
            "Namespace.ClassName.ThisIsTestWithNameLongerThanDirectoryMaxAllowedLength12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890", 
            "Namespace/ClassName/ThisIsTestWithNameLongerThanDirectoryMaxAllowedLength123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567/89012345678901234567890"
            )]
        [InlineData(
            "Namespace002eClassName002eThisIsTestWithNameLongerThanDirectoryMaxAllowedLengthAndContainsNonStandardSymbolsABCDEFGHIJK005fABCDEFGHIJK0024ABCDEFGHIJK0029ABCDEFGHIJK0028ABCDEFGHIJK002dABCDEFGHIJK002b1234567890003d000000000000261234567890005f005f005f1234567890002412345678900060123456789000271234567890002212345678901234567",
            "Namespace/ClassName/ThisIsTestWithNameLongerThanDirectoryMaxAllowedLengthAndContainsNonStandardSymbolsABCDEFGHIJK005fABCDEFGHIJK0024ABCDEFGHIJK0029ABCDEFGHIJK0028ABCDEFGHIJK002dABCDEFGHIJK002b1234567890003d00000000000026/1234567890005f005f005f1234567890002412345678900060123456789000271234567890002212345678901234567"
        )]
        [InlineData(
            "SuperProject1.SuperProject2.SuperProject3.SuperProject4.SuperProject5.SuperProject6.SuperProject7.SuperProject8.SuperProject9.SuperProject10.SuperProject11.SuperProject12.SuperProject13.SuperProject14.SuperProject15.SuperProject16.SuperProject17.SuperProject18.SuperProject19.SuperProject20.MySuperClass.TestName",
            "SuperProject1/SuperProject2/SuperProject3/SuperProject4/SuperProject5/SuperProject6/SuperProject7/SuperProject8/SuperProject9/SuperProject10/SuperProject11/SuperProject12/SuperProject13/SuperProject14/SuperProject15/SuperProject16/SuperProject17/SuperProject18/SuperProject19/SuperProject20/MySuperClass/TestName"
            )]
        [InlineData(
            "Namespace1.ThisIsNamespaceWithLengthLongerThatMaxAllowedDirectoryLength123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890.ClassName.MyTestName",
            "Namespace1/ThisIsNamespaceWithLengthLongerThatMaxAllowedDirectoryLength12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890/1234567890/ClassName/MyTestName"
            )]
        [InlineData(
            "AAASuperTesting.SimpleWordWord: AAASuperTesting.SimpleWordWord.Features.Portrait.Portrait_WirclichFeature.DEaglePure_QWERTY_31777_VerifyTestWillPassButTheThingIsThatThisTestHasAVeryLongNameAndSomeAnotherVerificationToMakeTheNameEvenLonger",
            "AAASuperTesting/SimpleWordWord: AAASuperTesting/SimpleWordWord/Features/Portrait/Portrait_WirclichFeature/DEaglePure_QWERTY_31777_VerifyTestWillPassButTheThingIsThatThisTestHasAVeryLongNameAndSomeAnotherVerificationToMakeTheNameEvenLonger"
            )]
        public void ShouldComposeTestDirectoryAsExpected(string testName, string expected)
        {
            // Act
            var result = _testAttachmentPathResolver.Resolve(testName);

            // Assert
            result.ShouldBe(expected);
        }
    }
}