namespace TeamCity.VSTest.TestLogger
{
    internal interface IOptions
    {
        [CanBeNull] string TestRunDirectory { get; set; }

        TeamCityVersion Version { get; }

        string RootFlowId { get; }

        bool AllowExperimental { get; }

        TeamCityVersion TestMetadataSupportVersion { get; }
    }
}