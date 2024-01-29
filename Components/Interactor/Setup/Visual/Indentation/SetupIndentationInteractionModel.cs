using Bible_Blazer_PWA.Components.Interactor.Home;
using Bible_Blazer_PWA.Components.Interactor.Setup.Visual.Indentation;
using MudBlazor;
using System;
using System.Collections.Generic;

namespace Bible_Blazer_PWA.Components.Interactor.Setup.Visual
{
    public class SetupIndentationInteractionModel : InteractionModelBase<SetupIndentationInteractionModel>
    {
        public override bool IsSide => true;

        public override bool ShouldPersistInHistory => true;

        public override Type ComponentType => typeof(SetupIndentationInteractionComponent);

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
                Text = "Отступы",
                Action = () =>
                {
                    SetupIndentationInteractionModel.ApplyToCurrentPanel(this);
                },
                Icon = null
            };
        }
    }
}
