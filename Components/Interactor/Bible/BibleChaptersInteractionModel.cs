using System;

namespace Bible_Blazer_PWA.Components.Interactor.Bible
{
    public class BibleChaptersInteractionModel : InteractionModelBase<BibleChaptersInteractionModel>
    {
        public override bool IsSide => true;

        public override bool ShouldPersistInHistory => false;

        public override Type ComponentType => typeof(BibleChaptersInteractionComponent);
        public int BookId { get; set; }

        public class BibleBookId : Parameters
        {
            public BibleBookId(int bookId)
            {
                BookId = bookId;
            }

            public int BookId { get; set; }
            public override void ApplyParametersToModel(BibleChaptersInteractionModel model)
            {
                model.BookId = BookId;
            }
        }
    }
}
