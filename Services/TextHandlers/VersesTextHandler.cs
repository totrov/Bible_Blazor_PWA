using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Bible_Blazer_PWA.Services.TextHandlers
{
    public class VersesTextHandler
    {
        private string bibleRefVersesNumbersColor;

        public VersesTextHandler(string bibleRefVersesNumbersColor)
        {
            this.bibleRefVersesNumbersColor = bibleRefVersesNumbersColor;
        }

        internal string GetHtmlFromVerses(IEnumerable<BibleService.Verse> verses, bool singleVerse, bool _startVersesOnANewLine)
        {
            var br = _startVersesOnANewLine ? "<br>" : "";
            return verses.Select(v => HandleSingleVerse(v, singleVerse)).Aggregate((a, b) => { return $"{a}{br}{b}"; });
        }

        private string HandleSingleVerse(BibleService.Verse verse, bool singleVerse)
        {
            return AddCursive(RemoveTags(AddNumberLabelIfNeeded(verse, singleVerse)));
        }

        private string AddCursive(string text)
        {
            return $"<i>{text}</i>";
        }

        private string AddNumberLabelIfNeeded(BibleService.Verse verse, bool singleVerse)
        {
            return singleVerse ? verse.Value : $"<sup style=\"color:{bibleRefVersesNumbersColor};\">{verse.Id} </sup>{verse.Value}";
        }

        private string RemoveTags(string text)
        {
            return Regex.Replace(text, @"(?:<S>.*?</S>)|(?:<f>.*?</f>)|<pb/>|<t>|</t>|<i>|</i>|<J>|</J>", "");
        }


    }
}
