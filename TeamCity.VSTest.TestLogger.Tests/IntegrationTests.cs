﻿// ReSharper disable ConvertToConstant.Local
namespace TeamCity.VSTest.TestLogger.Tests;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Helpers;
using Shouldly;
using Xunit;

public class IntegrationTests
{
    private readonly string _resultsPath;
        
    public IntegrationTests()
    {
#if DEBUG
        var configuration = "Debug";
#else
        var configuration = "Release";
#endif
        _resultsPath = Path.Combine(CommandLine.WorkingDirectory, Path.Combine("bin", Path.Combine(configuration, "IntegrationTestResults.txt")));
    }

    [Fact]
    public void ShouldProduceServiceMessages()
    {
        // Given

        // When

        // Then
        foreach (var testResult in GetTestResults())
        {
            foreach (var message in testResult.Messages)
            {
                ServiceMessages.GetNumberOfServiceMessages(message.Item2).ShouldBe(message.Item1, testResult.ToString());
                ServiceMessages.ResultShouldContainCorrectStructureAndSequence(testResult.ToString());
            }
        }
    }

    private IEnumerable<TestResult> GetTestResults()
    {
        var state = State.Unknown;
        var header = new StringBuilder();
        var commands = new StringBuilder();
        var results = new StringBuilder();
        var messages = new List<Tuple<int, StringBuilder>>();
        Tuple<int, StringBuilder>? messageBlock = default;
        foreach (var line in File.ReadLines(_resultsPath))
        {
            var trimmedLine = line.Trim();
            if (trimmedLine.Length > 3 && trimmedLine[..3] == "!!!")
            {
                var stateInfo = trimmedLine.Substring(3, trimmedLine.Length - 3);
                var stateParts = stateInfo.Split(':');
                if (stateParts.Length > 0 && Enum.TryParse(stateParts[0], true, out State curState))
                {
                    if (curState == State.Messages && stateParts.Length > 1)
                    {
                        if (int.TryParse(stateParts[1], out var curMessageCount))
                        {
                            state = curState;
                            messageBlock = Tuple.Create(curMessageCount, new StringBuilder());
                            messages.Add(messageBlock);
                            continue;
                        }
                    }
                    {
                        state = curState;
                        continue;
                    }
                }
            }

            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (state)
            {
                case State.Start:
                    header.AppendLine(line);
                    break;

                case State.Commands:
                    commands.AppendLine(line);
                    break;

                case State.Messages:
                    results.AppendLine(line);
                    if (messageBlock == null)
                    {
                        throw new InvalidOperationException();
                    }

                    messageBlock.Item2.AppendLine(line);
                    break;

                case State.Results:
                    results.AppendLine(line);
                    break;

                case State.Finish:
                    yield return new TestResult(
                        header.ToString(),
                        commands.ToString(),
                        results.ToString(),
                        messages.Select(i => Tuple.Create(i.Item1, i.ToString())).ToList());
                    state = State.Unknown;
                    header.Clear();
                    commands.Clear();
                    results.Clear();
                    messages.Clear();
                    messageBlock = default;
                    break;
            }
        }

    }

    private enum State
    {
        Unknown,
        Start,
        Commands,
        Results,
        Messages,
        Finish
    }

    private class TestResult(
        string header,
        string commands,
        string results,
        ICollection<Tuple<int, string>> messages)
    {
        private string Header { get; } = header ?? throw new ArgumentNullException(nameof(header));

        private string Commands { get; } = commands ?? throw new ArgumentNullException(nameof(commands));

        private string Results { get; } = results ?? throw new ArgumentNullException(nameof(results));

        public IEnumerable<Tuple<int, string>> Messages { get; } = messages ?? throw new ArgumentNullException(nameof(messages));

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(Header);
            sb.AppendLine("_________");
            sb.AppendLine("Commands:");
            sb.AppendLine(Commands);
            sb.AppendLine("_________");
            sb.AppendLine("Results:");
            sb.AppendLine(Results);
            sb.AppendLine("_________");
            return sb.ToString();
        }
    }
}