using Bible_Blazer_PWA.DataBase;
using Bible_Blazer_PWA.DataBase.DTO;
using Bible_Blazer_PWA.DataSources;
using Bible_Blazer_PWA.Facades;
using Bible_Blazer_PWA.Services.Parse;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Bible_Blazer_PWA.DataSources.LessonDS;

namespace Bible_Blazer_PWA.Services
{
    public class LessonKeeper:IAsyncInitializable
    {
        private readonly DatabaseJSFacade db;
        private readonly ISnackbar snackbar;
        private readonly HttpFacade http;
        private LessonDS _lessonDS { get; set; }
        private Task _initTask;
        private DateTime? lastUpdateDate = null;

        public SortedDictionary<string, LessonBlock> LessonsUnits { get; set; }
        public DateTime LastUpdate => lastUpdateDate ??= GetLastUpdate();
        public event Action<DateTime> LastUpdateDateChanged;
        protected void OnLastUpdateDateChanged(DateTime date) => LastUpdateDateChanged?.Invoke(date);

        private DateTime GetLastUpdate()
        {
            if (!LessonsUnits.Any())
                return DateTime.MinValue;

            Func<SortedDictionary<int, LessonLightweightDTO>, DateTime> getMinDateFromLessons = lessons => 
                lessons.Any() ? lessons.Min(l => l.Value.VersionDate) : DateTime.MinValue;

            return LessonsUnits.Aggregate(DateTime.MaxValue, (date, lu)
                => new DateTime(Math.Min(date.Ticks, getMinDateFromLessons(lu.Value.Lessons).Ticks)));
        }

        public LessonKeeper(DatabaseJSFacade db, ISnackbar snackbar, HttpFacade http)
        {
            this.db = db;
            this.snackbar = snackbar;
            this.http = http;

            _lessonDS = new LessonDS(db);
            _initTask = FetchLessonsUnitsFromDb();
        }

        private async Task FetchLessonsUnitsFromDb()
        {
            LessonsUnits = await _lessonDS.GetBlocks(new DBCache(db), true);
        }

        public async Task RefreshAsync()
        {
            await FetchLessonsUnitsFromDb();
            DateTime newLastUpdate = GetLastUpdate();
            if (newLastUpdate != lastUpdateDate)
            {
                OnLastUpdateDateChanged((lastUpdateDate = newLastUpdate).Value);
            }           
        }

        public LessonBlock this[string lessonUnitId]
        {
            get => LessonsUnits[lessonUnitId];
        }

        public Task InitTask => _initTask;

        public async Task<bool> CheckUpdateRequired()
        {
            if (LessonsUnits.Any(b => b.Value.Lessons.Count == 0))
                return true;
            var minimalVersion = await http.GetVersionDateAsync();
            var oudatedLessonBlocks = LessonsUnits
                .Where(b => b.Value.Lessons.Any(l => l.Value.VersionDate < minimalVersion || l.Value.VersionDate == DateTime.MaxValue))
                .Select(block => block.Value.Name).ToList();
            return oudatedLessonBlocks.Any();
        }

        public async Task UpdateLessons()
        {
            await ClearObjectStores("lessons", "lessonElementData", "cache");

            var url = "https://covenantofchrist.onrender.com/Assets/online/lessons/lessons.json";
            await db.ImportJsonByURL(url, "lessons");
            await RefreshAsync();
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
