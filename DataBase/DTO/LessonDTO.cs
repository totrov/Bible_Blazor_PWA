using Bible_Blazer_PWA.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bible_Blazer_PWA.DataBase.DTO
{
    public record LessonDTO: LessonLightweightDTO
    {
        public string Content { get; set; }
        private string[] GetLines()
        {
            var list = new LinkedList<string>();
            list.AddLast(Name);
            Content.Split("<br>").Aggregate(list, (l, s) => { l.AddLast(s); return l; });
            return list.ToArray();
        }
        public LessonElementData GetComposite(DatabaseJSFacade db, System.Net.Http.HttpClient http)
        {
            return new LessonElementData(
                new ParseLines_LessonElementDataInitializationStrategy(GetLines(), UnitId, Id, db, VersionDate));
        }
    }

    public record LessonLightweightDTO
    {
        public string UnitId { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime VersionDate { get; set; }
    }
}