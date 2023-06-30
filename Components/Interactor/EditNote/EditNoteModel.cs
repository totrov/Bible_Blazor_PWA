using Bible_Blazer_PWA.Components.Interactor.RemoveNote;
using Bible_Blazer_PWA.ViewModels;
using BibleComponents;
using System;

namespace Bible_Blazer_PWA.Components.Interactor.EditNote
{
    public class EditNoteModel:InteractionModelBase
    {
        public LessonElementMediator ElelementForNoteAdding { get; set; }
        public NoteModel NoteModel { get; set; }
        public override event Action OnClose;
        public override void Close() => OnClose?.Invoke();
        public event Action<NoteModel> OnRemoveNote;
        public void RemoveNote(NoteModel noteModel) => OnRemoveNote?.Invoke(noteModel);
        public override bool IsSide => true;
        public override Type ComponentType => typeof(EditNoteInteractionComponent);

        public class Parameters:IInteractionModelParameters<EditNoteModel>
        {
            public LessonElementMediator ElelementForNoteAdding { get; set; }
            public NoteModel NoteModel { get; set; }
            public Parameters(LessonElementMediator elelementForNoteAdding, NoteModel noteModel)
            {
                ElelementForNoteAdding = elelementForNoteAdding;
                NoteModel = noteModel;
            }
            public void ApplyParametersToModel(EditNoteModel model)
            {
                model.NoteModel = NoteModel;
                model.ElelementForNoteAdding = ElelementForNoteAdding;
            }
        }
    }


}
