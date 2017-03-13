namespace TeamCity.VSTest.TestAdapter.Tests.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using JetBrains.TeamCity.ServiceMessages;
    using JetBrains.TeamCity.ServiceMessages.Read;
    using NUnit.Framework;

    internal static class ServiceMessages
    {
        private static readonly IServiceMessageParser Parser = new ServiceMessageParser();

        public static int GetNumberServiceMessage([NotNull] string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            var actualMessages = Parser.ParseServiceMessages(text).ToList();
            return actualMessages.Count;
        }

        public static void ResultShouldContainCorrectStructureAndSequence([NotNull] string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            var messages = Parser.ParseServiceMessages(text).ToList();
            var rootFlows = new List<Flow>();
            foreach (var serviceMessage in messages)
            {
                var message = new Message(serviceMessage);
                var flow = rootFlows.SingleOrDefault(i => i.FlowId == message.CurrentFlowId);
                if (flow == null)
                {
                    flow = new Flow(message.FlowIdAttr);
                    rootFlows.Add(flow);
                }

                flow.ProcessMessage(message);

                if (flow.IsFinished)
                    rootFlows.Remove(flow);
            }
        }

        private class Message
        {
            public Message(IServiceMessage message)
            {
                if (message == null) throw new ArgumentNullException("message");

                Name = message.Name;

                FlowIdAttr = message.GetValue("flowId");
                Assert.IsNotEmpty(FlowIdAttr);

                NameAttr = message.GetValue("name");
                ParentAttr = message.GetValue("parent");
                CaptureStandardOutputAttr = message.GetValue("captureStandardOutput");
                DurationAttr = message.GetValue("duration");
                OutAttr = message.GetValue("duration");
                MessageAttr = message.GetValue("message");
                DetailsAttr = message.GetValue("details");
                TcTagsAttr = message.GetValue("tc:tags");
            }

            public string Name { get; }

            public string FlowIdAttr { get; }

            public string NameAttr { get; }

            public string ParentAttr { get; }

            public string CurrentFlowId => ParentAttr ?? FlowIdAttr;

            public string CaptureStandardOutputAttr { get; }

            public string DurationAttr { get; }

            public string OutAttr { get; }

            public string MessageAttr { get; }

            public string DetailsAttr { get; }

            public string TcTagsAttr { get; }
        }

        private class Flow
        {
            private readonly Stack<Message> _messages = new Stack<Message>();

            public Flow(string flowId)
            {
                FlowId = flowId;
            }

            public string FlowId { get; private set; }

            public bool IsFinished => _messages.Count == 0;

            public void ProcessMessage(Message message)
            {
                switch (message.Name)
                {
                    case "testSuiteStarted":
                        Assert.AreEqual(_messages.Count, 0, "testSuiteStarted should be a first message");
                        Assert.IsNotEmpty(message.FlowIdAttr, "FlowId attribute is empty");
                        Assert.IsNotEmpty(message.NameAttr, "Name attribute is empty");
                        FlowId = message.FlowIdAttr;
                        _messages.Push(message);
                        break;

                    case "testSuiteFinished":
                        Assert.AreEqual(_messages.Count, 1, "testSuiteFinished should close testSuiteStarted");
                        var testSuiteStarted = _messages.Pop();
                        Assert.AreEqual(testSuiteStarted.Name, "testSuiteStarted", "testSuiteFinished should close testSuiteStarted");
                        Assert.AreEqual(testSuiteStarted.FlowIdAttr, message.FlowIdAttr, "Invalid FlowId attribute");
                        Assert.AreEqual(testSuiteStarted.NameAttr, message.NameAttr, "Invalid Name attribute");
                        break;

                    case "flowStarted":
                        Assert.IsNotEmpty(message.FlowIdAttr, "Invalid FlowId attribute");
                        Assert.AreEqual(message.ParentAttr, FlowId, "Invalid Parent attribute");
                        FlowId = message.FlowIdAttr;
                        _messages.Push(message);
                        break;

                    case "flowFinished":
                        Assert.AreEqual(message.FlowIdAttr, FlowId, "Invalid FlowId attribute");
                        Assert.Greater(_messages.Count, 1, "flowFinished should close flowStarted");
                        var flowStarted = _messages.Pop();
                        Assert.AreEqual(flowStarted.Name, "flowStarted", "flowFinished should close flowStarted");
                        FlowId = flowStarted.ParentAttr;
                        break;

                    case "testStarted":
                        Assert.AreEqual(message.FlowIdAttr, FlowId, "Invalid FlowId attribute");
                        Assert.IsNotEmpty(message.NameAttr, "Name attribute is empty");
                        if (message.CaptureStandardOutputAttr != null)
                            Assert.AreEqual(message.CaptureStandardOutputAttr, "false", "Invalid CaptureStandardOutput attribute");

                        _messages.Push(message);
                        break;

                    case "testFinished":
                        Assert.AreEqual(message.FlowIdAttr, FlowId, "Invalid FlowId attribute");
                        Assert.IsNotEmpty(message.NameAttr, "Name attribute is empty");
                        Assert.Greater(_messages.Count, 1, "testFinished should close testStarted");
                        var testStarted = _messages.Pop();
                        Assert.AreEqual(testStarted.Name, "testStarted", "testFinished should close testStarted");
                        Assert.AreEqual(testStarted.NameAttr, message.NameAttr, "Invalid Name attribute");
                        Assert.IsNotEmpty(message.DurationAttr, "Duration attribute is empty");
                        break;

                    case "testStdOut":
                        Assert.AreEqual(message.FlowIdAttr, FlowId, "Invalid FlowId attribute");
                        Assert.IsNotEmpty(message.NameAttr, "Name attribute is empty");
                        Assert.Greater(_messages.Count, 1, "testStdOut should be within testStarted and testFinished");
                        var testStartedForStdOut = _messages.Peek();
                        Assert.AreEqual(testStartedForStdOut.Name, "testStarted", "testStdOut should be within testStarted and testFinished");
                        Assert.AreEqual(testStartedForStdOut.NameAttr, message.NameAttr, "Invalid Name attribute");
                        Assert.IsNotEmpty(message.OutAttr, "Out attribute is empty");
                        Assert.IsNotEmpty(message.TcTagsAttr, "tc:tags should be tc:parseServiceMessagesInside");
                        break;

                    case "testFailed":
                        Assert.AreEqual(message.FlowIdAttr, FlowId, "Invalid FlowId attribute");
                        Assert.IsNotEmpty(message.NameAttr, "Name attribute is empty");
                        Assert.Greater(_messages.Count, 1, "testFailed should be within testStarted and testFinished");
                        var testStartedForTestFailed = _messages.Peek();
                        Assert.AreEqual(testStartedForTestFailed.Name, "testStarted", "testFailed should be within testStarted and testFinished");
                        Assert.AreEqual(testStartedForTestFailed.NameAttr, message.NameAttr, "Invalid Name attribute");
                        Assert.IsNotEmpty(message.MessageAttr, "Message attribute is empty");
                        Assert.IsNotNull(message.DetailsAttr, "Details attribute is empty");
                        break;

                    case "testIgnored":
                        Assert.AreEqual(message.FlowIdAttr, FlowId, "Invalid FlowId attribute");
                        Assert.IsNotEmpty(message.NameAttr, "Name attribute is empty");
                        Assert.Greater(_messages.Count, 1, "testIgnored should be within testStarted and testFinished");
                        var testStartedForTestIgnored = _messages.Peek();
                        Assert.AreEqual(testStartedForTestIgnored.Name, "testStarted", "testIgnored should be within testStarted and testFinished");
                        Assert.AreEqual(testStartedForTestIgnored.NameAttr, message.NameAttr, "Invalid Name attribute");
                        Assert.IsNotEmpty(message.MessageAttr, "Message attribute is empty");
                        break;

                    default:
                        Assert.Fail($"Unexpected message {message.Name}");
                        break;
                }
            }
        }
    }
}