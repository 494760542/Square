using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SOHO.Search.BLL
{
    class Check
    {
        public static bool CheckFloor(string input)
        {
            string pattern = "^[A-Za-z0-9]+$";
            Match match = Regex.Match(input, pattern);
            if (match.Groups.Count > 1)
            {
                return true;
            }
            return false;
        }

        public static bool CheckBuild(string input)
        {
            string pattern = "^[A-Za-z]+$";
            Match match = Regex.Match(input, pattern);
            if (match.Groups.Count > 1)
            {
                return true;
            }
            return false;
        }
    }
}
