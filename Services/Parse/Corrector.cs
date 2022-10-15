using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Bible_Blazer_PWA.Services.Parse
{
    public class Corrector : ICorrector
    {
        public IRegexHelper RegexHelper;

        public Corrector(IRegexHelper regexHelper)
        {
            this.RegexHelper = regexHelper;
        }

        public string ApplyHighLevelReplacements(string input, string unitId)
        {
            if (RegexHelper.GetReplacements().ContainsKey(unitId))
            {
                return MultipleReplace(input, RegexHelper.GetReplacements()[unitId]);
            }
            return input;
        }

        private string MultipleReplace(string text, Dictionary<string, string> replacements)
        {
            return Regex.Replace(
                text,
                "(" + String.Join("|", replacements.Keys).Replace("(", "[(]").Replace(")", "[)]") + ")",
                (Match m) => { return replacements[m.Value]; }
            );
        }

        public string HandleBrackets(string stringToParse)
        {
            string buf = stringToParse;
            foreach (Match match in Regex.Matches(stringToParse, RegexHelper.GetBracketsHandlerPattern()))
            {
                var bookName = match.Groups.Cast<Group>().Where(g => g.Name == "book").First().Value;
                var bracketsContent = match.Groups.Cast<Group>().Where(g => g.Name == "bracketsContent").First().Value;
                buf = buf.Replace("(" + bracketsContent + ")", "(" + bookName + bracketsContent + ")");
            }
            return buf;
        }

        public string ReplaceBookNames(string stringToParse)
        {
            return stringToParse;
            //return RegexHelper.GetReplacements()["bookNames"]
            //    .Aggregate(stringToParse, (str, replacement) => { return str.Replace(replacement.Key, replacement.Value); });
        }
    }
}
