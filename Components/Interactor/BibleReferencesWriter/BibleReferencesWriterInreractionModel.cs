using BibleComponents;
using System;

namespace Bible_Blazer_PWA.Components.Interactor.BibleReferencesWriter
{
    public partial class Interaction
    {
        public class BibleReferencesWriterInteractionModel : InteractionModelBase<BibleReferencesWriterInteractionModel>
        {
            public override bool IsSide => true;
            public override bool ShouldPersistInHistory => true;
            public override Type ComponentType => typeof(BibleReferencesWriterInteractionComponent);
            public override event Action OnClose;
            public override void Close() => OnClose?.Invoke();

            public event Action<string, int, int> OnLinkClicked;
            public void LinkClicked(string BookShortName, int ChapterNumber, int Verse)
                => OnLinkClicked?.Invoke(BookShortName, ChapterNumber, Verse);
            public void MouseLeave() { if (!Overflowed) OnClose?.Invoke(); }
            public LessonElementMediator Mediator { get; set; }
            public int ReferenceNumber { get; set; }
            public bool Overflowed { get; set; } = false;


            public class MediatorReferenceNumber : Parameters
            {
                public LessonElementMediator LessonElementMediator { get; set; }
                public int ReferenceNumber { get; private set; }
                public MediatorReferenceNumber(LessonElementMediator lessonElementMediator, int referenceNumber)
                {
                    LessonElementMediator = lessonElementMediator;
                    ReferenceNumber = referenceNumber;
                }
                public override void ApplyParametersToModel(BibleReferencesWriterInteractionModel model)
                {
                    model.Mediator = LessonElementMediator;
                    model.ReferenceNumber = ReferenceNumber;
                }
            }
        }
    }
}
