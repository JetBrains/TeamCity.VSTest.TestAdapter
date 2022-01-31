namespace TeamCity.VSTest.TestLogger
{
    using System;
    using System.Linq;
    using JetBrains.TeamCity.ServiceMessages;
    using JetBrains.TeamCity.ServiceMessages.Write;

    internal class PatchedServiceMessage : ServiceMessage
    {
        public PatchedServiceMessage([NotNull] IServiceMessage message)
            : base(message.Name)
        {
            if (message == null) throw new ArgumentNullException(nameof (message));
            AddRange(message.Keys.ToDictionary(x => x, message.GetValue));
        }
    }
}