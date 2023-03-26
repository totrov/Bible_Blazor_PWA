using Bible_Blazer_PWA.DataBase;
using Bible_Blazer_PWA.DataBase.DTO;
using Bible_Blazer_PWA.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace Bible_Blazer_PWA.Facades
{
    public class DBCache
    {
        private readonly DatabaseJSFacade db;
        public static string LessonMenuBlocks = "LessonMenuBlocks";

        public DBCache(DatabaseJSFacade db)
        {
            this.db = db;
        }

        public async Task<T> GetFromCache<T>(string key)
        {
            CacheDTO cache = await (await db.GetRecordFromObjectStoreByKey<CacheDTO>("cache", key)).GetTaskCompletionSourceWrapper();
            if (cache is null)
                return default(T);
            return System.Text.Json.JsonSerializer.Deserialize<T>(cache.Value);
        }

        public async Task<bool> TryPopulateFromCache<T, TValue>(string key, T collection) where T : IDictionary<string, TValue>
        {
            CacheDTO cache = await (await db.GetRecordFromObjectStoreByKey<CacheDTO>("cache", key)).GetTaskCompletionSourceWrapper();
            if (cache is null)
                return false;
            using var jsonDoc = JsonDocument.Parse(cache.Value);
            foreach (var jsonElement in jsonDoc.RootElement.EnumerateObject())
            {
                collection.Add(jsonElement.Name, jsonElement.Value.Deserialize<TValue>());
            }
            return true;
        }

        public async Task SetToCache<T>(string key, T obj)
        {
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = false
            };
            await (await db.SetKeyValueIntoObjectStore("cache", key, System.Text.Json.JsonSerializer.Serialize(obj, options))).GetTaskCompletionSourceWrapper();
        }
    }
}
