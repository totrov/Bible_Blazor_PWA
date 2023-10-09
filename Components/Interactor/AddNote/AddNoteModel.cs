using BibleComponents;
using System;

namespace Bible_Blazer_PWA.Components.Interactor.AddNote
{
    public class AddNoteModel : InteractionModelBase<AddNoteModel>
    {
        public LessonElementMediator ElelementForNoteAdding { get; set; }
        public override event Action OnClose;
        public override Type ComponentType => typeof(AddNoteInteractionComponent);
        public override bool IsSide { get => false; }
        public override bool ShouldPersistInHistory => false;

        public override void Close() => OnClose?.Invoke();

        public class ElementMediator: Parameters
        {
            public LessonElementMediator ElelementForNoteAdding { get; set; }
            public ElementMediator(LessonElementMediator elelementForNoteAdding)
            {
                ElelementForNoteAdding = elelementForNoteAdding;
            }

            public override void ApplyParametersToModel(AddNoteModel model)
            {
                model.ElelementForNoteAdding = ElelementForNoteAdding;
            }
        }
    }


}
