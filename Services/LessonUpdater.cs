using Bible_Blazer_PWA.DataBase;
using MudBlazor;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bible_Blazer_PWA.Services
{
    public class LessonUpdater
    {
        private readonly DatabaseJSFacade db;

        public LessonUpdater(DatabaseJSFacade db)
        {
            this.db = db;
        }
        public async Task UpdateLessons()
        {
            await ClearObjectStores("lessons", "lessonElementData", "cache");

            var url = "https://covenantofchrist.onrender.com/Assets/online/lessons/lessons.json";
            await db.ImportJsonByURL(url, "lessons");
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
