namespace TeamCity.VSTest.TestLogger
{
    internal interface IOptions
    {
        string? TestRunDirectory { get; set; }

        TeamCityVersion Version { get; }

        string RootFlowId { get; }

        public string ServiceMessagesFileSavePath { get; }

        bool FallbackToStdOutTestReporting { get; }
        
        bool AllowServiceMessageBackup { get; }
        
        string ServiceMessagesBackupPath { get; }

        string ServiceMessagesBackupSource { get; }

        bool AllowExperimental { get; }

        bool MetadataEnable { get; }

        TeamCityVersion TestMetadataSupportVersion { get; }
    }
}
