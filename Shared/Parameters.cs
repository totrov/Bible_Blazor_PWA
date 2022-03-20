using System;

namespace Bible_Blazer_PWA.Shared
{
    public class Parameters
    {
        private bool areReferencesOpened;

        public event EventHandler OnReferenceToggle;

        public int FontSize { get; set; }

        public bool AreReferencesOpened
        {
            get => areReferencesOpened;
            set { areReferencesOpened = value; HandleOnReferenceToggle(); }
        }
        public byte CollapseLevel { get; set; }

        protected virtual void HandleOnReferenceToggle()
        {
            EventHandler handler = OnReferenceToggle;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
