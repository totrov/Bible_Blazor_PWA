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
        private LinkedList<BibleReference> _bibleReferences;
        private LinkedList<ITextExpression> _textExpressions;

        public Parser()
        {
            _bibleReferences = new LinkedList<BibleReference>();
            _textExpressions = new LinkedList<ITextExpression>();
        }

        public Parser ParseTextLineWithBibleReferences(string stringToParse)
        {
            string stringWithReplacements = Replacer.ReplaceBookNames(stringToParse);
            foreach (Match match in Regex.Matches(stringWithReplacements, BibleRegexHelper.GetBibleReferencesPattern()))
            {
                BibleReference bibleReference = this.CreateBibleReferenceFromMatch(match);
                _bibleReferences.AddLast(bibleReference);
            }

            return this;
        }

        private BibleReference CreateBibleReferenceFromMatch(Match match)
        {
            BibleReference bibleReference = new BibleReference();
            bibleReference.BookShortName = match.Groups.Where(g => g.Name == "book").First().Value;
            bibleReference.References = new LinkedList<BibleVersesReference>();
            foreach (Capture capture in match.Groups.Where(g => g.Name == "ref").First().Captures)
            {
                BibleVersesReference bibleVerseReference = CreateBibleVerseReferenceFromString(capture.Value);
                bibleReference.References.AddLast(bibleVerseReference);
            }
            return bibleReference;
        }

        private BibleVersesReference CreateBibleVerseReferenceFromString(string stringToParse)
        {
            BibleVersesReference bibleVersesReference = new BibleVersesReference();

            Match match = Regex.Match(stringToParse, BibleRegexHelper.GetBibleVerseReferencesPattern());
            bibleVersesReference.Chapter = int.Parse(match.Groups.Where(g => g.Name == "chapter").First().Value);
            bibleVersesReference.FromToVerses = new LinkedList<FromToVerses>();
            foreach (Capture capture in match.Groups.Where(g => g.Name == "fromTo").First().Captures)
            {
                bibleVersesReference.FromToVerses.AddLast(CreateFromToVerseFromString(capture.Value));
            }

            return bibleVersesReference;
        }

        private FromToVerses CreateFromToVerseFromString(string stringToParse)
        {
            FromToVerses fromToVerses = new FromToVerses();
            Match match = Regex.Match(stringToParse, BibleRegexHelper.GetFromToVersesPattern());
            fromToVerses.FromVerse = int.Parse(match.Groups.Where(g => g.Name == "from").First().Value);
            var groups = match.Groups.Where(g => g.Name == "to");
            var debug = groups.FirstOrDefault();
            if (groups.Any())
            {
                string val = groups.First().Value;
                if (val != "")
                {
                    fromToVerses.ToVerse = int.Parse(val);
                }
            }

            return fromToVerses;
        }

        public LinkedList<BibleReference> GetBibleReferences()
        {
            return _bibleReferences;
        }
    }
}
