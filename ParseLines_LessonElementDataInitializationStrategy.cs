using Bible_Blazer_PWA.Services.Parse;
using System.Linq;
using System.Text.RegularExpressions;
using System;
using Bible_Blazer_PWA.DataBase;
using System.Threading.Tasks;
using System.Net.Http;
using Bible_Blazer_PWA.Extensions;

namespace Bible_Blazer_PWA
{
    internal class ParseLines_LessonElementDataInitializationStrategy : ILessonDataInitializationStrategy
    {
        private string[] lines;
        private string unitId;
        private string lessonId;
        private readonly HttpClient http;
        private int currentIndex = 0; // wierd issues with ?compilation? when it is local
        private int currentLevel = 0;
        private Func<LessonElementData, int, int[], string, LessonElementData> addChildMethod;
        private int[] identifier = new int[] { 0, 0, 0 };

        private LessonElementData prevLessonElementData = null;
        private int[] prevIdentifier;

        private ILessonElementDataStagingImplemeter staging;

        public ParseLines_LessonElementDataInitializationStrategy(
            string[] lines,
            ILessonElementDataStagingImplemeter staging)
        {
            this.lines = lines;
            this.staging = staging;
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

            var lessonElementData = addChildMethod(parent, level, identifier.ToArray(), value);
            await StartPutPreviousElement();
            prevLessonElementData = lessonElementData;
            prevIdentifier = identifier.ToArray();
            return lessonElementData;
        }

        private async Task StartPutPreviousElement()
        {
            if (prevLessonElementData is null || prevLessonElementData is null)
                return;

            await staging.StartPutLessonElementData(prevIdentifier, lessonId, unitId, prevLessonElementData.Value);
        }

        public async Task Initialize(LessonElementData lessonElementData)
        {
            var enumerator = lines.AsEnumerable().GetEnumerator();
            enumerator.MoveNext();
            string current = enumerator.Current;
            lessonElementData.Value = "";
            lessonElementData.Level = 0;
            lessonId = lessonElementData.LessonId;
            unitId = lessonElementData.UnitId;

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
                                //third level containing sublevels should represent it as list
                                //so exit only when face \u0003 before level pattern,
                                //that means end of nesting of elements of 4th level (that a represented as list items)
                                if (Regex.IsMatch(lines[currentIndex], "^[(][а-я][)]\u0002"))
                                {
                                    var child3 = await AddChild(child2, 3, lines[currentIndex++]);
                                    while (currentIndex < lines.Length && !Regex.IsMatch(lines[currentIndex], "\u0003"))
                                    {
                                        child3.Value += " " + lines[currentIndex++];
                                    }
                                    child3.Value += " " + lines[currentIndex++];
                                }
                                //regular third level with exit when face any level
                                else if (Regex.IsMatch(lines[currentIndex], "^[(][а-я][)]"))
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
                    await staging.StartPutLessonElementData(new[] { 0, 0, 0 }, lessonId, unitId, lessonElementData.Value);
                }
            }
            await StartPutPreviousElement();
        }

        public void SetAddChildMethod(Func<LessonElementData, int, int[], string, LessonElementData> addChildMethod)
        {
            this.addChildMethod = addChildMethod;
        }
    }
}