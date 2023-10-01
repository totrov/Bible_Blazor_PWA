using Bible_Blazer_PWA.DataBase;
using Bible_Blazer_PWA.DataBase.DTO;
using Bible_Blazer_PWA.Facades;
using Bible_Blazer_PWA.Services.Parse;
using Bible_Blazer_PWA.Services.Readers;
using System;
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
            (var stream, DateTime versionDate) = await httpFacade.GetStreamByLessonNameAsync(lessonName);
            var readerBuilder = new ReaderBuilder(stream);
            await LoadLesson(readerBuilder, lessonName, versionDate);
        }

        public async Task LoadLessonFromFile(string fileName, DateTime dateTime)
        {
            await db.ClearObjectStore(Static.Constants.CacheObjectStoreName);
            var readerBuilder = new ReaderBuilder(fileName);
            await LoadLesson(readerBuilder, fileName, dateTime);
        }

        protected async Task LoadLesson(ReaderBuilder readerBuilder, string lessonName, DateTime versionDate)
        {
            string stringContent = "";
            bool readSucceeded = false;
            handler.HandleStartReading(lessonName);

            try
            {
                using (var reader = readerBuilder.GetReader())
                {
                    stringContent = reader.ReadDocumentToString(out readSucceeded);
                    if (readSucceeded)
                        handler.HandleStringAfterSuccessfulReading(stringContent);
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
                handler.HandleReadCompleted(lessonName);
                foreach (LessonDTO lesson in LessonParser.ParseLessons(stringContent, corrector, versionDate))
                {
                    await db.ImportJson(await ConvertLessonToJSON(lesson), "lessons");
                    LessonDbImportAwaiter = new TaskCompletionSource();
                    //await LessonDbImportAwaiter.Task;
                }
            }
            handler.HandleImportCompleted(lessonName);
        }
        private static async Task<string> ConvertLessonToJSON(LessonDTO lessonModel)
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
