using Bible_Blazer_PWA.DataBase;
using System.Collections.Generic;
using System.Linq;

namespace Bible_Blazer_PWA
{
    public class LessonContainerDb
    {
        public string UnitId { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public string LinkName { get; set; }
        private string[] GetLines()
        {
            var list = new LinkedList<string>();
            list.AddLast(Name);
            Content.Split("<br>").Aggregate(list, (l, s) => { l.AddLast(s); return l; });
            return list.ToArray();
        }
        public LessonElementData GetComposite(DatabaseJSFacade db)
        {
            return new LessonElementData(
                new ParseLines_LessonElementDataInitializationStrategy(this.GetLines(), UnitId, Id, db));
        }
    }
}
