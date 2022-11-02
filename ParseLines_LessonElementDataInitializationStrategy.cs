using Bible_Blazer_PWA.Services.Parse;
using System.Linq;
using System.Text.RegularExpressions;
using System;

namespace Bible_Blazer_PWA
{
    internal class ParseLines_LessonElementDataInitializationStrategy : ILessonDataInitializationStrategy
    {
        private string[] lines;
        private int currentIndex = 0; // wierd issues with ?compilation? when it is local
        private Func<LessonElementData, int, string, LessonElementData> addChildMethod;

        public ParseLines_LessonElementDataInitializationStrategy(string[] lines)
        {
            this.lines = lines;
        }

        public void Initialize(LessonElementData lessonElementData)
        {
            var enumerator = lines.AsEnumerable().GetEnumerator();
            enumerator.MoveNext();
            string current = enumerator.Current;
            lessonElementData.Value = "";
            lessonElementData.Level = 0;

            bool CheckMatchForFirstLevel(string input, out string match)
            {
                match = Regex.Match(input, "^([0-9][.]?[0-9]?[)])|(Заключение)|(000[)])").Value;
                return !String.IsNullOrEmpty(match);
            }

            while (currentIndex < lines.Length)
            {
                if (CheckMatchForFirstLevel(lines[currentIndex], out string firstLevelMatch))
                {
                    var child1 = addChildMethod(lessonElementData, 1, ElementDataProcessor.ProcessFirstLevel(lines[currentIndex++], firstLevelMatch));
                    while (currentIndex < lines.Length && !Regex.IsMatch(lines[currentIndex], "^([0-9][.]?[0-9]?[)])|(Заключение)|(000[)])"))
                    {
                        if (Regex.IsMatch(lines[currentIndex], "^[(][0-9][.]?[0-9]?[)]"))
                        {
                            var child2 = addChildMethod(child1, 2, lines[currentIndex++]);
                            while (currentIndex < lines.Length && !Regex.IsMatch(lines[currentIndex], "^([(]?[0-9][.]?[0-9]?[)])|(Заключение)|(000[)])"))
                            {
                                if (Regex.IsMatch(lines[currentIndex], "^[(][а-я][)]"))
                                {
                                    var child3 = addChildMethod(child2, 3, lines[currentIndex++]);
                                    while (currentIndex < lines.Length && !Regex.IsMatch(lines[currentIndex], "^([(]?(([0-9][.]?[0-9]?)|[а-я])[)])|(Заключение)|(000[)])"))
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
                    lessonElementData.Value += " " + lines[currentIndex++];
                }
            }
        }

        public void SetAddChildMethod(Func<LessonElementData, int, string, LessonElementData> addChildMethod)
        {
            this.addChildMethod = addChildMethod;
        }
    }
}