using Bible_Blazer_PWA.Components.Interactor.Home;
using MudBlazor;
using System;
using System.Collections.Generic;

namespace Bible_Blazer_PWA.Components.Interactor.LessonUnits
{
    public class LessonUnitsInteractionModel : InteractionModelBase<LessonUnitsInteractionModel>
    {
        public override bool IsSide => true;
        public override bool ShouldPersistInHistory => false;

        public override Type ComponentType => typeof(LessonUnitsInteractionComponent);

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
                Text = "Планы уроков",
                Action = () =>
                {
                    LessonUnitsInteractionModel.ApplyToCurrentPanel(this);
                },
                Icon = null
            };
        }
    }
}
