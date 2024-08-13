using Bible_Blazer_PWA.Components.Interactor.Home;
using MudBlazor;
using System;
using System.Collections.Generic;

namespace Bible_Blazer_PWA.Components.Interactor.Notes
{
    public class NotesInteractionModel : InteractionModelBase<NotesInteractionModel>
    {
        public override bool IsSide => false;

        public override bool ShouldPersistInHistory => true;

        public override Type ComponentType => typeof(NotesInteractionComponent);

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
                Text = "",
                Action = () =>
                {
                    NotesInteractionModel.ApplyToCurrentPanel(this);
                },
                Icon = Icons.Material.Filled.EditNote
            };
        }
    }
}
