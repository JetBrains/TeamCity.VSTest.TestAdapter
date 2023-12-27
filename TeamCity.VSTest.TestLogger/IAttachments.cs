namespace TeamCity.VSTest.TestLogger
{
    using JetBrains.TeamCity.ServiceMessages.Write.Special;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;

    internal interface IAttachments
    {
        void SendAttachment(string testName, UriDataAttachment attachment, ITeamCityTestWriter testWriter);
    }
}