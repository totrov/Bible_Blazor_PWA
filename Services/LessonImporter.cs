using Bible_Blazer_PWA.DataBase;
using Bible_Blazer_PWA.Facades;
using Bible_Blazer_PWA.Services.Parse;
using Bible_Blazer_PWA.Services.Readers;
using BlazorWorker.WorkerCore;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Bible_Blazer_PWA.Services
{
    public class LessonImporter
    {
        #region fields
        private readonly HttpFacade httpFacade;
        private readonly ICorrector corrector;
        private readonly DatabaseJSFacade db;
        private readonly ALessonImportHandler handler;
        private string lessonName;
        #endregion
        public TaskCompletionSource LessonDbImportAwaiter { get; private set; }
        public LessonImporter(HttpClient http, ICorrector corrector, DatabaseJSFacade db, ALessonImportHandler handler)
        {
            httpFacade = new HttpFacade(http);
            this.corrector = corrector;
            this.db = db;
            this.handler = handler;
        }

        public async Task LoadPredefinedLesson(string lessonName)
        {
            this.lessonName = lessonName;
            var readerBuilder = new ReaderBuilder(await httpFacade.GetStreamByLessonNameAsync(lessonName));
            await LoadLesson(readerBuilder);
        }

        public async Task LoadLessonFromFile(string fileName)
        {
            var readerBuilder = new ReaderBuilder(fileName);
            await LoadLesson(readerBuilder);
        }

        protected async Task LoadLesson(ReaderBuilder readerBuilder)
        {
            string stringContent = "";
            bool readSucceeded = false;
            handler.HandleStartReading();

            try
            {
                using (var reader = readerBuilder.GetReader())
                {
                    stringContent = reader.ReadDocumentToString(out readSucceeded);
                }
            }
            catch (ReaderException ex)
            {
                handler.HandleReaderException(ex);
            }
            finally
            {
                handler.HandleReadFinalization();
            }
            if (readSucceeded)
            {
                handler.HandleReadCompleted();
                foreach (LessonModel lesson in LessonParser.ParseLessons(stringContent, corrector))
                {
                    await db.ImportLessonsJson(await ConvertLessonToJSON(lesson));
                    LessonDbImportAwaiter = new TaskCompletionSource();
                    await LessonDbImportAwaiter.Task;
                }
            }
            handler.HandleImportCompleted(lessonName);
        }
        private static async Task<string> ConvertLessonToJSON(LessonModel lessonModel)
        {
            using MemoryStream memoryStream = new MemoryStream();
            await JsonSerializer.SerializeAsync(memoryStream, lessonModel, new JsonSerializerOptions() { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
            memoryStream.Position = 0;
            using StreamReader sr = new(memoryStream);
            string result = sr.ReadToEnd();
            return result;
        }
    }
}
