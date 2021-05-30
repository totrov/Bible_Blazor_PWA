using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bible_Blazer_PWA
{
    [System.Obsolete("obsolete implementation. Use DataBaseBibleServiceFetchStrategy instead.")]
    internal class InMemoryBibleServiceFetchStrategy : IBibleServiceFetchStrategy
    {
        private BibleService.Verse[] _verses;
        private BibleService.Book[] _books;

        public InMemoryBibleServiceFetchStrategy(BibleService.Verse[] verses, BibleService.Book[] books)
        {
            _verses = verses;
            _books = books;
        }

        public async Task<int> GetBookIdByShortNameAsync(string bookShortName)
        {
            return await Task.Run(() => _books.Where(b => b.ShortName == bookShortName).Select(b => b.Id).First());
        }

        public async Task<IEnumerable<BibleService.Verse>> GetVersesAsync(int bookId, int chapter, int fromVerse, int? toVerse)
        {
            return await Task.Run(() =>
                _verses.Where(v =>
                    v.Chapter == chapter
                    && v.Id >= fromVerse
                    && (v.Id <= (toVerse == null ? fromVerse : toVerse))
                    && v.BookId == bookId
                )
            );
        }
    }
}