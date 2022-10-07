using Bible_Blazer_PWA.DataBase;
using Bible_Blazer_PWA.Facades;
using Bible_Blazer_PWA.Services.Parse;
using Bible_Blazer_PWA.Services.Readers;
using System.Net.Http;
using System.Threading.Tasks;

namespace Bible_Blazer_PWA.Services
{
    public class LessonImporter
    {
        #region fields
        private readonly HttpFacade httpFacade;
        private readonly Corrector corrector;
        private readonly DatabaseJSFacade db;
        private readonly ALessonImportHandler handler;
        #endregion

        public LessonImporter(HttpClient http, Corrector corrector, DatabaseJSFacade db, ALessonImportHandler handler)
        {
            httpFacade = new HttpFacade(http);
            this.corrector = corrector;
            this.db = db;
            this.handler = handler;
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
                string json = await LessonParser.ParseLessons(stringContent, corrector);
                var resultHandler = await db.ImportLessonsJson(json);
                resultHandler.OnDbResultOK += () => { handler.HandleImportCompleted(); };
                await resultHandler.GetTaskCompletionSourceWrapper();
            }
        }
    }
}
