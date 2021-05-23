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
        private IBibleServiceFetchStrategy _dataProvider;
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

        public async Task<IEnumerable<VersesView>> GetVersesFromReference(BibleReference reference)
        {
            LinkedList<VersesView> result = new LinkedList<VersesView>();
            int bookId = await _dataProvider.GetBookIdByShortName(reference.BookShortName);
            string badge = "";
            foreach (BibleVersesReference versesReference in reference.References)
            {
                badge = $"{versesReference.Chapter}:";
                foreach (FromToVerses fromTo in versesReference.FromToVerses)
                {
                    VersesView versesView = new VersesView();
                    string toVerse = fromTo.ToVerse == null ? "" : $"-{fromTo.ToVerse}";
                    versesView.Badge = $"{badge}{fromTo.FromVerse}{toVerse}";
                    versesView.RawText = (await _dataProvider.GetVersesAsync(bookId, versesReference.Chapter, fromTo.FromVerse, fromTo.ToVerse))
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

        //public string getVerseValue(string bookName, int chapter, int verse)
        //{
        //    int bookId = _books.Where(b => (b.ShortName == bookName)).Select(b => b.Id).FirstOrDefault();
        //    return getVerseValue(bookId, chapter, verse);
        //}

        //public string getVerseValue(int bookId, int chapter, int verse)
        //{
        //    string verseValue = _verses.Where(v => (v.Chapter == 3 && v.Id == 15 && v.BookId == bookId)).Select(v => v.Value).FirstOrDefault();
        //    return verseValue;
        //}

        private bool _isLoaded = false;
        public bool IsLoaded { get { return _isLoaded; } }
        public void Init(Verse[] verses, Book[] books)
        {
            _dataProvider = new InMemoryBibleServiceFetchStrategy(verses, books);
            _isLoaded = true;
        }

    }
}
