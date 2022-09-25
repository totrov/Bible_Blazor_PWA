using Bible_Blazer_PWA.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace Bible_Blazer_PWA.Facades
{
    public class HttpFacade
    {
        private readonly HttpClient client;
        public HttpFacade(HttpClient client)
        {
            this.client = client;
        }

        public async Task<Stream> GetStreamByLessonNameAsync(string lessonName)
        {
            Stream result;
            try
            {
                result = await client.GetStreamAsync(LessonLoadConfig.GetUrlByLessonName(lessonName, true));
            }
            catch(HttpRequestException)
            {
                result = await client.GetStreamAsync(LessonLoadConfig.GetUrlByLessonName(lessonName, false));
            }
            return result;
        }

        internal async Task<Dictionary<string, Dictionary<string, string>>> GetRepacementsFromJsonAsync()
        {
            Dictionary<string, Dictionary<string, string>> replacements;
            try
            {
                replacements = await client.GetFromJsonAsync<Dictionary<string, Dictionary<string, string>>>(
                    LessonLoadConfig.GetReplacementsUrl(true));
            }
            catch (Exception)
            {
                replacements = await client.GetFromJsonAsync<Dictionary<string, Dictionary<string, string>>>(
                    LessonLoadConfig.GetReplacementsUrl(false));
            }
            return replacements;
        }
    }
}
