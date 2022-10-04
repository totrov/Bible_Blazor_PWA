using Bible_Blazer_PWA.DataBase;
using Bible_Blazer_PWA.Facades;
using Bible_Blazer_PWA.Services.Parse;
using Bible_Blazer_PWA.Services.Readers;
using DocumentFormat.OpenXml.Features;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace Bible_Blazer_PWA.Services
{
    public class LessonImporter
    {
        #region events
        public event Action<ReaderException> OnReaderException;
        protected void ReaderExceptionHandler(ReaderException ex) => OnReaderException?.Invoke(ex);

        public event Action OnReadCompleted;
        protected void ReadCompletedHandler() => OnReadCompleted?.Invoke();

        public event Action OnImportCompleted;
        protected void ImportCompletedHandler() => OnImportCompleted?.Invoke();

        public event Action OnReadFinalization;
        protected void ReadFinalizationHandler() => OnReadFinalization?.Invoke();
        #endregion
        #region fields
        private readonly HttpFacade httpFacade;
        private readonly Corrector corrector;
        private readonly DatabaseJSFacade db;
        #endregion

        public LessonImporter(HttpClient http, Corrector corrector, DatabaseJSFacade db)
        {
            httpFacade = new HttpFacade(http);
            this.corrector = corrector;
            this.db = db;
        }

        public async Task LoadPredefinedLesson(string lessonName)
        {
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

            try
            {
                using (var reader = readerBuilder.GetReader())
                {
                    stringContent = reader.ReadDocumentToString(out readSucceeded);
                }
            }
            catch (ReaderException ex)
            {
                ReaderExceptionHandler(ex);
            }
            finally
            {
                ReadFinalizationHandler();
            }
            if (readSucceeded)
            {
                ReadCompletedHandler();
                string json = await LessonParser.ParseLessons(stringContent, corrector);
                var resultHandler = await db.ImportLessonsJson(json);
                resultHandler.OnDbResultOK += () => { ImportCompletedHandler(); };
            }
        }
    }
}
