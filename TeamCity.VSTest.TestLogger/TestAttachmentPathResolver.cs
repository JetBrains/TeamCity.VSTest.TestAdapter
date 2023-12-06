namespace TeamCity.VSTest.TestLogger
{
    using System;
    using System.Text;

    internal class TestAttachmentPathResolver : ITestAttachmentPathResolver
    {
        // different file systems have varying limitations on the maximum length of directory names
        // normally the limit is around 255 characters
        // in order to successfully create a directory and not catch an exception,
        // we control the length of the directory name with this constant
        private const int DirectoryMaxLength = 200;
        private const char NamespaceDelimiter = '.';
        private const char DirectorySeparator = '/';

        public string Resolve(string testFullyQualifiedName)
        {
            var testName = DenormalizeNamespaceDelimiter(testFullyQualifiedName);
            var result = new StringBuilder();
            var position = 0;
            var offset = 0;

            while (position + offset < testName.Length)
            {
                position = ComputePosition(testName, position, offset);
                offset = ComputeOffset(testName, position);

                result.Append(DirectorySeparator);
                result.Append(testName.Substring(position, offset));
            }

            return result.ToString().Trim(DirectorySeparator);
        }

        private static int ComputePosition(string testName, int currentPosition, int currentOffset)
        {
            var position = currentPosition + currentOffset;
            while (position < testName.Length && testName[position] == NamespaceDelimiter)
            {
                position++;
            }

            return position;
        }

        private static int ComputeOffset(string testName, int currentPosition)
        {
            var maxOffset = Math.Min(testName.Length - currentPosition, DirectoryMaxLength);

            for (var i = 0; i < maxOffset; i++)
            {
                if (testName[currentPosition + i] == NamespaceDelimiter)
                {
                    return i;
                }
            }

            return maxOffset;
        }

        private static string DenormalizeNamespaceDelimiter(string testName)
        {
            return testName.Replace($"{(ulong)NamespaceDelimiter:x4}", NamespaceDelimiter.ToString());
        }
    }
}