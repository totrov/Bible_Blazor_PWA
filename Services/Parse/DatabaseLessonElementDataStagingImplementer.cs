using Bible_Blazer_PWA.DataBase;
using Bible_Blazer_PWA.DataBase.DTO;
using System;
using System.Threading.Tasks;

namespace Bible_Blazer_PWA.Services.Parse
{
    public class DatabaseLessonElementDataStagingImplementer : ILessonElementDataStagingImplemeter
    {
        private DatabaseJSFacade dbFacade;
        private DateTime versionDate;

        public DatabaseLessonElementDataStagingImplementer(DatabaseJSFacade dbFacade)
        {
            this.dbFacade = dbFacade;
        }
        public void SetVersionDate(DateTime versionDate)
        {
            this.versionDate = versionDate;
        }

        public async Task StartPutLessonElementData(int[] lessonElementDataIdentifier, string lessonId, string unitId, string value)
        {
            LessonElementDataDTO lessonElementDataDb = new()
            {
                Id = lessonElementDataIdentifier,
                LessonId = lessonId,
                UnitId = unitId,
                Content = value,
                VersionDate = versionDate
            };
            await dbFacade.StartPutIntoObjectStore("lessonElementData", lessonElementDataDb);
        }
    }
}
