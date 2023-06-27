using Bible_Blazer_PWA.Components.Interactor.EditNote;
using Bible_Blazer_PWA.Components.Interactor.RemoveNote;
using BibleComponents;
using DocumentFormat.OpenXml.EMMA;

namespace Bible_Blazer_PWA.Components.Interactor.Transitions
{
    public class EditNoteToRemoveNote : Transition<EditNoteModel>
    {
        public override void ApplyTransition(EditNoteModel source)
        {
            source.OnRemoveNote += noteModel =>
            {
                Interaction.ModelOfType<RemoveNoteModel>.WithParameters<RemoveNoteModel.Parameters>
                    .Apply(new(noteModel, source.ElelementForNoteAdding));
            };
        }
    }
}
