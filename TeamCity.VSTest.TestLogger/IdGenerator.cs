// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.VSTest.TestLogger
{
    using System;
    using System.Linq;

    internal class IdGenerator : IIdGenerator
    {
        public string NewId() => new string(Guid.NewGuid().ToString().Where(c => c != '-').ToArray());
    }
}
