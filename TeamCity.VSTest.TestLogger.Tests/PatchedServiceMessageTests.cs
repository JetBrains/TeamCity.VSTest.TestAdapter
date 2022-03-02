namespace TeamCity.VSTest.TestLogger.Tests
{
    using System.Linq;
    using JetBrains.TeamCity.ServiceMessages;
    using JetBrains.TeamCity.ServiceMessages.Write;
    using Moq;
    using Shouldly;
    using Xunit;

    public class PatchedServiceMessageTests
    {
        [Fact]
        public void ShouldUpdateValue()
        {
            // Given
            var baseMessage = new ServiceMessage("Abc")
            {
                { "Name1", "Val1" }
            };

            var patchedMessage = CreateInstance(baseMessage);
            
            // When
            patchedMessage.Add("Name1", "Val 1");

            // Then
            patchedMessage.GetValue("Name1").ShouldBe("Val 1");
            patchedMessage.Name.ShouldBe("Abc");
            patchedMessage.Keys.Count().ShouldBe(1);
            patchedMessage.Keys.ShouldContain("Name1");
        }
        
        [Fact]
        public void ShouldAddValue()
        {
            // Given
            var baseMessage = new ServiceMessage("Abc")
            {
                { "Name1", "Val1" }
            };

            var patchedMessage = CreateInstance(baseMessage);
            
            // When
            patchedMessage.Add("Name2", "Val 2");

            // Then
            patchedMessage.GetValue("Name1").ShouldBe("Val1");
            patchedMessage.GetValue("Name2").ShouldBe("Val 2");
            patchedMessage.Name.ShouldBe("Abc");
            patchedMessage.Keys.Count().ShouldBe(2);
            patchedMessage.Keys.ShouldContain("Name1");
            patchedMessage.Keys.ShouldContain("Name2");
        }
        
        [Fact]
        public void ShouldCopyDefaultValue()
        {
            // Given
            var baseMessage = new Mock<IServiceMessage>();
            var patchedMessage = CreateInstance(baseMessage.Object);
            baseMessage.SetupGet(i => i.Keys).Returns(Enumerable.Empty<string>());
            
            // When
            baseMessage.SetupGet(i => i.DefaultValue).Returns("Default Val");

            // Then
            patchedMessage.DefaultValue.ShouldBe("Default Val");
        }

        private static PatchedServiceMessage CreateInstance(IServiceMessage baseMessage) =>
            new PatchedServiceMessage(baseMessage);
    }
}