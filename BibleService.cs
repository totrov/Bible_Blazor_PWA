using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Bible_Blazer_PWA.DomainObjects;

namespace Bible_Blazer_PWA
{
    public class BibleService
    {
        public class Verse
        {
            public int BookId { get; set; }
            public int Chapter { get; set; }
            public string Value { get; set; }
            public int Id { get; set; }
        }

        public class VersesView
        {
            public string Badge { get; set; }
            public string RawText { get; set; }
        }

        public class Book
        {
            public string Color { get; set; }
            public int Id { get; set; }
            public string Name { get; set; }
            public string ShortName { get; set; }
        }

        public IEnumerable<VersesView> GetVersesFromReference(BibleReference reference)
        {
            LinkedList<VersesView> result = new LinkedList<VersesView>();
            int bookId = _books.Where(b => (b.ShortName == reference.BookShortName)).Select(b => b.Id).First();
            string badge = "";
            foreach (BibleVersesReference versesReference in reference.References)
            {
                badge = $"{versesReference.Chapter}:";
                foreach (FromToVerses fromTo in versesReference.FromToVerses)
                {
                    VersesView versesView = new VersesView();
                    string toVerse = fromTo.ToVerse == null ? "" : $"-{fromTo.ToVerse}";
                    versesView.Badge = $"{badge}{fromTo.FromVerse}{toVerse}";
                    versesView.RawText = _verses
                        .Where(v =>(
                            v.Chapter == versesReference.Chapter
                            && v.Id >= fromTo.FromVerse
                            && (v.Id <= (fromTo.ToVerse == null ? fromTo.FromVerse : fromTo.ToVerse))
                            && v.BookId == bookId
                        ))
                        .Select(v => Regex.Replace(
                            v.Value,
                            @"(?:<S>.*?</S>)|(?:<f>.*?</f>)|<pb/>|<t>|</t>|<i>|</i>", "")
                        )
                        .Aggregate((a, b) => { return a + b; });
                    result.AddLast(versesView);
                }
            }

            return result;
        }

        public string getVerseValue(string bookName, int chapter, int verse)
        {
            int bookId = _books.Where(b => (b.ShortName == bookName)).Select(b => b.Id).FirstOrDefault();
            return getVerseValue(bookId, chapter, verse);
        }

        public string getVerseValue(int bookId, int chapter, int verse)
        {
            string verseValue = _verses.Where(v => (v.Chapter == 3 && v.Id == 15 && v.BookId == bookId)).Select(v => v.Value).FirstOrDefault();
            return verseValue;
        }

        public string perfTest()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 10; i <= 100; i += 10)
            {
                for (int j = 1; j <= 2; j++)
                {
                    for (int k = 1; k <= 10; k++)
                    {
                        sb.Append(getVerseValue(i, j, k));
                    }
                }
            }
            return sb.ToString();
        }

        private Verse[] _verses;
        private Book[] _books;
        private bool _isLoaded = false;
        public bool IsLoaded { get { return _isLoaded; } }
        public void Init(Verse[] verses, Book[] books)
        {
            _verses = verses;
            _books = books;
            _isLoaded = true;
        }

    }
}
