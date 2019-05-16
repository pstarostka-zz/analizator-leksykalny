using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace AnalizatorLeksykalny
{
    public static class StringExtensions
    {
        public static int NthIndexOf(this string target, string value, int n)
        {
            int index = -1;
            int found = 0;

            while (true)
            {
                index = target.IndexOf(value, index + 1);

                if (index < 0)
                    return -1;

                if (++found == n)
                    return index;
            }
        }
    }
}
