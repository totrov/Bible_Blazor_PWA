using Bible_Blazer_PWA.Components.Interactor.AddNote;
using Bible_Blazer_PWA.ViewModels;
using BibleComponents;
using System;

namespace Bible_Blazer_PWA.Components.Interactor.RemoveNote
{
    public class RemoveNoteModel : IInteractionModel
    {
        public NoteModel NoteModel { get; set; }
        public event Action OnClose;
        public void Close() => OnClose?.Invoke();
        public event Action OnRemoveCompleted;
        public void HandleRemoveCompleted() => OnRemoveCompleted?.Invoke();
        public bool IsBottom => false;
        public Type ComponentType => typeof(RemoveNoteInteractionComponent);
    }
}
