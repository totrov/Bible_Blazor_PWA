using Bible_Blazer_PWA.Components.Interactor.Creed;
using Bible_Blazer_PWA.Components.Interactor.Home;
using Bible_Blazer_PWA.Components.Interactor.LessonUnits;
using MudBlazor;
using System;
using System.Collections.Generic;

namespace Bible_Blazer_PWA.Components.Interactor.LessonsInUnit
{
    public partial class Interaction
    {
        public class LessonsInUnitInteractionModel : InteractionModelBase<LessonsInUnitInteractionModel>
        {
            public override bool IsSide => true;

            public override bool ShouldPersistInHistory => false;

            public override Type ComponentType => typeof(LessonsInUnitInteractionComponent);

            #region Parameters

            public string UnitId { get; set; }

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

                yield return new BreadcrumbsFacade.BreadcrumbRecord
                {
                    Text = UnitId,
                    Action = () =>
                    {
                        LessonsInUnitInteractionModel.WithParameters<SelectedUnitId>
                            .ApplyToCurrentPanel(new(UnitId), this);
                    },
                    Icon = null
                };
            }

            public class SelectedUnitId : Parameters
            {
                public string UnitId { get; set; }
                public SelectedUnitId(string unitName)
                {
                    UnitId = unitName;
                }
                public override void ApplyParametersToModel(LessonsInUnitInteractionModel model)
                {
                    model.UnitId = UnitId;
                }
            }
            #endregion
        }
    }
}
