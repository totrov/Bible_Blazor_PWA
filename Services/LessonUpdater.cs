using Bible_Blazer_PWA.DataBase;
using Bible_Blazer_PWA.Facades;
using BibleComponents;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Bible_Blazer_PWA.DataSources.LessonDS;
using static System.Net.WebRequestMethods;

namespace Bible_Blazer_PWA.Services
{
    public class LessonUpdater
    {
        private readonly DatabaseJSFacade db;
        private readonly ISnackbar snackbar;
        private readonly HttpFacade http;

        public LessonUpdater(DatabaseJSFacade db, ISnackbar snackbar, HttpFacade http)
        {
            this.db = db;
            this.snackbar = snackbar;
            this.http = http;
        }

        public async Task<bool> CheckUpdateRequired(SortedDictionary<string, LessonBlock> blocks)
        {
            if (blocks.Any(b => b.Value.Lessons.Count == 0))
                return true;
            var minimalVersion = await http.GetVersionDateAsync();
            var oudatedLessonBlocks = blocks
                .Where(b => b.Value.Lessons.Any(l => l.Value.VersionDate < minimalVersion || l.Value.VersionDate == DateTime.MaxValue))
                .Select(block => block.Value.Name).ToList();
            return oudatedLessonBlocks.Any();
        }

        public async Task UpdateLessons()
        {
            await ClearObjectStores("lessons", "lessonElementData", "cache");

            var url = "https://covenantofchrist.onrender.com/Assets/online/lessons/lessons.json";
            await db.ImportJsonByURL(url, "lessons");
            snackbar.Add("Уроки обновлены");
        }

        private async Task ClearObjectStores(params string[] objectSotres)
        {
            List<Task> tasksToAwait = new();
            foreach (var objectStore in objectSotres)
            {
                tasksToAwait.Add((await db.ClearObjectStore(objectStore)).GetTaskCompletionSourceWrapper());
            }
            await Task.WhenAll(tasksToAwait);
        }
    }
}
