using System;

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
