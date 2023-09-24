using System;

namespace Bible_Blazer_PWA.Components.Interactor.Home
{
    public class HomeInteractionModel : InteractionModelBase
    {
        public override bool IsSide => true;

        public override Type ComponentType => typeof(HomeInteractionComponent);

        public override event Action OnClose;
        public override void Close() => OnClose?.Invoke();
    }
}
