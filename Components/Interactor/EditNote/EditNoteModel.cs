using Bible_Blazer_PWA.Components.Interactor.RemoveNote;
using Bible_Blazer_PWA.ViewModels;
using BibleComponents;
using System;

namespace Bible_Blazer_PWA.Components.Interactor.EditNote
{
    public class EditNoteModel:IInteractionModel
    {
        public LessonElementMediator ElelementForNoteAdding { get; set; }
        public NoteModel NoteModel { get; set; }
        public event Action OnClose;
        public void Close() => OnClose?.Invoke();
        public event Action OnRemoveNote;
        public void RemoveNote() => OnRemoveNote?.Invoke();
        public bool IsBottom => true;
        public Type ComponentType => typeof(EditNoteInteractionComponent);
    }
}
