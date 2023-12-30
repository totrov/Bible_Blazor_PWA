using Bible_Blazer_PWA.Components.Interactor.Home;
using BibleComponents;
using MudBlazor;
using System;
using System.Collections.Generic;

namespace Bible_Blazer_PWA.Components.Interactor.AddNote
{
    public class AddNoteModel : InteractionModelBase<AddNoteModel>
    {
        public LessonElementMediator ElelementForNoteAdding { get; set; }
        public override Type ComponentType => typeof(AddNoteInteractionComponent);
        public override bool IsSide { get => false; }
        public override bool ShouldPersistInHistory => false;

        public override IEnumerable<BreadcrumbsFacade.BreadcrumbRecord> GetBreadcrumbs()
        {
            return null;
        }

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
