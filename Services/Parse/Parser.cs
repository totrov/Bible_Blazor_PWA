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

            string stringWithReplacements = corrector.HandleBrackets(corrector.ReplaceBookNames(stringToParse));
            MatchCollection bibleRefMatches = Regex.Matches(stringWithReplacements, corrector.RegexHelper.GetBibleReferencesPattern());
            (int Index, string Match)[] sublevels = GetSublevelInfos(stringWithReplacements, bibleRefMatches);

            var pos = 0;
            foreach (Match match in bibleRefMatches)
            {
                CreateTokensAndSeekPos(stringWithReplacements, ref pos, match, sublevels);

                BibleReference bibleReference = this.CreateBibleReferenceFromMatch(match);
                _bibleReferences.AddLast(bibleReference);
            }
            this.AddTokenOfTextAfterLastReference(stringWithReplacements, pos, sublevels);
            parseCompleted = true;
            return this;
        }

        private void AddTokenOfTextAfterLastReference(string inputString, int pos, (int Index, string Match)[] sublevels)
        {
            (int Index, string Match)[] adjustedSublevels = GetAdjustedSublevelInfos(sublevels, pos, inputString.Length - 1);
            PlaceNonRefToken(inputString.Substring(pos), adjustedSublevels);
        }

        private void CreateTokensAndSeekPos(string stringWithReplacements, ref int pos, Match match, (int Index, string Match)[] sublevels)
        {
            if (match.Index > pos)
            {
                var previousText = stringWithReplacements.Substring(pos, match.Index - pos);

                (int Index, string Match)[] adjustedSublevels = GetAdjustedSublevelInfos(sublevels, pos, match.Index);
                PlaceNonRefToken(previousText, adjustedSublevels);
            }
            _tokens.AddLast(new LessonElementToken() { Text = match.Value, Type = TokenType.BibleReference });
            pos = match.Index + match.Length;
        }

        private (int Index, string Match)[] GetAdjustedSublevelInfos((int Index, string Match)[] indexesToAdjust, int pos, int end)
        {
            if (indexesToAdjust.Length < 2)
                return null;

            (int Index, string Match)[] adjustedSublevels = indexesToAdjust
                .Where(index => index.Index >= pos && index.Index <= end)
                .Select(index => (index.Index - pos, index.Match))
                .ToArray();

            return adjustedSublevels;
        }

        private (int Index, string Match)[] GetSublevelInfos(string text, MatchCollection bibleRefMatches)
        {
            string listItemPattern = @"([0-9][)])|(1[0-9][)])|([(][0-9][)])|([(][а-я][)])";
            int indexOf4LevelStart = text.IndexOf('\u0002');
            var falsePositives = Regex.Matches(text, "[(].*?[)]").Where(m => m.Length > 3)                     //for example (в) is 3 signs length. Can't use .{2,}, need non-greedy capture to avoid mix of two brackets like: "(в)some text(something else)" => "в)some text(something else"
                .Select(m => (
                    From: m.Index, 
                    To: m.Index + m.Length))
                .Concat(bibleRefMatches.Select(bibleRefMatch => (
                    From: bibleRefMatch.Index,
                    To: bibleRefMatch.Index + bibleRefMatch.Length)));

            return Regex.Matches(text, listItemPattern).Select(m => (m.Index, m.Value))
                .Where(subItemMatch =>
                    subItemMatch.Index > indexOf4LevelStart
                    && !falsePositives.Any(falsePositive => 
                        falsePositive.From <= subItemMatch.Index && falsePositive.To >= subItemMatch.Index)
            ).ToArray();
        }

        private void PlaceNonRefToken(string text, (int Index, string Match)[] sublevels)
        {
            if (sublevels is null || sublevels.Length == 0)
            {
                _tokens.AddLast(new LessonElementToken() { Text = text.Replace("\u0003", ""), Type = TokenType.PlainText });
                return;
            }

            if (sublevels[0].Index > 0)
                _tokens.AddLast(new LessonElementToken() { Text = text.Substring(0, sublevels[0].Index).Replace("\u0002", ""), Type = TokenType.PlainText });

            for (int i = 0; i < sublevels.Length; i++)
            {
                _tokens.AddLast(new ListItemToken());
                _tokens.AddLast(new LessonElementToken
                {
                    Text = text.Substring(sublevels[i].Index,
                        i == sublevels.Length - 1
                        ? text.Length - sublevels[i].Index
                        : sublevels[i + 1].Index - sublevels[i].Index)
                        .Replace(sublevels[i].Match, string.Empty).Replace("\u0003", ""),
                    Type = TokenType.PlainText
                });
            }
        }

        private BibleReference CreateBibleReferenceFromMatch(Match match)
        {
            BibleReference bibleReference = new BibleReference();
            bibleReference.BookShortName = match.Groups.Cast<Group>().Where(g => g.Name == "book").First().Value;
            bibleReference.References = new LinkedList<BibleVersesReference>();

            foreach (Capture capture in match.Groups.Cast<Group>().Where(g => g.Name == "ref").First().Captures)
                foreach (BibleVersesReference bibleVerseReference in CreateBibleVerseReferencesFromString(capture.Value))
                    bibleReference.References.AddLast(bibleVerseReference);

            return bibleReference;
        }

        private IEnumerable<BibleVersesReference> CreateBibleVerseReferencesFromString(string stringToParse)
        {
            BibleVersesReference bibleVersesReference = new BibleVersesReference();
            Match match;
            int chapterCount = stringToParse.Count(ch => ch == ':');
            switch (chapterCount)
            {
                case 0:
                    string pattern = corrector.RegexHelper.GetBibleReferencesPattern_ChapterOnly();
                    match = Regex.Match(stringToParse, pattern);
                    var refsGroup = match.Groups.Cast<Group>().Where(g => g.Name == "ref").FirstOrDefault();
                    if (refsGroup != null)
                        foreach (Capture capture in refsGroup.Captures)
                        {
                            bibleVersesReference = new BibleVersesReference();
                            var uniqueRefGroups = Regex.Match(capture.Value, pattern).Groups.Cast<Group>();
                            bibleVersesReference.Chapter = int.Parse(uniqueRefGroups.First(g => g.Name == "chapter").Captures.First().Value);
                            string toChapter = uniqueRefGroups.FirstOrDefault(g => g.Name == "chapterTo")?.Captures?.FirstOrDefault()?.Value;
                            if (toChapter != null)
                                bibleVersesReference.ChapterTo = int.Parse(toChapter);
                            yield return bibleVersesReference;
                        }
                    break;
                case 1:
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
                    break;
                default:
                    var commaIndexesToSplitBy = stringToParse.Select((ch, index) => ch == ':' ? index : -1)
                        .Where(colonIndex => colonIndex > -1)
                        .Skip(1)
                        .Select(colonIndex => stringToParse.LastIndexOf(',', colonIndex, colonIndex - 1)).Append(999);

                    int startIndex = 0;
                    foreach (int commaIndex in commaIndexesToSplitBy)
                    {
                        string subReference = string.Join(null, stringToParse.Where((ch, index) => index >= startIndex && index < commaIndex));
                        foreach (BibleVersesReference reference in CreateBibleVerseReferencesFromString(subReference))
                            yield return reference;
                        startIndex = commaIndex + 1;
                    }

                    break;
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
