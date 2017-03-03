namespace TeamCity.VSTestAdapter
{
    using System;
    using JetBrains.TeamCity.ServiceMessages.Write.Special;

    internal class Factory
    {
        public static ITeamCityWriter CreateTeamCityWriter()
        {
            return new TeamCityServiceMessages().CreateWriter(Console.WriteLine);
        }
    }
}
