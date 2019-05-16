using System;
using System.Collections.Generic;
using System.Text;

namespace AnalizatorLeksykalny
{
    public class MatchObject
    {
        public string Value { get; set; }
        public MatchType MatchType { get; set; }
        public int Index { get; set; }

        public override string ToString()
        {
            switch (MatchType)
            {
                case MatchType.Integer:
                    return $"Integer found: {Value}";
                case MatchType.Float:
                    return $"Float found: {Value}";
                case MatchType.Identifier:
                    return $"Identifier found: {Value}";
                case MatchType.Operator:
                    return $"Operator found: {Value}";
                case MatchType.Bracket:
                    return $"Bracket found: {Value}";
                default:
                    return $"Exception found: {Value}";
            }
        }
    }
}
