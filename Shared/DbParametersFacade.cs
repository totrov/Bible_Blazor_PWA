using Bible_Blazer_PWA.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bible_Blazer_PWA.Shared
{
    public class DbParametersFacade
    {
        private DatabaseJSFacade _db;
        public DbParametersFacade(DatabaseJSFacade db)
        {
            _db = db;
        }

        public async Task<string> GetParameterAsync(string key)
        {
            TaskCompletionSource taskCompletionSource = new TaskCompletionSource();

            var resultHandler = await _db.GetRecordFromObjectStoreByKey<string>("parameters", key);
            string result = null;
            resultHandler.OnDbResultOK += () => { result = resultHandler.Result; taskCompletionSource.SetResult(); };
            await taskCompletionSource.Task;
            return result;
        }

        public async Task<bool> SetParameterAsync(string key, string value)
        {
            //var resultHandler = await _db.SetRecord<string>("parameters", key);
            //string result = null;
            //resultHandler.OnDbResultOK += () => { result = resultHandler.Result; taskCompletionSource.SetResult(); };
            return false;
        }
    }
}
