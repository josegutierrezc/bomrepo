using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomRepo.EntityNamePattern
{
    public static class EntityNamePattern
    {
        public static bool MatchPattern(string pattern, string partname) {
            if (partname.Length != pattern.Length) return false;

            partname = partname.ToUpper();
            pattern = pattern.ToUpper();
            for (int i = 0; i <= partname.Length - 1; i++) {
                if (pattern[i] == '@')
                {
                    if (!char.IsLetter(partname[i])) return false;
                }
                else if (pattern[i] == '#') {
                    if (!char.IsNumber(partname[i])) return false;
                }
                else if (partname[i] != pattern[i]) return false;
            }
            return true;
        }
    }
}
