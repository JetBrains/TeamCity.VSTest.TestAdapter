namespace TeamCity.VSTest.TestLogger;

using System;
using JetBrains.TeamCity.ServiceMessages;
using JetBrains.TeamCity.ServiceMessages.Write.Special;

internal class TestInfoUpdater(IEventContext eventContext) : IServiceMessageUpdater
{
    public IServiceMessage UpdateServiceMessage(IServiceMessage message)
    {
        if (message == null) throw new ArgumentNullException(nameof(message));
        if (!eventContext.TryGetEvent(out var testEvent))
        {
            return message;
        }
            
        var newMessage = new PatchedServiceMessage(message);
        if (!string.IsNullOrEmpty(testEvent.SuiteName))
        {
            newMessage.Add("suiteName", testEvent.SuiteName);
        }
            
        if (!string.IsNullOrEmpty(testEvent.TestCase.Source))
        {
            newMessage.Add("testSource", testEvent.TestCase.Source);
        }
            
        if (!string.IsNullOrEmpty(testEvent.TestCase.DisplayName))
        {
            newMessage.Add("displayName", testEvent.TestCase.DisplayName);
        }
            
        if (!string.IsNullOrEmpty(testEvent.DisplayName))
        {
            newMessage.Add("resultDisplayName", testEvent.DisplayName);
        }
            
        if (!string.IsNullOrEmpty(testEvent.TestCase.CodeFilePath))
        {
            newMessage.Add("codeFilePath", testEvent.TestCase.CodeFilePath);
        }
            
        if (!string.IsNullOrEmpty(testEvent.TestCase.FullyQualifiedName))
        {
            newMessage.Add("fullyQualifiedName", testEvent.TestCase.FullyQualifiedName);
        }

        newMessage.Add("id", testEvent.TestCase.Id.ToString());
            
        if (testEvent.TestCase.ExecutorUri != default)
        {
            newMessage.Add("executorUri", testEvent.TestCase.ExecutorUri.ToString());
        }
            
        newMessage.Add("lineNumber", testEvent.TestCase.LineNumber.ToString());

        return newMessage;
    }
}