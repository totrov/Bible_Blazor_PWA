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
                Severity.Info => "I",
                Severity.Warning => "W",
                Severity.Error => "E",
                Severity.Normal => "N",
                Severity.Success => "S",
                _ => "Z",
            } + message;
            workerMessageService.PostMessageAsync(msg);
        }
    }
}
