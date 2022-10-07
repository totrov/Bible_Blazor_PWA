using MudBlazor;
using System;

namespace Bible_Blazer_PWA.Services
{
    internal class SnackbarImportHandler : ALessonImportHandler
    {
        private readonly ISnackbar snackbar;

        public SnackbarImportHandler(Action finalizationAction, ISnackbar snackbar) : base(finalizationAction)
        {
            this.snackbar = snackbar;
        }

        protected override void Inform(string message, Severity severity = Severity.Info)
        {
            snackbar.Add(message, severity);
        }
    }
}