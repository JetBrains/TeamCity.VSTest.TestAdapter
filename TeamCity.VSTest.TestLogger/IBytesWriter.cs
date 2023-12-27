namespace TeamCity.VSTest.TestLogger;

internal interface IBytesWriter
{
    void Write(byte[] bytes);

    void Flush();
}