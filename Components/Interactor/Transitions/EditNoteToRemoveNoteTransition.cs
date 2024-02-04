using Bible_Blazer_PWA.Components.Interactor.EditNote;
using Bible_Blazer_PWA.Components.Interactor.RemoveNote;
using BibleComponents;
using DocumentFormat.OpenXml.EMMA;
using static Bible_Blazer_PWA.Components.Interactor.RemoveNote.RemoveNoteModel;

namespace Bible_Blazer_PWA.Components.Interactor.Transitions
{
    public class EditNoteToRemoveNote : Transition<EditNoteModel>
    {
        public override void ApplyTransition(EditNoteModel source)
        {
            source.OnRemoveNote += noteModel =>
            {
                RemoveNoteModel.WithParameters<NoteModelMediator>
                    .Apply(new(noteModel, source.ElelementForNoteAdding), false);
            };
        }
    }
}
