using Bible_Blazer_PWA.Services.Parse;
using System.Collections.Generic;

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
    public class LessonElementData
    {
        public string Value { get; set; }
        public LinkedList<LessonElementData> Children { get; set; }
        public int Level { get; set; }
        public string Key { get; set; }

        private LessonElementData()
        { }

        private LessonElementData AddChild(LessonElementData parent, int level, string value)
        {
            parent.Children ??= new LinkedList<LessonElementData>();
            var newChild = new LessonElementData { Level = level, Value = value };
            parent.Children.AddLast(newChild);
            return newChild;
        }

        public LessonElementData(ILessonDataInitializationStrategy initialization)
        {
            initialization.SetAddChildMethod(AddChild);
            initialization.Initialize(this);
        }
    }

    public class LessonElementDataDb
    {
        public int[] Id { get; set; }
        public string UnitId { get; set; }
        public string LessonId { get; set; }
        public string Content { get; set; }
    }
}
