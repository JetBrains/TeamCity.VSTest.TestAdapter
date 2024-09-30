// ReSharper disable UnusedMember.Global
namespace TeamCity.VSTest.TestLogger;

internal interface IOptions
{
    string? TestRunDirectory { get; set; }

    TeamCityVersion Version { get; }

    string RootFlowId { get; }

    string ServiceMessagesFileSavePath { get; }
    
    string FailedTestsReportSavePath { get; }

    bool FallbackToStdOutTestReporting { get; }
        
    bool AllowServiceMessageBackup { get; }
        
    string ServiceMessagesBackupPath { get; }

    string ServiceMessagesBackupSource { get; }

    bool AllowExperimental { get; }

    bool MetadataEnable { get; }

    TeamCityVersion TestMetadataSupportVersion { get; }
    
    public bool UseTestResultDisplayName { get; }
}