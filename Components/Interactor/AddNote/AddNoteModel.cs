using BibleComponents;
using System;

namespace Bible_Blazer_PWA.Components.Interactor.AddNote
{
    public class AddNoteModel : IInteractionModel
    {
        public LessonElementMediator ElelementForNoteAdding { get; set; }
        public event Action OnClose;
        public Type ComponentType => typeof(AddNoteInteractionComponent);
        public bool IsBottom { get => false; }
        public void Close() => OnClose?.Invoke();
    }
}
