using System;
using System.Threading.Tasks;

namespace Bible_Blazer_PWA.Services.Parse
{
    public interface ILessonDataInitializationStrategy
    {
        Task Initialize(LessonElementData lessonElementData);
        void SetAddChildMethod(Func<LessonElementData, int, string, string, LessonElementData> addChildMethod);
    }
}
