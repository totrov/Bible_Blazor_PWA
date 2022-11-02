using System;

namespace Bible_Blazer_PWA.Services.Parse
{
    public interface ILessonDataInitializationStrategy
    {
        void Initialize(LessonElementData lessonElementData);
        void SetAddChildMethod(Func<LessonElementData, int, string, LessonElementData> addChildMethod);
    }
}
