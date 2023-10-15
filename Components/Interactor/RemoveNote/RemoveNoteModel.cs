using Bible_Blazer_PWA.ViewModels;
using BibleComponents;
using System;

namespace Bible_Blazer_PWA.Components.Interactor.RemoveNote
{
    public class RemoveNoteModel : Command<RemoveNoteModel>
    {
        public NoteModel NoteModel { get; set; }
        public LessonElementMediator Mediator { get; set; }
        public override bool IsSide => false;
        public override bool ShouldPersistInHistory => false;
        public override Type ComponentType => typeof(RemoveNoteInteractionComponent);

        public class NoteModelMediator: Parameters
        {
            public NoteModel NoteModel { get; }
            public LessonElementMediator Mediator { get; set; }
            public NoteModelMediator(NoteModel noteModel, LessonElementMediator mediator)
            {
                NoteModel = noteModel;
                Mediator = mediator;
            }

            public override void ApplyParametersToModel(RemoveNoteModel model)
            {
                model.Mediator = Mediator;
                model.NoteModel = NoteModel;
            }
        }
    }
}
