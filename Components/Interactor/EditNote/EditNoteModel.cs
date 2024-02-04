using Bible_Blazer_PWA.Components.Interactor.Creed;
using Bible_Blazer_PWA.Components.Interactor.RemoveNote;
using Bible_Blazer_PWA.ViewModels;
using BibleComponents;
using System;
using System.Collections.Generic;

namespace Bible_Blazer_PWA.Components.Interactor.EditNote
{
    public class EditNoteModel:InteractionModelBase<EditNoteModel>
    {
        public LessonElementMediator ElelementForNoteAdding { get; set; }
        public NoteModel NoteModel { get; set; }
        public event Action<NoteModel> OnRemoveNote;
        public void RemoveNote(NoteModel noteModel) => OnRemoveNote?.Invoke(noteModel);

        public override IEnumerable<BreadcrumbsFacade.BreadcrumbRecord> GetBreadcrumbs()
        {
            yield return new BreadcrumbsFacade.BreadcrumbRecord
            {
                Text = "Редактирование заметки",
                Action = () => { },
                Icon = null
            };
        }

        public override bool IsSide => true;
        public override bool ShouldPersistInHistory => false;
        public override Type ComponentType => typeof(EditNoteInteractionComponent);

        public class MediatorNoteModel: Parameters
        {
            public LessonElementMediator ElelementForNoteAdding { get; set; }
            public NoteModel NoteModel { get; set; }
            public MediatorNoteModel(LessonElementMediator elelementForNoteAdding, NoteModel noteModel)
            {
                ElelementForNoteAdding = elelementForNoteAdding;
                NoteModel = noteModel;
            }
            public override void ApplyParametersToModel(EditNoteModel model)
            {
                model.NoteModel = NoteModel;
                model.ElelementForNoteAdding = ElelementForNoteAdding;
            }
        }
    }


}
