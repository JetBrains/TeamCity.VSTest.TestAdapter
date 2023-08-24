namespace TeamCity.VSTest.TestLogger
{
    internal interface IMessageWriter
    {
        void Write(string message);

        void Flush();
    }
}
