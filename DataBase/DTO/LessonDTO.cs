using Bible_Blazer_PWA.DataBase;
using Bible_Blazer_PWA.Services.Parse;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bible_Blazer_PWA.DataBase.DTO
{
    public record LessonDTO : LessonLightweightDTO
    {
        public string Content { get; set; }
        protected string[] GetLines()
        {
            var list = new LinkedList<string>();
            list.AddLast(Name);
            Content.Split("<br>").Aggregate(list, (l, s) => { l.AddLast(s); return l; });
            return list.ToArray();
        }
        public LessonElementData GetComposite(ILessonElementDataStagingImplemeter staging)
        {
            staging.SetVersionDate(VersionDate);
            return new LessonElementData(UnitId, Id,
                new ParseLines_LessonElementDataInitializationStrategy(GetLines(), staging));
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