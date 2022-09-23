using Bible_Blazer_PWA.DomainObjects;
using Bible_Blazer_PWA.Services.Parse;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Bible_Blazer_PWA.BibleReferenceParse
{

    public class Parser
    {
        private LinkedList<BibleReference> _bibleReferences;
        private LinkedList<LessonElementToken> _tokens;
        private bool parseCompleted = false;
        private Replacer replacer;

        public Parser(Replacer replacer)
        {
            _bibleReferences = new LinkedList<BibleReference>();
            _tokens = new LinkedList<LessonElementToken>();
            this.replacer = replacer;
        }

        public LinkedList<LessonElementToken> GetTokens() => _tokens;

        public Parser ParseTextLineWithBibleReferences(string stringToParse)
        {
            if (parseCompleted)
                return this;
            
            string stringWithReplacements = replacer.ReplaceBookNames(stringToParse);
            stringWithReplacements = replacer.HandleBrackets(stringWithReplacements);

            var pos = 0;
            foreach (Match match in Regex.Matches(stringWithReplacements, BibleRegexHelper.GetBibleReferencesPattern()))
            {
                CreateTokensAndSeekPos(stringWithReplacements, ref pos, match);

                BibleReference bibleReference = this.CreateBibleReferenceFromMatch(match);
                _bibleReferences.AddLast(bibleReference);
            }
            this.AddTokenOfTextAfterLastReference(stringWithReplacements, pos);
            parseCompleted = true;
            return this;
        }

        private void AddTokenOfTextAfterLastReference(string inputString, int pos)
        {
            _tokens.AddLast(new LessonElementToken() { Text = inputString.Substring(pos), Type = TokenType.PlainText });
        }

        private void CreateTokensAndSeekPos(string stringWithReplacements, ref int pos, Match match)
        {
            if (match.Index > pos)
            {
                var previousText = stringWithReplacements.Substring(pos, match.Index - pos);
                _tokens.AddLast(new LessonElementToken() { Text = previousText, Type = TokenType.PlainText });
            }
            _tokens.AddLast(new LessonElementToken() { Text = match.Value, Type = TokenType.BibleReference });
            pos = match.Index + match.Length;
        }

        private BibleReference CreateBibleReferenceFromMatch(Match match)
        {
            BibleReference bibleReference = new BibleReference();
            bibleReference.BookShortName = match.Groups.Cast<Group>().Where(g => g.Name == "book").First().Value;
            bibleReference.References = new LinkedList<BibleVersesReference>();
            foreach (Capture capture in match.Groups.Cast<Group>().Where(g => g.Name == "ref").First().Captures)
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
            bibleVersesReference.Chapter = int.Parse(match.Groups.Cast<Group>().Where(g => g.Name == "chapter").First().Value);
            bibleVersesReference.FromToVerses = new LinkedList<FromToVerses>();
            foreach (Capture capture in match.Groups.Cast<Group>().Where(g => g.Name == "fromTo").First().Captures)
            {
                bibleVersesReference.FromToVerses.AddLast(CreateFromToVerseFromString(capture.Value));
            }

            return bibleVersesReference;
        }

        private FromToVerses CreateFromToVerseFromString(string stringToParse)
        {
            FromToVerses fromToVerses = new FromToVerses();
            Match match = Regex.Match(stringToParse, BibleRegexHelper.GetFromToVersesPattern());
            fromToVerses.FromVerse = int.Parse(match.Groups.Cast<Group>().Where(g => g.Name == "from").First().Value);
            var groups = match.Groups.Cast<Group>().Where(g => g.Name == "to");
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
