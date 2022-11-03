using Bible_Blazer_PWA.Services.Parse;
using System.Linq;
using System.Text.RegularExpressions;
using System;
using Bible_Blazer_PWA.DataBase;
using System.Threading.Tasks;

namespace Bible_Blazer_PWA
{
    internal class ParseLines_LessonElementDataInitializationStrategy : ILessonDataInitializationStrategy
    {
        private string[] lines;
        private readonly string unitId;
        private readonly string lessonId;
        private readonly DatabaseJSFacade dbFacade;
        private int currentIndex = 0; // wierd issues with ?compilation? when it is local
        private int currentLevel = 0;
        private Func<LessonElementData, int, string, LessonElementData> addChildMethod;
        private int[] identifier = new int[] { 0, 0, 0 };

        public ParseLines_LessonElementDataInitializationStrategy(
            string[] lines,
            string unitId,
            string lessonId,
            DatabaseJSFacade dbFacade)
        {
            this.lines = lines;
            this.unitId = unitId;
            this.lessonId = lessonId;
            this.dbFacade = dbFacade;
        }

        private async Task<LessonElementData> AddChild(LessonElementData parent, int level, string value)
        {
            if (level < currentLevel)
            {
                for (int i = identifier.Length - 1; i >= level; i--)
                {
                    identifier[i] = 0;
                }
            }
            currentLevel = level;
            identifier[level - 1]++;

            var lessonElementData = addChildMethod(parent, level, value);
            await StartPutLessonElementData(identifier, lessonId, unitId, lessonElementData.Value);
            return lessonElementData;
        }

        private async Task StartPutLessonElementData(int[] identifier, string lessonId, string unitId, string value)
        {
            LessonElementDataDb lessonElementDataDb = new()
            {
                Id = identifier,
                LessonId = lessonId,
                UnitId = unitId,
                Content = value
            };
            await dbFacade.StartPutIntoObjectStore("lessonElementData", lessonElementDataDb);
        }

        public async Task Initialize(LessonElementData lessonElementData)
        {
            var enumerator = lines.AsEnumerable().GetEnumerator();
            enumerator.MoveNext();
            string current = enumerator.Current;
            lessonElementData.Value = "";
            lessonElementData.Level = 0;

            static bool CheckMatchForFirstLevel(string input, out string match)
            {
                match = Regex.Match(input, "^([0-9][.]?[0-9]?[)])|(Заключение)|(000[)])").Value;
                return !String.IsNullOrEmpty(match);
            }

            while (currentIndex < lines.Length)
            {
                if (CheckMatchForFirstLevel(lines[currentIndex], out string firstLevelMatch))
                {
                    var child1 = await AddChild(lessonElementData, 1, ElementDataProcessor.ProcessFirstLevel(lines[currentIndex++], firstLevelMatch));
                    while (currentIndex < lines.Length && !Regex.IsMatch(lines[currentIndex], "^([0-9][.]?[0-9]?[)])|(Заключение)|(000[)])"))
                    {
                        if (Regex.IsMatch(lines[currentIndex], "^[(][0-9][.]?[0-9]?[)]"))
                        {
                            var child2 = await AddChild(child1, 2, lines[currentIndex++]);
                            while (currentIndex < lines.Length && !Regex.IsMatch(lines[currentIndex], "^([(]?[0-9][.]?[0-9]?[)])|(Заключение)|(000[)])"))
                            {
                                if (Regex.IsMatch(lines[currentIndex], "^[(][а-я][)]"))
                                {
                                    var child3 = await AddChild(child2, 3, lines[currentIndex++]);
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
                    await StartPutLessonElementData(new[] { 0, 0, 0 }, lessonId, unitId, lessonElementData.Value);
                }
            }
        }

        public void SetAddChildMethod(Func<LessonElementData, int, string, LessonElementData> addChildMethod)
        {
            this.addChildMethod = addChildMethod;
        }
    }
}