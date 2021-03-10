using Bible_Blazer_PWA.BibleReferenceParse;
using Bible_Blazer_PWA.DomainObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bible_Blazer_PWA.Services.Parse
{
    public class FromToRegex
    {
        public FromToVerses Interpret(string stringToParse)
        {
            var result = new FromToVerses();
            var fromMatch = Regex.Matches(stringToParse, new FromRegex().GetPattern());
            return result;
        }
    }
}
