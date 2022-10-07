using Bible_Blazer_PWA.Parameters;
using Bible_Blazer_PWA.Services.Menu;
using System;

namespace Bible_Blazer_PWA.Pages.Lesson
{
    public abstract class GeneralHandler : IconResolvableHandlerWithPostHandler
    {
        protected readonly ParametersModel parameters;
        public GeneralHandler(IconResolver iconResolver, Action postHandleAction, ParametersModel parameters) : base(iconResolver, postHandleAction)
        {
            this.parameters = parameters;
        }
        protected abstract void HandleInternal(int state);
        public override void Handle(int state)
        {
            HandleInternal(state);
            postHandleAction?.Invoke();
        }
    }
}
