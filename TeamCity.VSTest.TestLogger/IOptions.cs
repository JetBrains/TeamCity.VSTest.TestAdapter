namespace TeamCity.VSTest.TestLogger
{
    internal interface IOptions
    {
        [CanBeNull] string TestRunDirectory { get; set; }
    }
}