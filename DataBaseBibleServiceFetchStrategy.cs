using Bible_Blazer_PWA.DataBase;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bible_Blazer_PWA
{
    internal class DataBaseBibleServiceFetchStrategy : IBibleServiceFetchStrategy
    {
        private DatabaseJSFacade _db;

        public DataBaseBibleServiceFetchStrategy(DatabaseJSFacade database)
        {
            _db = database;
        }
        public async Task<int> GetBookIdByShortNameAsync(string bookShortName)
        {
            TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();
            IndexedDBResultHandler<BibleService.Book> resultHandler = await _db.CallDbAsync<BibleService.Book>(null, "getRecordFromObjectStoreByIndex", "books", "ShortName", bookShortName);
            resultHandler.OnDbResultOK += () => { tcs.SetResult(resultHandler.Result.Id); }; 
            return await tcs.Task;            
        }

        public async Task<IEnumerable<BibleService.Verse>> GetVersesAsync(int bookId, int chapter, int fromVerse, int? toVerse)
        {
            TaskCompletionSource<IEnumerable<BibleService.Verse>> tcs = new TaskCompletionSource<IEnumerable<BibleService.Verse>>();

            var dbFacade = new Parameters.DbParametersFacade(_db);
            await dbFacade.Init();
            string versesObjectStoreName = dbFacade.ParametersModel.Lang switch
            {
                "1" => "verses_UA",
                "2" => "verses_RO",
                _ => "verses"
            };

            if (toVerse != null)
            {
                var resultHandler = await _db.GetRangeFromObjectStoreByKey<BibleService.Verse>(
                    versesObjectStoreName, bookId, chapter, fromVerse, toVerse);
                resultHandler.OnDbResultOK += () => { tcs.SetResult(resultHandler.Result); };
            }
            else
            {
                LinkedList<BibleService.Verse> verses = new LinkedList<BibleService.Verse>();
                var resultHandler = await _db.GetRecordFromObjectStoreByKey<BibleService.Verse>(
                    versesObjectStoreName, bookId, chapter, fromVerse);
                resultHandler.OnDbResultOK += () => { verses.AddLast(resultHandler.Result); tcs.SetResult(verses); };
            }
            
            return await tcs.Task;
        }
    }
}