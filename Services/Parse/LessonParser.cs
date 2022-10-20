using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Bible_Blazer_PWA.Services.Parse
{
    public record LessonModel
    {
        public string UnitId { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
    }
    public class LessonParser
    {
        public static IEnumerable<LessonModel> ParseLessons(string _input, ICorrector corrector)
        {
            var idSet = new HashSet<int>();

            string regex = corrector.RegexHelper.GetLessonsPattern();
            var contents = Regex.Split(_input, regex);

            string UnitId = GetUnitId(contents[0]);
            contents = Regex.Split(corrector.ApplyHighLevelReplacements(_input, UnitId), regex);

            LessonModel lessonModel = null;
            var numberRegex = "([0-9]+[.]?[0-9]*)[.]";
            for (int i = 1; i < contents.Length; i++)
            {
                if (i % 2 == 1)
                {
                    lessonModel = new LessonModel() { UnitId = UnitId };
                    var split = Regex.Split(contents[i], numberRegex);
                    lessonModel.Name = Regex.Split(contents[i], numberRegex)[2];
                    lessonModel.Id = Regex.Match(contents[i], numberRegex).Value;
                    lessonModel.Id = GetId(lessonModel.Id.Substring(0, lessonModel.Id.Length - 1), idSet);
                }
                else
                {
                    lessonModel.Content = contents[i].Replace("\r", "<br>");
                    if (Regex.IsMatch(lessonModel.Content, corrector.RegexHelper.GetSublessonHeaderPattern(false)))
                    {
                        foreach (var lesson in GetSublessons(lessonModel, idSet, corrector))
                        {
                            yield return lesson;
                        }
                    }
                    else
                    {
                        yield return lessonModel;
                    }
                }
            }
        }

        private static IEnumerable<LessonModel> GetSublessons(LessonModel lessonModel, HashSet<int> idSet, ICorrector corrector)
        {
            string name = lessonModel.Name.TrimEnd('.');
            int sublessonNumber = 1;

            foreach (Match sublessonMatch in Regex.Matches(lessonModel.Content, corrector.RegexHelper.GetSublessonsPattern()))
            {
                var lessonHeader = sublessonMatch.Groups["header"].Value;
                lessonHeader = lessonHeader.Replace("<br>", "");
                lessonHeader = String.IsNullOrEmpty(lessonHeader) ? lessonHeader : $": {lessonHeader}";
                yield return new LessonModel
                {
                    Id = GetId(lessonModel.Id, idSet),
                    UnitId = lessonModel.UnitId,
                    Name = $"{name}.  Урок {sublessonNumber++}{lessonHeader}",
                    Content = sublessonMatch.Value
                };
            }
        }

        private static string GetId(string input, HashSet<int> idSet)
        {
            int dotPosition = input.IndexOf(".");
            int id = Convert.ToInt32(dotPosition switch
            {
                -1 => input.Length > 2 ? input : (Convert.ToInt32(input) * 100).ToString(),
                1 or 2 => getIdForSublesson(dotPosition),
                _ => throw new ArgumentException($"{input} - некоррекный номер урока"),
            });

            while (idSet.Contains(id))
            {
                id++;
            }
            idSet.Add(id);

            return id.ToString();

            string getIdForSublesson(int dotPosition)
            {
                string lessonPart = input.Substring(0, dotPosition);
                string sublessonPart = input.Substring(dotPosition + 1, input.Length - dotPosition - 1);
                switch (sublessonPart.Length)
                {
                    case 1:
                        sublessonPart = "0" + sublessonPart;
                        break;
                    case 2:
                        break;
                    default: throw new ArgumentException($"{input} - некоррекный номер урока");
                }
                return lessonPart + sublessonPart;
            }
        }

        private static string GetUnitId(string contents)
        {
            if (contents.Contains("Бытие"))
            {
                return "Быт";
            }
            if (contents.Contains("Исход.(1446"))
            {
                return "ИсхСол";
            }
            if (contents.Contains("             Деяния."))
            {
                return "ДеянОткр";
            }
            if (contents.Contains("        Евангелия."))
            {
                return "Евн";
            }
            if (contents.Contains("Основы Веры."))
            {
                return "Осн";
            }
            if (contents.Contains("         Пророки."))
            {
                return "Прор";
            }
            return "";
        }
    }
}
