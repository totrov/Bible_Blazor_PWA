﻿using Bible_Blazer_PWA.Services.Readers;
using MudBlazor;
using System;

namespace Bible_Blazer_PWA.Services
{
    public abstract class ALessonImportHandler
    {
        private readonly Action finalizationAction;

        public ALessonImportHandler(Action finalizationAction)
        {
            this.finalizationAction = finalizationAction;
        }
        internal void HandleImportCompleted(string lessonName)
        {
            Inform($"{lessonName}: Успешно загружено", Severity.Success);
        }

        public virtual void HandleReadCompleted(string lessonName)
        {
            Inform($"{lessonName}: Разбор текста...");
        }

        internal void HandleReaderException(ReaderException ex)
        {
            Inform(ex.Message, Severity.Error);
        }

        public virtual void HandleReadFinalization()
        {
            finalizationAction?.Invoke();
        }

        protected abstract void Inform(string message, Severity severity = Severity.Info);
        public virtual void HandleStartReading(string lessonName)
        {
            Inform($"{lessonName}:Чтение...");
        }
        public virtual  void HandleStringAfterSuccessfulReading(string stringContent) { }
    }
}