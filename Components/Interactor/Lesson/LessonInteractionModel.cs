using System;

namespace Bible_Blazer_PWA.Components.Interactor.Lesson
{
    public class LessonInteractionModel : InteractionModelBase<LessonInteractionModel>
    {
        public override bool IsSide => false;

        public override bool ShouldPersistInHistory => true;

        public override Type ComponentType => throw new NotImplementedException();

        public override event Action OnClose;
        public override void Close() => OnClose?.Invoke();

        #region Parameters

        public string UnitId { get; set; }
        public string LessonNumber { get; set; }

        public class UnitIdLessonId : Parameters
        {
            public string UnitId { get; set; }
            public string LessonNumber { get; set; }
            public UnitIdLessonId(string unitId, string lessonNumber)
            {
                UnitId = unitId;
                LessonNumber = lessonNumber;
            }

            public override void ApplyParametersToModel(LessonInteractionModel model)
            {
                model.UnitId = UnitId;
                model.LessonNumber = LessonNumber;
            }
        }
        #endregion
    }
}
