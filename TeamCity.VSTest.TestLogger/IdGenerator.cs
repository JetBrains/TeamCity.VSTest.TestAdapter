// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.VSTest.TestLogger
{
    using System;

    internal class IdGenerator : IIdGenerator
    {
        public string NewId() => Guid.NewGuid().ToString().Substring(0, 8);
    }
}
