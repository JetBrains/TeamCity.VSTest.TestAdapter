namespace TeamCity.VSTest.TestLogger;

internal interface IBytesWriterFactory
{
    IBytesWriter Create(string fileName);
}