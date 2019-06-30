using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AnalizatorLeksykalny
{
    public class Analyzer
    {
        private readonly string IntegerRegex = @"(?<=^\.|\s|^|[-+\/*()\\])\d+(?=[a-zA-Z\/\\()]|$|\s|[-+\/*]|\.[a-zA-Z\s]+|(\.\.)+)";
        private readonly string FloatRegex = @"(?<![a-zA-Z0-9])([0-9]+\.[0-9]+)";
        private readonly string IdentifierRegex = @"([a-zA-Z]+[a-zA-Z0-9]+|[a-zA-Z])";
        private readonly string OperatorRegex = @"([\+\-\*\/])";
        private readonly string BracketRegex = @"([\(\)])";

        public void AnalyzeLexically(string text)
        {
            var fullRegexString = string.Join("|", new[] { IntegerRegex, FloatRegex, IdentifierRegex, OperatorRegex, BracketRegex });
            var lines = text.Split("\r\n");

            foreach (var line in lines)
            {
                List<MatchObject> matchObjects = new List<MatchObject>();

                Console.WriteLine("==========================");
                Console.WriteLine($"Text Line: {line} \r\n");

                var integerMatches = Regex.Matches(line, IntegerRegex);
                matchObjects.AddRange(integerMatches.Select(x =>
                    new MatchObject
                    {
                        MatchType = MatchType.Integer,
                        Value = x.Value,
                        Index = x.Index
                    }));


                var floatMatches = Regex.Matches(line, FloatRegex);
                matchObjects.AddRange(floatMatches.Select(x =>
                    new MatchObject
                    {
                        MatchType = MatchType.Float,
                        Value = x.Value,
                        Index = x.Index
                    }));

                var identifierMatches = Regex.Matches(line, IdentifierRegex);
                matchObjects.AddRange(identifierMatches.Select(x =>
                    new MatchObject
                    {
                        MatchType = MatchType.Identifier,
                        Value = x.Value,
                        Index = x.Index
                    }));

                var operatorMatches = Regex.Matches(line, OperatorRegex);
                matchObjects.AddRange(operatorMatches.Select(x =>
                    new MatchObject
                    {
                        MatchType = MatchType.Operator,
                        Value = x.Value,
                        Index = x.Index
                    }));

                var bracketMatches = Regex.Matches(line, BracketRegex);
                matchObjects.AddRange(bracketMatches.Select(x =>
                    new MatchObject
                    {
                        MatchType = MatchType.Bracket,
                        Value = x.Value,
                        Index = x.Index
                    }));

                PrintWithExceptions(line, matchObjects);

                Console.WriteLine("==========================");
                Console.WriteLine();
            }
        }

        private void PrintWithExceptions(string line, List<MatchObject> allMatches)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                Console.WriteLine("Line is empty");
                return;
            }

            var reducedLine = line;
            var tmp = new List<MatchObject>();
            var sorted = allMatches.OrderBy(x => x.Index).ToList();

            for (int i = 0; i < sorted.Count; i++)
            {
                var indexToRemove = reducedLine.IndexOf(sorted[i].Value);
                reducedLine = reducedLine.Remove(indexToRemove, sorted[i].Value.Length).Trim();
            }

            if (reducedLine.Length > 0)
            {
                var tmpSplit = reducedLine.Split().Where(x => x.Length > 0).ToArray();

                for (int i = 1; i <= tmpSplit.Length; i++)
                {
                    var exceptionOccurences = tmpSplit.Where(x => x == tmpSplit[i - 1]).Count();
                    var occurence = i > 1 ? (exceptionOccurences == 1 ? 1 : i) : i;
                    var nthIndexOf = line.NthIndexOf(tmpSplit[i - 1], occurence);
                    tmp.Add(new MatchObject
                    {
                        Index = nthIndexOf,
                        MatchType = MatchType.NotFound,
                        Value = tmpSplit[i - 1]
                    });
                }
            }
            sorted.AddRange(tmp);
            sorted = sorted.OrderBy(x => x.Index).ToList();

            var errorCount = sorted.Where(x => x.MatchType == MatchType.NotFound).Count();

            if (errorCount > 0)
            {
                Console.WriteLine("Line has errors");
            }
            else
            {
                SyntacticAnalyzer.AnalyzeSyntacticaly(sorted);
            }
        }
    }
}
