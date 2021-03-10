using Bible_Blazer_PWA.DomainObjects;
using Bible_Blazer_PWA.Services.Parse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bible_Blazer_PWA.BibleReferenceParse
{

    public class Parser
    {
        public static LinkedList<BibleReference> GetBibleReferences(string stringToParse)
        {
            ParsingContext context = new ParsingContext(stringToParse);
                        
            foreach (Match match in Regex.Matches(stringToParse, BibleRegexHelper.GetBibleReferencesPattern()))
            {
                result.AddLast(bookRegex.Interpret(match));
            }


            string spases = @"\s*";
            BookRegex bookRegex = new BookRegex();
            ChapterRegex chapterRegex = new ChapterRegex();
            FromRegex fromRegex = new FromRegex();
            ToRegex toRegex = new ToRegex();
            string pattern = string.Join(spases,
                bookRegex.GetPattern(),
                @"(?:;?(?<ref>",
                    chapterRegex.GetPattern(),
                    @"(?:,?(?<fromTo>",
                        fromRegex.GetPattern(), toRegex.GetPattern(),
                    @"))+",
                @"))+"
                );
            

            return result;
        }
    }
}
