using System;
using System.Collections.Generic;
using System.Text;

namespace AnalizatorLeksykalny
{
    public enum MatchType
    {
        NotFound=-1,
        Integer = 0,
        Float,
        Identifier,
        Operator,
        Bracket
    }

}
