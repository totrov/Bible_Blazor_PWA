using BlazorWorker.WorkerCore;
using Microsoft.JSInterop;
using System.Threading.Tasks;

using Bible_Blazer_PWA.DataBase;
using Bible_Blazer_PWA.Services.Parse;
using System.Net.Http;
using System.IO;
using System.Text.Json;

namespace Bible_Blazer_PWA.Services
{
    public class LessonImportService
    {
        private readonly ICorrector corrector;
        private LessonImporter lessonImporter;
        private string lessonFileName;
        private FileStream file;

        public LessonImportService(
            IWorkerMessageService workerMessageService,
            HttpClient httpClient,
            ICorrector Corrector,
            IJSRuntime jSRuntime)
        {
            WorkerMessageService = workerMessageService;
            corrector = Corrector;
            JSRuntime = jSRuntime;
            DatabaseJSFacade databaseJSFacade = new DatabaseJSFacade();
            databaseJSFacade.SetJS(jSRuntime);
            InterProcessImportHandler handler = new(
                () => { if (!string.IsNullOrEmpty(lessonFileName)) File.Delete(lessonFileName); },
                workerMessageService);
            lessonImporter = new(httpClient, corrector, databaseJSFacade, handler);
        }

        public async Task<string> WriteBytesToLessonFile(string lessonFileName, string serializedBytes)
        {
            this.lessonFileName = lessonFileName;

            file = new FileStream(lessonFileName, FileMode.Create, FileAccess.Write);
            await file.WriteAsync(System.Convert.FromBase64String(serializedBytes));
            file.Close();

            return "workaround for bug in backgroud worker library. Return type is needed";
        }

        public async Task<string> LoadPredefinedLesson(string lessonName)
        {
            await lessonImporter.LoadPredefinedLesson(lessonName);
            return "workaround for bug in backgroud worker library. Return type is needed";
        }
        public async Task<string> LoadLessonFromFile()
        {
            await lessonImporter.LoadLessonFromFile(lessonFileName);
            return "workaround for bug in backgroud worker library. Return type is needed";
        }
        public void NotifyImportCompleted() => lessonImporter.LessonDbImportAwaiter.SetResult();
        public IWorkerMessageService WorkerMessageService { get; }
        public IJSRuntime JSRuntime { get; }
    }
}
