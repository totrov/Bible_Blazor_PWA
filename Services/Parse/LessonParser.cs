using Bible_Blazer_PWA.DataBase.DTO;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Bible_Blazer_PWA.Services.Parse
{
    public class LessonParser
    {
        public static IEnumerable<LessonDTO> ParseLessons(string _input, ICorrector corrector, DateTime versionDate)
        {
            var idSet = new HashSet<int>();

            string regex = corrector.RegexHelper.GetLessonsPattern();
            var contents = Regex.Split(_input, regex);

            string UnitId = GetUnitId(contents[0]);
            contents = Regex.Split(corrector.ApplyHighLevelReplacements(_input, UnitId), regex);

            LessonDTO lessonDTO = null;
            var numberRegex = "([0-9]+[.]?[0-9]*)[.]";
            for (int i = 1; i < contents.Length; i++)
            {
                if (i % 2 == 1)
                {
                    lessonDTO = new LessonDTO() { UnitId = UnitId, VersionDate = versionDate };
                    var split = Regex.Split(contents[i], numberRegex);
                    lessonDTO.Name = Regex.Split(contents[i], numberRegex)[2];
                    lessonDTO.Id = Regex.Match(contents[i], numberRegex).Value;
                    lessonDTO.Id = GetId(lessonDTO.Id.Substring(0, lessonDTO.Id.Length - 1), idSet);
                }
                else
                {
                    lessonDTO.Content = contents[i].Replace("\r", "<br>");
                    if (Regex.IsMatch(lessonDTO.Content, corrector.RegexHelper.GetSublessonHeaderPattern(false)))
                    {
                        foreach (var lesson in GetSublessons(lessonDTO, idSet, corrector))
                        {
                            yield return lesson;
                        }
                    }
                    else
                    {
                        yield return lessonDTO;
                    }
                }
            }
        }

        private static IEnumerable<LessonDTO> GetSublessons(LessonDTO lessonModel, HashSet<int> idSet, ICorrector corrector)
        {
            string name = lessonModel.Name.TrimEnd('.');
            int sublessonNumber = 1;

            foreach (Match sublessonMatch in Regex.Matches(lessonModel.Content, corrector.RegexHelper.GetSublessonsPattern()))
            {
                var lessonHeader = sublessonMatch.Groups["header"].Value;
                lessonHeader = lessonHeader.Replace("<br>", "");
                lessonHeader = String.IsNullOrEmpty(lessonHeader) ? lessonHeader : $": {lessonHeader}";
                yield return new LessonDTO
                {
                    Id = GetId(lessonModel.Id, idSet),
                    UnitId = lessonModel.UnitId,
                    Name = $"{name}.  Урок {sublessonNumber++}{lessonHeader}",
                    Content = sublessonMatch.Value,
                    VersionDate = lessonModel.VersionDate
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
