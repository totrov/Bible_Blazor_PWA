using System.Collections.Generic;
using System.Threading.Tasks;
using static Bible_Blazer_PWA.BibleService;

namespace Bible_Blazer_PWA
{
    public interface IBibleServiceFetchStrategy
    {
        Task<IEnumerable<Verse>> GetVersesAsync(int bookId, int chapter, int fromVerse, int? toVerse);
        Task<int> GetBookIdByShortNameAsync(string bookShortName);
    }
}
