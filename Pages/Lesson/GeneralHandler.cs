﻿using Bible_Blazer_PWA.Components.Interactor;
using Bible_Blazer_PWA.Parameters;
using Bible_Blazer_PWA.Services.Menu;
using System;

namespace Bible_Blazer_PWA.Pages.Lesson
{
    public abstract class GeneralHandler : IconResolvableHandlerWithPostHandler
    {
        protected ParametersModel parameters { get => Interaction.GetParameters(); }
        public GeneralHandler(IconResolver iconResolver, Action postHandleAction) : base(iconResolver, postHandleAction) { }

        protected abstract void HandleInternal(int state);
        public override void Handle(int state)
        {
            HandleInternal(state);
            postHandleAction?.Invoke();
        }
    }
}
