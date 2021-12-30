using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bible_Blazer_PWA
{
    public class LessonElementData
    {
        private int currentIndex = 0; // wierd issues with ?compilation? when it is local
        public string Value { get; set; }
        public LinkedList<LessonElementData> Children { get; set; }
        public int Level { get; set; }
        public string Key { get; set; }

        private LessonElementData()
        { }

        private LessonElementData AddChild(int level, string value)
        {
            if (Children == null)
            {
                Children = new LinkedList<LessonElementData>();
            }
            var newChild = new LessonElementData { Level = level, Value = value };
            Children.AddLast(newChild);
            return newChild;
        }

        public LessonElementData(string[] lines)
        {
            var enumerator = lines.AsEnumerable().GetEnumerator();
            enumerator.MoveNext();
            string current = enumerator.Current;
            Value = lines[currentIndex];
            Level = 0;
            while (currentIndex < lines.Length)
            {
                if (Regex.IsMatch(lines[currentIndex], "^[0-9][.]?[0-9]?[)]") || Regex.IsMatch(lines[currentIndex], "(Заключение)"))
                {
                    var child1 = this.AddChild(1, lines[currentIndex++]);
                    while(currentIndex < lines.Length && !Regex.IsMatch(lines[currentIndex], "^[0-9][.]?[0-9]?[)]") && !Regex.IsMatch(lines[currentIndex], "Заключение"))
                    {
                        if (Regex.IsMatch(lines[currentIndex], "^[(][0-9][.]?[0-9]?[)]"))
                        {
                            var child2 = child1.AddChild(2, lines[currentIndex++]);
                            while (currentIndex < lines.Length && !Regex.IsMatch(lines[currentIndex], "^[(]?[0-9][.]?[0-9]?[)]") && !Regex.IsMatch(lines[currentIndex], "Заключение"))
                            {
                                if (Regex.IsMatch(lines[currentIndex], "^[(][а-я][)]"))
                                {
                                    var child3 = child2.AddChild(3, lines[currentIndex++]);
                                    while (currentIndex < lines.Length && !Regex.IsMatch(lines[currentIndex], "^[(]?(([0-9][.]?[0-9]?)|[а-я])[)]") && !Regex.IsMatch(lines[currentIndex], "Заключение"))
                                    {
                                        child3.Value += " " + lines[currentIndex++];
                                    }
                                }
                                else
                                {
                                    child2.Value += " " + lines[currentIndex++];
                                }
                            }
                        }
                        else
                        {
                            child1.Value += " " + lines[currentIndex++];
                        }
                    }
                }
                else
                {
                    //if (currentIndex != 0)
                    //{
                        Value += " " + lines[currentIndex++];
                    //}
                }
            }
        }
    }
}
