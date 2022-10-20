using BlazorWorker.WorkerCore;
using MudBlazor;
using System;

namespace Bible_Blazer_PWA.Services
{
    public class InterProcessImportHandler : ALessonImportHandler
    {
        private readonly IWorkerMessageService workerMessageService;

        public InterProcessImportHandler(Action finalizationAction, IWorkerMessageService workerMessageService) : base(finalizationAction)
        {
            this.workerMessageService = workerMessageService;
        }

        protected override void Inform(string message, Severity severity = Severity.Info)
        {
            string msg = severity switch
            {     
                Severity.Info => "INFO                ",
                Severity.Warning => "WARNING             ",
                Severity.Error => "ERROR               ",
                Severity.Normal => "MESSAGE             ",
                Severity.Success => "SUCCESS             ",
                _ => throw new Exception($"{severity.ToString()} is not expected")
            } + message;
            workerMessageService.PostMessageAsync(msg);
        }
    }
}
