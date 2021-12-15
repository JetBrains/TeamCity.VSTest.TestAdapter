namespace TeamCity.VSTest.TestLogger
{
    internal interface IOptions
    {
        [CanBeNull] string TestRunDirectory { get; set; }

        TeamCityVersion Version { get; }

        string RootFlowId { get; }
        
        bool AllowServiceMessageBackup { get; }
        
        string ServiceMessagesSource { get; }
        
        string ServiceIndicesFile { get; }

        string ServiceMessagesFile { get; }

        bool AllowExperimental { get; }

        bool MetadataEnable { get; }

        TeamCityVersion TestMetadataSupportVersion { get; }
    }
}