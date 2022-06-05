using System;

namespace Bible_Blazer_PWA.Services.Menu
{
    public abstract class IconResolvableHandlerWithPostHandler : IButtonStateHandler
    {
        private protected IconResolver iconResolver;
        protected readonly Action postHandleAction;

        internal IconResolvableHandlerWithPostHandler(IconResolver iconResolver, Action postHandleAction)
        {
            this.iconResolver = iconResolver;
            this.postHandleAction = postHandleAction;
        }

        public abstract int StatesCount { get; }
        public abstract string Key { get; }

        public abstract string GetIcon(int state);

        public abstract void Handle(int state);
    }
}
