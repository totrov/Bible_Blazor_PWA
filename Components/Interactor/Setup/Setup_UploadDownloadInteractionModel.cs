using Bible_Blazer_PWA.Components.Interactor.Home;
using Bible_Blazer_PWA.Components.Interactor.Setup.Visual;
using MudBlazor;
using System;
using System.Collections.Generic;

namespace Bible_Blazer_PWA.Components.Interactor.Setup
{
    public class Setup_UploadDownloadInteractionModel : InteractionModelBase<Setup_UploadDownloadInteractionModel>
    {
        public override bool IsSide => true;

        public override bool ShouldPersistInHistory => true;

        public override Type ComponentType => typeof(Setup_UploadDownloadInteractionComponent);

        public override IEnumerable<BreadcrumbsFacade.BreadcrumbRecord> GetBreadcrumbs()
        {
            yield return new BreadcrumbsFacade.BreadcrumbRecord
            {
                Text = "",
                Action = () =>
                {
                    HomeInteractionModel.ApplyToCurrentPanel(this);
                },
                Icon = Icons.Material.Filled.Home
            };

            yield return new BreadcrumbsFacade.BreadcrumbRecord
            {
                Text = "Настройки",
                Action = () =>
                {
                    SetupInteractionModel.ApplyToCurrentPanel(this);
                },
                Icon = @Icons.Material.Filled.PhonelinkSetup
            };

            yield return new BreadcrumbsFacade.BreadcrumbRecord
            {
                Text = "Скачать/загрузить",
                Action = () =>
                {
                    Setup_UploadDownloadInteractionModel.ApplyToCurrentPanel(this);
                },
                Icon = null
            };
        }
    }
}