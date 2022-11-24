using Bible_Blazer_PWA.Services.Parse;
using System.Linq;
using System.Text.RegularExpressions;
using System;
using Bible_Blazer_PWA.DataBase;
using System.Threading.Tasks;
using System.Net.Http;
using Bible_Blazer_PWA.Config;
using System.Text.Json;
using Bible_Blazer_PWA.Extensions;
using Bible_Blazer_PWA.DataBase.DTO;

namespace Bible_Blazer_PWA
{
    internal class ParseLines_LessonElementDataInitializationStrategy : ILessonDataInitializationStrategy
    {
        private string[] lines;
        private readonly string unitId;
        private readonly string lessonId;
        private readonly DatabaseJSFacade dbFacade;
        private readonly DateTime versionDate;
        private readonly HttpClient http;
        private int currentIndex = 0; // wierd issues with ?compilation? when it is local
        private int currentLevel = 0;
        private Func<LessonElementData, int, string, string, LessonElementData> addChildMethod;
        private int[] identifier = new int[] { 0, 0, 0 };

        private LessonElementData prevLessonElementData = null;
        private int[] prevIdentifier;

        public ParseLines_LessonElementDataInitializationStrategy(
            string[] lines,
            string unitId,
            string lessonId,
            DatabaseJSFacade dbFacade,
            DateTime versionDate)
        {
            this.lines = lines;
            this.unitId = unitId;
            this.lessonId = lessonId;
            this.dbFacade = dbFacade;
            this.versionDate = versionDate;
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

            var lessonElementData = addChildMethod(parent, level, identifier.ConcatWithDotDelemiter(), value);
            await StartPutPreviousElement();
            prevLessonElementData = lessonElementData;
            prevIdentifier = identifier.ToArray();
            return lessonElementData;
        }

        private async Task StartPutPreviousElement()
        {
            if (prevLessonElementData is null || prevLessonElementData is null)
                return;

            await StartPutLessonElementData(prevIdentifier, lessonId, unitId, prevLessonElementData.Value, versionDate);
        }

        private async Task StartPutLessonElementData(int[] identifier, string lessonId, string unitId, string value, DateTime versionDate)
        {
            LessonElementDataDTO lessonElementDataDb = new()
            {
                Id = identifier,
                LessonId = lessonId,
                UnitId = unitId,
                Content = value,
                VersionDate = versionDate
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
                    await StartPutLessonElementData(new[] { 0, 0, 0 }, lessonId, unitId, lessonElementData.Value, versionDate);
                }
            }
            await StartPutPreviousElement();
        }

        public void SetAddChildMethod(Func<LessonElementData, int, string, string, LessonElementData> addChildMethod)
        {
            this.addChildMethod = addChildMethod;
        }
    }
}