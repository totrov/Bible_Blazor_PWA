using System.Collections.Generic;
using System.Linq;

namespace Bible_Blazer_PWA
{
    public class LessonContainer
    {
        public string LinkName { get; set; }
        public string Content { get; set; }
        private string[] GetLines()
        {
            return Content.Split("<br>");
        }
        public LessonElementData GetComposite()
        {
            return new LessonElementData(this.GetLines());
        }
    }

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
            Content.Split("<br>").Aggregate( list, (l, s) => { l.AddLast(s); return l; });
            return list.ToArray();
        }
        public LessonElementData GetComposite()
        {
            return new LessonElementData(this.GetLines());
        }
    }
}
