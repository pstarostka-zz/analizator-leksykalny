using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace AnalizatorLeksykalny
{
    class Program
    {
        static void Main(string[] args)
        {
            Analyzer analyzer = new Analyzer();


            analyzer.AnalyzeLexically(GetFileText());

            Console.ReadLine();
        }

        static string GetFileText()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetManifestResourceNames()
                .Single(str => str.EndsWith("example.txt"));

            string result = "";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                result += reader.ReadToEnd();
            }

            return result;
        }


    }

    public class Analyzer
    {
        private readonly string IntegerRegex = @"(?<=^\.|\s|^|[-+\/*()\\])\d+(?=[a-zA-Z\/\\()]|$|\s|[-+\/*]|\.[a-zA-Z\s]+|(\.\.)+)";
        private readonly string FloatRegex = @"([0-9]+\.[0-9]+)";
        private readonly string IdentifierRegex = @"([a-zA-Z]+[a-zA-Z0-9]+|[a-zA-Z])";
        private readonly string OperatorRegex = @"([\+\-\*\/])";
        private readonly string BracketRegex = @"([\(\)\{\}\[\]\<\>])";

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

            //Console.WriteLine(text);
        }

        private void PrintWithExceptions(string line, List<MatchObject> allMatches)
        {
            if (string.IsNullOrEmpty(line))
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
            sorted.ForEach(Console.WriteLine);
        }

        private void PrintMatches(MatchCollection matches, MatchType type)
        {
            if (matches.Count == 0)
                return;


            switch (type)
            {
                case MatchType.Integer:
                    Console.Write("Integers Found: ");
                    break;
                case MatchType.Float:
                    Console.Write("Floats Found: ");
                    break;
                case MatchType.Identifier:
                    Console.Write("Identifiers Found: ");
                    break;
                case MatchType.Operator:
                    Console.Write("Operators Found: ");
                    break;
                case MatchType.Bracket:
                    Console.Write("Brackets Found: ");
                    break;
                default:
                    return;
            }

            Console.Write(string.Join(", ", matches.Select(x => $"'{x}'")));

            Console.WriteLine();
        }


    }

}
