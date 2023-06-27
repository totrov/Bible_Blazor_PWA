using Bible_Blazer_PWA.Components.Interactor.RemoveNote;

namespace Bible_Blazer_PWA.Components.Interactor.Transitions
{
    public class RemoveNoteTransition : Transition<RemoveNoteModel>
    {
        public override void ApplyTransition(RemoveNoteModel source)
        {
            source.OnSuccess += Interaction.GoToPrevious;
            source.OnSuccess += Interaction.RemoveCurrent;
            source.OnSuccess += source.Mediator.RefreshBody;
        }
    }
}
