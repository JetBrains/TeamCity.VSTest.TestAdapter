// ReSharper disable MemberCanBePrivate.Local
// ReSharper disable UnusedAutoPropertyAccessor.Local
namespace TeamCity.VSTest.TestLogger.Tests.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using JetBrains.TeamCity.ServiceMessages;
    using JetBrains.TeamCity.ServiceMessages.Read;
    using Shouldly;

    internal static class ServiceMessages
    {
        private static readonly IServiceMessageParser Parser = new ServiceMessageParser();

        public static int GetNumberOfServiceMessages([NotNull] string text)
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
                IsNotEmpty(FlowIdAttr);

                NameAttr = message.GetValue("name");
                ParentAttr = message.GetValue("parent");
                CaptureStandardOutputAttr = message.GetValue("captureStandardOutput");
                DurationAttr = message.GetValue("duration");
                OutAttr = message.GetValue("out");
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
                        AreEqual(_messages.Count, 1, "testSuiteStarted should be a first message");
                        IsNotEmpty(message.FlowIdAttr, "FlowId attribute is empty");
                        IsNotEmpty(message.NameAttr, "Name attribute is empty");
                        FlowId = message.FlowIdAttr;
                        _messages.Push(message);
                        break;

                    case "testSuiteFinished":
                        AreEqual(_messages.Count, 2, "testSuiteFinished should close testSuiteStarted");
                        var testSuiteStarted = _messages.Pop();
                        AreEqual(testSuiteStarted.Name, "testSuiteStarted", "testSuiteFinished should close testSuiteStarted");
                        AreEqual(testSuiteStarted.FlowIdAttr, message.FlowIdAttr, "Invalid FlowId attribute");
                        AreEqual(testSuiteStarted.NameAttr, message.NameAttr, "Invalid Name attribute");
                        break;

                    case "flowStarted":
                        _messages.Push(message);
                        break;

                    case "flowFinished":
                        var flowStarted = _messages.Pop();
                        break;

                    case "testStarted":
                        AreEqual(message.FlowIdAttr, FlowId, "Invalid FlowId attribute");
                        IsNotEmpty(message.NameAttr, "Name attribute is empty");
                        if (message.CaptureStandardOutputAttr != null)
                            AreEqual(message.CaptureStandardOutputAttr, "false", "Invalid CaptureStandardOutput attribute");

                        _messages.Push(message);
                        break;

                    case "testFinished":
                        AreEqual(message.FlowIdAttr, FlowId, "Invalid FlowId attribute");
                        IsNotEmpty(message.NameAttr, "Name attribute is empty");
                        Greater(_messages.Count, 1, "testFinished should close testStarted");
                        var testStarted = _messages.Pop();
                        AreEqual(testStarted.Name, "testStarted", "testFinished should close testStarted");
                        AreEqual(testStarted.NameAttr, message.NameAttr, "Invalid Name attribute");
                        IsNotEmpty(message.DurationAttr, "Duration attribute is empty");
                        break;

                    case "testStdOut":
                    case "testStdErr":
                        AreEqual(message.FlowIdAttr, FlowId, "Invalid FlowId attribute");
                        IsNotEmpty(message.NameAttr, "Name attribute is empty");
                        Greater(_messages.Count, 1, "testStdOut should be within testStarted and testFinished");
                        var testStartedForStdOut = _messages.Peek();
                        AreEqual(testStartedForStdOut.Name, "testStarted", "testStdOut should be within testStarted and testFinished");
                        AreEqual(testStartedForStdOut.NameAttr, message.NameAttr, "Invalid Name attribute");
                        IsNotEmpty(message.OutAttr, "Out attribute is empty");
                        IsNotEmpty(message.TcTagsAttr, "tc:tags should be tc:parseServiceMessagesInside");
                        break;

                    case "testFailed":
                        AreEqual(message.FlowIdAttr, FlowId, "Invalid FlowId attribute");
                        IsNotEmpty(message.NameAttr, "Name attribute is empty");
                        Greater(_messages.Count, 1, "testFailed should be within testStarted and testFinished");
                        var testStartedForTestFailed = _messages.Peek();
                        AreEqual(testStartedForTestFailed.Name, "testStarted", "testFailed should be within testStarted and testFinished");
                        AreEqual(testStartedForTestFailed.NameAttr, message.NameAttr, "Invalid Name attribute");
                        // IsNotEmpty(message.MessageAttr, "Message attribute is empty");
                        IsNotNull(message.DetailsAttr, "Details attribute is empty");
                        break;

                    case "testIgnored":
                        AreEqual(message.FlowIdAttr, FlowId, "Invalid FlowId attribute");
                        IsNotEmpty(message.NameAttr, "Name attribute is empty");
                        Greater(_messages.Count, 1, "testIgnored should be within testStarted and testFinished");
                        var testStartedForTestIgnored = _messages.Peek();
                        AreEqual(testStartedForTestIgnored.Name, "testStarted", "testIgnored should be within testStarted and testFinished");
                        AreEqual(testStartedForTestIgnored.NameAttr, message.NameAttr, "Invalid Name attribute");
                        // IsNotEmpty(message.MessageAttr, "Message attribute is empty");
                        break;
                    
                    case "blockOpened":
                        AreEqual(message.FlowIdAttr, FlowId, "Invalid FlowId attribute");
                        IsNotEmpty(message.NameAttr, "Name attribute is empty");
                        break;
                    
                    case "blockClosed":
                        AreEqual(message.FlowIdAttr, FlowId, "Invalid FlowId attribute");
                        IsNotEmpty(message.NameAttr, "Name attribute is empty");
                        break;

                    default:
                        Fail($"Unexpected message {message.Name}");
                        break;
                }
            }
        }

        private static void Fail(string message)
        {
            throw new Exception(message);
        }

        private static void IsNotNull(string str, string message)
        {
            str.ShouldNotBeNull(message);
        }

        private static void Greater(int val1, int val2, string message)
        {
            val1.ShouldBeGreaterThan(val2, message);
        }

        private static void IsNotEmpty(string str, string message = "")
        {
            str.ShouldNotBeNullOrEmpty(message);
        }

        private static void AreEqual<T>(T val1, T val2, string message)
        {
            val1.ShouldBe(val2, message);
        }
    }
}