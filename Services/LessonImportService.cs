using BlazorWorker.WorkerCore;
using Microsoft.JSInterop;
using System.Threading.Tasks;

using Bible_Blazer_PWA.DataBase;
using Bible_Blazer_PWA.Services.Parse;
using System.Net.Http;

namespace Bible_Blazer_PWA.Services
{
    public class LessonImportService
    {
        private readonly ICorrector corrector;
        private LessonImporter lessonImporter;

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
            InterProcessImportHandler handler = new(() => { workerMessageService.PostMessageAsync("IReadCompleted"); }, workerMessageService);
            lessonImporter = new(httpClient, corrector, databaseJSFacade, handler);
            workerMessageService.PostMessageAsync("Iinitialized");
        }
        public async Task LoadPredefinedLesson(string lessonName) => await lessonImporter.LoadPredefinedLesson(lessonName);
        public async Task LoadLessonFromFile(string fileName) => await lessonImporter.LoadLessonFromFile(fileName);
        public IWorkerMessageService WorkerMessageService { get; }
        public IJSRuntime JSRuntime { get; }
    }
}
