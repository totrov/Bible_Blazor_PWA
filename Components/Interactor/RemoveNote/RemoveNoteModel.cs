using Bible_Blazer_PWA.ViewModels;
using BibleComponents;
using System;

namespace Bible_Blazer_PWA.Components.Interactor.RemoveNote
{
    public class RemoveNoteModel : Command
    {
        public NoteModel NoteModel { get; set; }
        public LessonElementMediator Mediator { get; set; }
        public override event Action OnClose;
        public override void Close() => OnClose?.Invoke();
        public override bool IsSide => false;
        public override Type ComponentType => typeof(RemoveNoteInteractionComponent);

        public class Parameters:IInteractionModelParameters<RemoveNoteModel>
        {
            public NoteModel NoteModel { get; }
            public LessonElementMediator Mediator { get; set; }
            public Parameters(NoteModel noteModel, LessonElementMediator mediator)
            {
                NoteModel = noteModel;
                Mediator = mediator;
            }

            public void ApplyParametersToModel(RemoveNoteModel model)
            {
                model.Mediator = Mediator;
                model.NoteModel = NoteModel;
            }
        }
    }
}
