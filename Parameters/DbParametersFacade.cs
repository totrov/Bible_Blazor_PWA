using Bible_Blazer_PWA.DataBase;
using Bible_Blazer_PWA.DomainObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Bible_Blazer_PWA.Parameters
{
    public class DbParametersFacade
    {
        private DatabaseJSFacade _db;
        public DbParametersFacade(DatabaseJSFacade db)
        {
            _db = db;
        }

        public ParametersModel ParametersModel { get; private set; }

        public async Task Init()
        {
            ParametersModel = new ParametersModel(this);
            await ParametersModel.InitFromDb();
            OnChange += ParametersModel.HandleParameterChange;
        }

        public event Action<Parameters, string> OnChange;
        public event Func<Parameters, string, Task> OnChangeAsync;

        class ParameterPOCO
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }

        private async Task<string> GetParameterAsync(string key)
        {
            if (await CheckHasParameterAsync(key))
            {
                var resultHandler = await _db.GetRecordFromObjectStoreByKey<ParameterPOCO>("parameters", key);
                ParameterPOCO result = await resultHandler.GetTaskCompletionSourceWrapper();
                return result.Value;
            }
            return null;
        }

        public async Task<string> GetParameterAsync(Parameters parameter, bool updateCache = false)
        {
            if (updateCache)
            {
                string value = await GetParameterAsync(parameter.ToString()) ?? "";
                await ParametersModel.SetPropertyByName(parameter.ToString(), value, false);
            }
            return ParametersModel.GetParamPropByName(parameter.ToString());
        }

        private async Task<bool> CheckHasParameterAsync(string key)
        {
            var resultHandler = await _db.GetCountFromObjectStoreByKey("parameters", key);
            int result = await resultHandler.GetTaskCompletionSourceWrapper();
            return result != 0;
        }

        public async Task<bool> CheckHasParameterAsync(Parameters parameter)
        {
            return await CheckHasParameterAsync(parameter.ToString());
        }

        internal async Task<bool> SetParameterAsync(string key, string value)
        {
            var resultHandler = await _db.SetKeyValueIntoObjectStore("parameters", key, value);
            bool result = await resultHandler.GetTaskCompletionSourceWrapper();
            return result;
        }
        public async Task<bool> SetParameterAsync(Parameters parameter, string value)
        {
            var ret = await SetParameterAsync(parameter.ToString(), value);
            OnChange?.Invoke(parameter, value);
            if (OnChangeAsync != null)
            {
                await OnChangeAsync(parameter, value);
            }
            return ret;
        }

        public async Task<Stream> ExportToJson()
        {
            var resultHandler = await _db.GetAllFromObjectStore<ParameterPOCO>("parameters");
            IEnumerable<ParameterPOCO> result = await resultHandler.GetTaskCompletionSourceWrapper();

            var jsonStream = new MemoryStream();
            await System.Text.Json.JsonSerializer.SerializeAsync(jsonStream, result);
            jsonStream.Position = 0;
            return jsonStream;
        }

        public async Task ImportFromStream(Stream stream)
        {
            await foreach (var param in System.Text.Json.JsonSerializer.DeserializeAsyncEnumerable<ParameterPOCO>(stream))
            {
                await SetParameterAsync(param.Key, param.Value);
            }
        }
    }
}
