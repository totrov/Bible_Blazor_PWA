using System;

namespace Bible_Blazer_PWA.Components.Interactor.Creed
{
    public class CreedInteractionModel : InteractionModelBase<CreedInteractionModel>
    {
        public override bool IsSide => true;
        public override bool ShouldPersistInHistory => false;
        public override Type ComponentType => typeof(CreedInteractionComponent);
    }
}
