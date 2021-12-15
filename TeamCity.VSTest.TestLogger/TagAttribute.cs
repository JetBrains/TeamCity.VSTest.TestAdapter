namespace TeamCity.VSTest.TestLogger
{
    using System;

    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method)]
    public class TagAttribute : Attribute
    {
        // A tag, which will be used during an injection
        public readonly object Tag;

        public TagAttribute(object tag) => Tag = tag;
    }
}