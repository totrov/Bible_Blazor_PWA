using Bible_Blazer_PWA.DomainObjects;
using Bible_Blazer_PWA.Services.Parse;
using DocumentFormat.OpenXml.Wordprocessing;
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
        private Corrector corrector;

        public Parser(Corrector corrector)
        {
            _bibleReferences = new LinkedList<BibleReference>();
            _tokens = new LinkedList<LessonElementToken>();
            this.corrector = corrector;
        }

        public LinkedList<LessonElementToken> GetTokens() => _tokens;

        public Parser ParseTextLineWithBibleReferences(string stringToParse)
        {
            if (parseCompleted)
                return this;

            string stringWithReplacements = corrector.ReplaceBookNames(stringToParse);
            stringWithReplacements = corrector.HandleBrackets(stringWithReplacements);

            var pos = 0;
            foreach (Match match in Regex.Matches(stringWithReplacements, corrector.RegexHelper.GetBibleReferencesPattern()))
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
                foreach(BibleVersesReference bibleVerseReference in CreateBibleVerseReferencesFromString(capture.Value))
                    bibleReference.References.AddLast(bibleVerseReference);

            return bibleReference;
        }

        private IEnumerable<BibleVersesReference> CreateBibleVerseReferencesFromString(string stringToParse)
        {
            BibleVersesReference bibleVersesReference = new BibleVersesReference();
            Match match;
            if (stringToParse.Contains(':'))
            {
                match = Regex.Match(stringToParse, corrector.RegexHelper.GetBibleVerseReferencesPattern());

                bibleVersesReference.Chapter = int.Parse(match.Groups.Cast<Group>().Where(g => g.Name == "chapter").First().Value);
                if (match.Groups.Cast<Group>().Where(g => g.Name == "chapterTo").FirstOrDefault()?.Value is string { Length: > 0 } chapterTo)
                    bibleVersesReference.ChapterTo = int.Parse(chapterTo);

                bibleVersesReference.FromToVerses = new LinkedList<FromToVerses>();
                var group = match.Groups.Cast<Group>().Where(g => g.Name == "fromTo").FirstOrDefault();
                if (group != null)
                    foreach (Capture capture in group.Captures)
                    {
                        bibleVersesReference.FromToVerses.AddLast(CreateFromToVerseFromString(capture.Value));
                    }

                yield return bibleVersesReference;
            }
            else
            {
                string pattern = corrector.RegexHelper.GetBibleReferencesPattern_ChapterOnly();
                match = Regex.Match(stringToParse, pattern);
                var refsGroup = match.Groups.Cast<Group>().Where(g => g.Name == "ref").FirstOrDefault();
                if (refsGroup != null)
                    foreach (Capture capture in refsGroup.Captures)
                    {
                        bibleVersesReference = new BibleVersesReference();
                        var uniqueRefGroups = Regex.Match(capture.Value, pattern).Groups.Cast<Group>();
                        bibleVersesReference.Chapter = int.Parse(uniqueRefGroups.First(g => g.Name == "chapter").Captures.First().Value);
                        string chapterTo = uniqueRefGroups.FirstOrDefault(g => g.Name == "chapterTo")?.Captures?.FirstOrDefault()?.Value;
                        if (chapterTo != null)
                            bibleVersesReference.ChapterTo = int.Parse(chapterTo);
                        yield return bibleVersesReference;
                    }
            }
        }

        private FromToVerses CreateFromToVerseFromString(string stringToParse)
        {
            FromToVerses fromToVerses = new FromToVerses();
            Match match = Regex.Match(stringToParse, corrector.RegexHelper.GetFromToVersesPattern());
            string from = match.Groups.Cast<Group>().Where(g => g.Name == "from").First().Value;
            fromToVerses.FromVerse = int.Parse(from);
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
