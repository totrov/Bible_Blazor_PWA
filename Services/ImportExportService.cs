using Bible_Blazer_PWA.DataBase;
using Bible_Blazer_PWA.DataBase.DTO;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;
using static Bible_Blazer_PWA.Parameters.DbParametersFacade;

namespace Bible_Blazer_PWA.Services
{
    public class ImportExportService
    {
        private readonly DatabaseJSFacade db;
        private readonly IJSRuntime JS;

        public ImportExportService(DatabaseJSFacade db, IJSRuntime JS)
        {
            this.db = db;
            this.JS = JS;
        }
        public async Task ExportToJSON<T>(string objectStoreName, bool camelCase = false)
        {
            using var stream = await CreateJsonStream<T>(objectStoreName, camelCase);
            using var streamRef = new DotNetStreamReference(stream);
            await JS.InvokeVoidAsync("downloadFileFromStream", $"{objectStoreName}.json", streamRef);
        }

        public async Task<Stream> CreateJsonStream<T>(string objectStoreName, bool camelCase = false)
        {
            var resultHandler = await db.GetAllFromObjectStore<T>(objectStoreName);
            IEnumerable<T> result = await resultHandler.GetTaskCompletionSourceWrapper();

            var jsonStream = new MemoryStream();
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            if (camelCase)
            {
                options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            }
            await JsonSerializer.SerializeAsync(jsonStream, result, options);
            jsonStream.Position = 0;
            return jsonStream;
        }

        public async Task ImportFromStream(Stream stream, string objectStoreName)
        {
            string json = string.Empty;
            using (StreamReader reader = new(stream))
            {
                json = await reader.ReadToEndAsync();
            }

            if (!string.IsNullOrEmpty(json))
            {
                await db.ImportJson(json, objectStoreName);
            }
        }
    }
}
