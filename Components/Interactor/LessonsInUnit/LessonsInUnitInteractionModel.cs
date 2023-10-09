using System;

namespace Bible_Blazer_PWA.Components.Interactor.LessonsInUnit
{
    public partial class Interaction
    {
        public class LessonsInUnitInteractionModel : InteractionModelBase<LessonsInUnitInteractionModel>
        {
            public override bool IsSide => false;

            public override bool ShouldPersistInHistory => false;

            public override Type ComponentType => typeof(LessonsInUnitInteractionComponent);

            public override event Action OnClose;
            public override void Close() => OnClose?.Invoke();

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
