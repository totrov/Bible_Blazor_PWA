using System;

namespace Bible_Blazer_PWA.Components.Interactor.LessonUnits
{
    public class LessonUnitsInteractionModel : InteractionModelBase<LessonUnitsInteractionModel>
    {
        public override bool IsSide => true;
        public override bool ShouldPersistInHistory => false;

        public override Type ComponentType => typeof(LessonUnitsInteractionComponent);
    }
}
