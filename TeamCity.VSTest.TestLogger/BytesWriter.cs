namespace TeamCity.VSTest.TestLogger;

using System.IO;

internal class BytesWriter(string fileName) : IBytesWriter
{
    private FileStream? _stream;
    private BinaryWriter? _writer;

    public void Write(byte[] bytes)
    {
        EnsureOpened();
        _writer!.Write(bytes);
    }

    public void Flush()
    {
        _writer?.Flush();
    }

    public void Dispose()
    {
        _writer?.Flush();
        _stream?.Dispose();
    }

    private void EnsureOpened()
    {
        if (_stream != default)
        {
            return;
        }

        _stream = File.Open(fileName, FileMode.CreateNew, FileAccess.Write, FileShare.Read);
        _writer = new BinaryWriter(_stream);
    }
}