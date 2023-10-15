using System;

namespace Bible_Blazer_PWA.Components.Interactor.Bible
{
    public class BibleInteractionModel : InteractionModelBase<BibleInteractionModel>
    {
        public override bool IsSide => true;

        public override bool ShouldPersistInHistory => false;

        public override Type ComponentType => typeof(BibleInteractionComponent);
    }
}
