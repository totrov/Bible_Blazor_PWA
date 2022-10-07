using MudBlazor;
using System;

namespace Bible_Blazer_PWA.Services
{
    internal class OnPageImportHandler : ALessonImportHandler
    {
        private readonly LoadingItem item;

        public OnPageImportHandler(Action finalizationAction, LoadingItem item) : base(finalizationAction)
        {
            this.item = item;
        }

        protected override void Inform(string message, Severity severity = Severity.Info)
        {
            item.Status = message;
        }
    }
}