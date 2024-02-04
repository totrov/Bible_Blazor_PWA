using MudBlazor;
using System;
using System.Collections.Generic;

namespace Bible_Blazer_PWA.Components.Interactor.Home
{
    public class HomeInteractionModel : InteractionModelBase<HomeInteractionModel>
    {
        public override bool IsSide => true;
        public override bool ShouldPersistInHistory => false;

        public override Type ComponentType => typeof(HomeInteractionComponent);

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
        }
    }
}
