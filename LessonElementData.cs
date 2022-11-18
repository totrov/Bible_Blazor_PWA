using Bible_Blazer_PWA.Config;
using Bible_Blazer_PWA.DataBase;
using Bible_Blazer_PWA.DataBase.DTO;
using Bible_Blazer_PWA.DomainObjects;
using Bible_Blazer_PWA.Facades;
using Bible_Blazer_PWA.Services.Parse;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Bible_Blazer_PWA
{
    internal static class ElementDataProcessor
    {
        internal static string ProcessFirstLevel(string input, string firstLevelMatch)
        {
            if (firstLevelMatch == "000)")
                return input.Replace("000)", "");
            return input;
        }
    }
    public class LessonElementData:IAsyncInitializable
    {
        public string Value { get; set; }
        public LinkedList<LessonElementData> Children { get; set; }
        public int Level { get; set; }
        public string Key { get; set; }

        public Task InitTask => initializationTask;

        private Task initializationTask;
        private LessonElementData()
        { }


        private LessonElementData AddChild(LessonElementData parent, int level, string id, string value)
        {
            parent.Children ??= new LinkedList<LessonElementData>();
            var newChild = new LessonElementData { Level = level, Value = value, Key = id };
            parent.Children.AddLast(newChild);
            return newChild;
        }

        public LessonElementData(ILessonDataInitializationStrategy initialization)
        {
            initialization.SetAddChildMethod(AddChild);
            initializationTask = initialization.Initialize(this);
        }

        public static async Task<LessonElementData> GetLessonCompositeAsync(int unitNumber, int id, DatabaseJSFacade db, HttpClient http)
        {
            var unitId = Unit.GetShortNameByUnitNumber(unitNumber);
            var idStringified = id.ToString();
            var versionDate = await new HttpFacade(http).GetVersionDateAsync();

            LessonElementData lessonElement = new(new DBLessonDataInitializationStrategy(db, unitId, idStringified, versionDate));
            await lessonElement.InitTask;
            if (!string.IsNullOrEmpty(lessonElement.Value))
            {
                return lessonElement;
            }

            var resultHandler = await db.GetRecordFromObjectStoreByKey<LessonDTO>("lessons", unitId, idStringified);
            var result = await resultHandler.GetTaskCompletionSourceWrapper();
            var ret = result.GetComposite(db, http);
            await ret.InitTask;
            return ret;
        }
    }

    public class LessonElementDataDb
    {
        public int[] Id { get; set; }
        public string UnitId { get; set; }
        public string LessonId { get; set; }
        public string Content { get; set; }
        public DateTime VersionDate { get; set; }
    }
}
