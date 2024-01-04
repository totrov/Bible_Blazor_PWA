using Bible_Blazer_PWA.Components.Interactor.Home;
using Bible_Blazer_PWA.Static;
using MudBlazor;
using System;
using System.Collections.Generic;

namespace Bible_Blazer_PWA.Components.Interactor.Bible
{
    public class BibleChaptersInteractionModel : InteractionModelBase<BibleChaptersInteractionModel>
    {
        public override bool IsSide => true;

        public override bool ShouldPersistInHistory => false;

        public override Type ComponentType => typeof(BibleChaptersInteractionComponent);
        public string ShortName { get; set; }

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
                Text = "Библия",
                Action = () =>
                {
                    BibleInteractionModel.ApplyToCurrentPanel(this);
                },
                Icon = Constants.BibleIcon
            };

            yield return new BreadcrumbsFacade.BreadcrumbRecord
            {
                Text = ShortName,
                Action = () =>
                {
                    BibleChaptersInteractionModel.WithParameters<BibleBookShortName>
                        .ApplyToCurrentPanel(new(ShortName), this);
                },
                Icon = null
            };
        }

        public class BibleBookShortName : Parameters
        {
            public BibleBookShortName(string shortName)
            {
                ShortName = shortName;
            }

            public string ShortName { get; set; }
            public override void ApplyParametersToModel(BibleChaptersInteractionModel model)
            {
                model.ShortName = ShortName;
            }
        }
    }
}
