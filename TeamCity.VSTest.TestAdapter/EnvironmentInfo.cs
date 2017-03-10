namespace TeamCity.VSTest.TestAdapter
{
    using System;

    internal class EnvironmentInfo: IEnvironmentInfo
    {
        public EnvironmentInfo()
        {
            var teamCityProjectName = Environment.GetEnvironmentVariable("TEAMCITY_PROJECT_NAME");
            IsUnderTeamCity = !string.IsNullOrEmpty(teamCityProjectName);
        }

        public bool IsUnderTeamCity { get; }
    }
}
