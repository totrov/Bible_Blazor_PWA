using BibleComponents;
using System;

namespace Bible_Blazer_PWA.Components.Interactor.BibleReferencesWriter
{
    public class BibleReferencesWriterInreractionModel : IInteractionModel
    {
        public bool IsBottom => true;

        public Type ComponentType => typeof(BibleReferencesWriterInteractionComponent);

        public event Action OnClose;
        public void Close() => OnClose?.Invoke();
        public void MouseLeave() { if (!Overflowed) OnClose?.Invoke(); }
        public LessonElementMediator Mediator { get; set; }
        public int ReferenceNumber { get; set; }
        public bool Overflowed { get; set; } = false;
    }
}
