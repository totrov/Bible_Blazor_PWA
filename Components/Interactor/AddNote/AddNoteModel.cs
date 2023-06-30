using BibleComponents;
using System;

namespace Bible_Blazer_PWA.Components.Interactor.AddNote
{
    public class AddNoteModel : InteractionModelBase
    {
        public LessonElementMediator ElelementForNoteAdding { get; set; }
        public override event Action OnClose;
        public override Type ComponentType => typeof(AddNoteInteractionComponent);
        public override bool IsSide { get => false; }
        public override void Close() => OnClose?.Invoke();

        public class Parameters:IInteractionModelParameters<AddNoteModel>
        {
            public LessonElementMediator ElelementForNoteAdding { get; set; }
            public Parameters(LessonElementMediator elelementForNoteAdding)
            {
                ElelementForNoteAdding = elelementForNoteAdding;
            }

            public void ApplyParametersToModel(AddNoteModel model)
            {
                model.ElelementForNoteAdding = ElelementForNoteAdding;
            }
        }
    }


}
