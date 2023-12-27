namespace TeamCity.VSTest.TestLogger
{
    using System;
    using System.IO;

    internal class BytesWriter : IBytesWriter, IDisposable
    {
        private readonly string _fileName;
        private FileStream? _stream;
        private BinaryWriter? _writer;

        public BytesWriter(string fileName) => _fileName = fileName;

        public void Write(byte[] bytes)
        {
            EnsureOpened();
            _writer!.Write(bytes);
        }

        public void Flush()
        {
            EnsureOpened();
            _writer!.Flush();
        }

        public void Dispose() => _stream?.Dispose();

        private void EnsureOpened()
        {
            if (_stream != default)
            {
                return;
            }

            _stream = File.Open(_fileName, FileMode.CreateNew, FileAccess.Write, FileShare.Read);
            _writer = new BinaryWriter(_stream);
        }
    }
}