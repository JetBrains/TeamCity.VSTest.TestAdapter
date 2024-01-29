namespace TeamCity.VSTest.TestLogger;

internal class BytesWriterFactory : IBytesWriterFactory
{
    public IBytesWriter Create(string fileName) => new BytesWriter(fileName);
}