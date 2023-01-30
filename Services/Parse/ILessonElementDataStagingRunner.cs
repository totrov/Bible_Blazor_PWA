using System;
using System.Threading.Tasks;

namespace Bible_Blazer_PWA.Services.Parse
{
    public interface ILessonElementDataStagingImplemeter
    {
        Task StartPutLessonElementData(int[] lessonElementDataIdentifier, string lessonId, string unitId, string value);
        void SetVersionDate(DateTime versionDate);
    }
}
