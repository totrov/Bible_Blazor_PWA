using Bible_Blazer_PWA.Components.Interactor.Home;
using MudBlazor;
using System;
using System.Collections.Generic;

namespace Bible_Blazer_PWA.Components.Interactor.Creed
{
    public class CreedInteractionModel : InteractionModelBase<CreedInteractionModel>
    {
        public override bool IsSide => true;
        public override bool ShouldPersistInHistory => false;
        public override Type ComponentType => typeof(CreedInteractionComponent);
        public override string Background => "beige";
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
                Text = "Символ Веры",
                Action = () =>
                {
                    CreedInteractionModel.ApplyToCurrentPanel(this);
                },
                Icon = null
            };
        }
    }
}
