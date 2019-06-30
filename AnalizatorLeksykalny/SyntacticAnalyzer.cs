using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AnalizatorLeksykalny
{
    public class SyntacticAnalyzer
    {
        public static bool AnalyzeSyntacticaly(List<MatchObject> matchesFromLine)
        {
            //matchesFromLine.ForEach(Console.WriteLine);
            var evenAmountOfBrackets = EvenAmountOfBrackets(matchesFromLine);

            if (evenAmountOfBrackets == false)
                return false;

            Console.WriteLine(IsNextTokenValid(matchesFromLine));

            return false;
        }

        private static bool? EvenAmountOfBrackets(List<MatchObject> matches)
        {
            var allBrackets = matches.Where(x => x.MatchType == MatchType.Bracket);
            if (allBrackets.Count() == 0)
            {
                return null;
            }

            var leftBracketCount = matches.Count(x => x.Value == "(");
            var rightBracketCount = matches.Count(x => x.Value == ")");

            return leftBracketCount == rightBracketCount;
        }

        private static bool IsNextTokenValid(List<MatchObject> matches)
        {
            MatchType? nextType = MatchType.NotFound;
            //

            for (int i = 0; i < matches.Count(); i++)
            {
                if (i < matches.Count() - 1)
                {
                    if (i == 0 && matches[i].MatchType == MatchType.Operator)
                        return false;

                    nextType = matches[i + 1].MatchType;

                    if (matches[i].MatchType == nextType && matches[i].MatchType != MatchType.Bracket)
                        return false;

                    switch (matches[i].MatchType)
                    {
                        case MatchType.NotFound:
                            break;
                        case MatchType.Integer:
                            if (nextType == MatchType.Float || nextType == MatchType.Identifier)
                                return false;

                            break;
                        case MatchType.Float:
                            if (nextType == MatchType.Integer || nextType == MatchType.Identifier)
                                return false;
                            break;
                        case MatchType.Identifier:
                            if (nextType == MatchType.Integer || nextType == MatchType.Float)
                                return false;
                            break;
                        case MatchType.Operator:
                            if (nextType == MatchType.Bracket && matches[i + 1].Value == ")")
                                return false;
                            break;
                        case MatchType.Bracket:
                            if (nextType == MatchType.Operator && matches[i].Value == "(")
                                return false;
                            break;
                        default:
                            break;
                    }
                }
            }

            return true;
        }
        // 2 + x
        // x + x 
        // x + )

        // prev
        // null int operator id

        //next
        // operator id null


    }
}
