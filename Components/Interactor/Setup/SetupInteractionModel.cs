﻿using Bible_Blazer_PWA.Components.Interactor.RemoveNote;
using System;

namespace Bible_Blazer_PWA.Components.Interactor.Setup
{
    public class SetupInteractionModel : InteractionModelBase
    {
        public override bool IsSide => true;

        public override Type ComponentType => typeof(SetupInteractionComponent);

        public override event Action OnClose;

        public override void Close() => OnClose?.Invoke();
        public class Parameters : IInteractionModelParameters<SetupInteractionModel>
        {
            public void ApplyParametersToModel(SetupInteractionModel model)
            {
                //TODO
            }
        }
    }
}