using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomRepo.EntityNamePattern
{
    public static class EntityNamePattern
    {
        public static string GetFromName(string partname)
        {
            string pattern = string.Empty;
            foreach (char c in partname)
            {
                if (char.IsNumber(c))
                    pattern += "#";
                else if (char.IsLetter(c))
                    pattern += "@";
                else
                    pattern += c;
            }
            return pattern;
        }

        public static bool MatchPattern(string pattern, string partname) {
            string partnamepattern = GetFromName(partname);
            return pattern == partnamepattern;
        }
    }
}
