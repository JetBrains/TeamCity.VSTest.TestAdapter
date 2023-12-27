namespace TeamCity.VSTest.TestLogger.Tests;

using IoC;
using JetBrains.TeamCity.ServiceMessages.Write.Special;
using Xunit;

public class BootstrappTests
{
    [Fact]
    public void ShouldResolveTeamCityWriter()
    {
            using var container = Container.Create().Using<IoCConfiguration>();
            var writer = container.Resolve<ITeamCityWriter>();
            writer.WriteMessage("abc");
        }
}