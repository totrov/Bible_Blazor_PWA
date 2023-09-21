using System;

namespace Bible_Blazer_PWA.Components.Interactor.Creed
{
    public class CreedInteractionModel : InteractionModelBase
    {
        public override bool IsSide => true;

        public override Type ComponentType => typeof(CreedInteractionComponent);

        public override event Action OnClose;
        public override void Close() => OnClose?.Invoke();
    }
}
