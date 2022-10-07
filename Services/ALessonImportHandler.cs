using Bible_Blazer_PWA.Services.Readers;
using MudBlazor;
using System;
using System.Threading.Tasks;

namespace Bible_Blazer_PWA.Services
{
    public abstract class ALessonImportHandler
    {
        private readonly Action finalizationAction;

        public ALessonImportHandler(Action finalizationAction)
        {
            this.finalizationAction = finalizationAction;
        }
        internal void HandleImportCompleted()
        {
            Inform("Успешно загружено", Severity.Success);
        }

        internal void HandleReadCompleted()
        {
            Inform("Чтение завершено. Разбор текста...");
        }

        internal void HandleReaderException(ReaderException ex)
        {
            Inform(ex.Message, Severity.Error);
        }

        internal void HandleReadFinalization()
        {
            finalizationAction?.Invoke();
        }

        protected abstract void Inform(string message, Severity severity = Severity.Info);
        internal void HandleStartReading()
        {
            Inform("Чтение из облака...");
        }
    }
}