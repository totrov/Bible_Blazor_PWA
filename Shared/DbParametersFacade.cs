using Bible_Blazer_PWA.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bible_Blazer_PWA.Shared
{
    public class DbParametersFacade
    {
        public enum Parameters
        {
            FontSize,
            MainBackground,
            ToolsBg,
            HideTools,
            FirstLevelBg,
            FirstLevelBodyBg,
            FirstLevelMarginTop,
            FirstLevelFontWeight,
            SecondLevelBg,
            SecondLevelBodyBg,
            SecondLevelMarginTop,
            SecondLevelFontWeight,
            ThirdLevelBg,
            ThirdLevelBodyBg,
            ThirdLevelMarginTop,
            ThirdLevelFontWeight,
        }
        private readonly Dictionary<Parameters, string> ParameterConstants = new()
        {
            { Parameters.FontSize, "FontSize" },
            { Parameters.MainBackground, "MainBackground" },
            { Parameters.ToolsBg, "ToolsBg" },
            { Parameters.HideTools, "HideTools" },
            { Parameters.FirstLevelBg, "FirstLevelBg" },
            { Parameters.FirstLevelBodyBg, "FirstLevelBodyBg" },
            { Parameters.FirstLevelFontWeight, "FirstLevelFontWeight" },
            { Parameters.FirstLevelMarginTop, "FirstLevelMarginTop" },
            { Parameters.SecondLevelBg, "SecondLevelBg" },
            { Parameters.SecondLevelBodyBg, "SecondLevelBodyBg" },
            { Parameters.SecondLevelFontWeight, "SecondLevelFontWeight" },
            { Parameters.SecondLevelMarginTop, "SecondLevelMarginTop" },
            { Parameters.ThirdLevelBg, "ThirdLevelBg" },
            { Parameters.ThirdLevelBodyBg, "ThirdLevelBodyBg" },
            { Parameters.ThirdLevelFontWeight, "ThirdLevelFontWeight" },
            { Parameters.ThirdLevelMarginTop, "ThirdLevelMarginTop" }

        };

        private DatabaseJSFacade _db;
        public DbParametersFacade(DatabaseJSFacade db)
        {
            _db = db;
        }

        public async void InitDefaults()
        {
            if (!await CheckHasParameterAsync(Parameters.FontSize))
            {
                await SetParameterAsync(Parameters.FontSize, "14");
            }
        }

        class ParameterModel
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }

        private async Task<string> GetParameterAsync(string key)
        {
            if (await CheckHasParameterAsync(key))
            {
                var resultHandler = await _db.GetRecordFromObjectStoreByKey<ParameterModel>("parameters", key);
                ParameterModel result = await resultHandler.GetTaskCompletionSourceWrapper();
                return result.Value;
            }
            return null;
        }

        public async Task<string> GetParameterAsync(Parameters parameter)
        {
            return await GetParameterAsync(ParameterConstants[parameter]);
        }

        private async Task<bool> CheckHasParameterAsync(string key)
        {
            var resultHandler = await _db.GetCountFromObjectStoreByKey("parameters", key);
            int result = await resultHandler.GetTaskCompletionSourceWrapper();
            return result != 0;
        }

        public async Task<bool> CheckHasParameterAsync(Parameters parameter)
        {
            return await CheckHasParameterAsync(ParameterConstants[parameter]);
        }

        private async Task<bool> SetParameterAsync(string key, string value)
        {
            var resultHandler = await _db.SetKeyValueIntoObjectStore("parameters", key, value);
            bool result = await resultHandler.GetTaskCompletionSourceWrapper();
            return result;
        }
        public async Task<bool> SetParameterAsync(Parameters parameter, string value)
        {
            return await SetParameterAsync(ParameterConstants[parameter], value);
        }
    }
}
