namespace TeamCity.VSTest.TestAdapter
{
    internal interface IEnvironmentInfo
    {
        bool IsUnderTeamCity { get; }
    }
}