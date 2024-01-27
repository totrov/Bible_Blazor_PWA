using System;

namespace Bible_Blazer_PWA.Services.Menu
{
    public interface IButtonVisibilityHandler
    {
        bool IsVisible { get; }
    }

    public class DefaultVisibilityHandler : IButtonVisibilityHandler
    {
        public bool IsVisible => true;
    }
    public class CustomVisibilityHandler : IButtonVisibilityHandler
    {
        private readonly Func<bool> handler;

        public CustomVisibilityHandler(Func<bool> handler)
        {
            this.handler = handler;
        }
        public bool IsVisible => handler();
    }
    
}