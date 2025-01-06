using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bible_Blazer_PWA.Services.Parse
{
    public class Corrector : ICorrector, IAsyncInitializable
    {
        public IRegexHelper RegexHelper { get; private set; }

        public Task InitTask => _initializationTask;
        private Task _initializationTask;

        public Corrector(IRegexHelper regexHelper)
        {
            RegexHelper = regexHelper;
            _initializationTask = regexHelper.Init();
        }

        public string ApplyHighLevelReplacements(string input, string unitId)
        {
            string result = input;

            if (RegexHelper.GetReplacements().ContainsKey(unitId))
            {
                result = MultipleReplace(result, RegexHelper.GetReplacements()[unitId]);
            }

            if (RegexHelper.GetContinualReplacements().ContainsKey(unitId))
            {
                foreach (var replacement in RegexHelper.GetContinualReplacements()[unitId])
                {
                    result = Regex.Replace(result, replacement.Key, replacement.Value);
                }
            }

            return result;
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
                string bookName = match.Groups.Cast<Group>().Where(g => g.Name == "book").First().Value;
                string bracketsContent = match.Groups.Cast<Group>().Where(g => g.Name == "bracketsContent").First().Value;

                string replacement = "(" + bookName;
                if (!bracketsContent.Contains(':'))
                {
                    string chapterVerseContent = match.Groups.Cast<Group>().Where(g => g.Name == "chapterVerse").First().Value;
                    int indexOfColon = chapterVerseContent.IndexOf(':');
                    if (indexOfColon > 0)
                        replacement += $"{chapterVerseContent.Substring(0, indexOfColon)}:";
                }
                replacement += bracketsContent + ")";

                buf = buf.Replace("(" + bracketsContent + ")", replacement);
            }
            return buf;
        }

        public string ReplaceBookNames(string stringToParse)
        {
            return RegexHelper.GetReplacements()["bookNames"]
                .Aggregate(stringToParse, (str, replacement) => { return str.Replace(replacement.Key, replacement.Value); });
        }
    }
}
