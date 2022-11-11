using Bible_Blazer_PWA.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Bible_Blazer_PWA.Facades
{
    public class HttpFacade
    {
        private readonly HttpClient client;
        public HttpFacade(HttpClient client)
        {
            this.client = client;
        }

        public async Task<DateTime> GetVersionDateAsync()
        {
            string manifestString = await client.GetStringAsync(
                LessonLoadConfig.GetManifestUrl());
            if (!string.IsNullOrEmpty(manifestString))
            {
                return JsonSerializer.Deserialize<LessonStorageManifestDTO>(manifestString).lastUpdateDate;
            }

            return LessonLoadConfig.GetOfflineVersionDate();
        }

        public async Task<(Stream, DateTime)> GetStreamByLessonNameAsync(string lessonName)
        {
            return (await GetOnlineFirst(
                async (url) => await client.GetStreamAsync(url),
                online => LessonLoadConfig.GetUrlByLessonName(lessonName, online)
                ), await GetVersionDateAsync());
        }

        internal async Task<List<string>> GetNegativeLookaheads()
        {
            return await GetOnlineFirst(
                async (url) => await client.GetFromJsonAsync<List<string>>(url),
                LessonLoadConfig.GetNegativeLookaheadsUrl
                );
        }

        internal async Task<Dictionary<string, Dictionary<string, string>>> GetRepacementsFromJsonAsync()
        {
            return await GetOnlineFirst(
                async (url) => await client.GetFromJsonAsync<Dictionary<string, Dictionary<string, string>>>(url),
                LessonLoadConfig.GetReplacementsUrl
                );
        }

        delegate string UrlGetterMethod(bool online);
        private async Task<T> GetOnlineFirst<T>(Func<string, Task<T>> func, UrlGetterMethod urlGetterMethod)
        {
            T result;
            try
            {
                result = await func(urlGetterMethod(true));
            }
            catch (HttpRequestException)
            {
                result = await func(urlGetterMethod(false));
            }
            return result;
        }
    }
}
