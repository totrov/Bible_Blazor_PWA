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
        class ParameterModel
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }

        public async Task<string> GetParameterAsync(string key)
        {
            if (await CheckHasParameterAsync(key))
            {
                var resultHandler = await _db.GetRecordFromObjectStoreByKey<ParameterModel>("parameters", key);
                ParameterModel result = await resultHandler.GetTaskCompletionSourceWrapper();
                return result.Value;
            }
            return null;
        }

        public async Task<bool> CheckHasParameterAsync(string key)
        {
            var resultHandler = await _db.GetCountFromObjectStoreByKey("parameters", key);
            int result = await resultHandler.GetTaskCompletionSourceWrapper();
            return result != 0;
        }

        public async Task<bool> SetParameterAsync(string key, string value)
        {
            var resultHandler = await _db.SetKeyValueIntoObjectStore("parameters", key, value);
            bool result = await resultHandler.GetTaskCompletionSourceWrapper();
            return result;
        }
    }
}
