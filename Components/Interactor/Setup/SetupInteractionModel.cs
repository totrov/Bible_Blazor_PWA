using Bible_Blazer_PWA.Components.Interactor.Home;
using Bible_Blazer_PWA.Components.Interactor.LessonUnits;
using Bible_Blazer_PWA.Components.Interactor.RemoveNote;
using MudBlazor;
using System;
using System.Collections.Generic;

namespace Bible_Blazer_PWA.Components.Interactor.Setup
{
    public class SetupInteractionModel : InteractionModelBase<SetupInteractionModel>
    {
        public override bool IsSide => true;
        public override bool ShouldPersistInHistory => false;
        public override Type ComponentType => typeof(SetupInteractionComponent);

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
                Icon = null
            };
        }
    }
}
