using System;

namespace Bible_Blazer_PWA.Components.Interactor.Home
{
    public class HomeInteractionModel : InteractionModelBase<HomeInteractionModel>
    {
        public override bool IsSide => true;
        public override bool ShouldPersistInHistory => false;

        public override Type ComponentType => typeof(HomeInteractionComponent);
    }
}
