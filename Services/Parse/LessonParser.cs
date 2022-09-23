using AngleSharp.Dom;
using Bible_Blazer_PWA.BibleReferenceParse;
using Bible_Blazer_PWA.Config;
using Bible_Blazer_PWA.DataBase;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace Bible_Blazer_PWA.Services.Parse
{
    public class LessonParser
    {
        class LessonModel
        {
            public string UnitId { get; set; }
            public string Id { get; set; }
            public string Name { get; set; }
            public string Content { get; set; }
        }
        public static async Task<string> ParseLessons(string _input, HttpClient Http)
        {
            var idSet = new HashSet<int>();

            string regex = BibleRegexHelper.GetLessonsPattern();
            var contents = Regex.Split(_input, regex);

            string UnitId = GetUnitId(contents[0]);
            contents = Regex.Split(await ApplyReplacements(_input, UnitId, Http), regex);

            LessonModel lessonModel = null;
            var lessonsList = new LinkedList<LessonModel>();
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
                    if (Regex.IsMatch(lessonModel.Content, BibleRegexHelper.GetSublessonHeaderPattern(false)))
                    {
                        AddSublessons(lessonsList, lessonModel, idSet);
                    }
                    else
                    {
                        lessonsList.AddLast(lessonModel);
                    }
                }
            }

            using MemoryStream memoryStream = new MemoryStream();
            await JsonSerializer.SerializeAsync(memoryStream, lessonsList, new JsonSerializerOptions() { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
            memoryStream.Position = 0;
            using StreamReader sr = new(memoryStream);
            string result = sr.ReadToEnd();
            return result;
        }

        private static void AddSublessons(LinkedList<LessonModel> lessonsList, LessonModel lessonModel, HashSet<int> idSet)
        {
            string name = lessonModel.Name.TrimEnd('.');
            int sublessonNumber = 1;

            foreach (Match sublessonMatch in Regex.Matches(lessonModel.Content, BibleRegexHelper.GetSublessonsPattern()))
            {
                var lessonHeader = sublessonMatch.Groups["header"].Value;
                lessonHeader = lessonHeader.Replace("<br>", "");
                lessonHeader = String.IsNullOrEmpty(lessonHeader) ? lessonHeader : $": {lessonHeader}";
                lessonsList.AddLast(new LessonModel
                {
                    Id = GetId(lessonModel.Id, idSet),
                    UnitId = lessonModel.UnitId,
                    Name = $"{name}.  Урок {sublessonNumber++}{lessonHeader}",
                    Content = sublessonMatch.Value
                });
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

        private static async Task<string> ApplyReplacements(string input, string unitId, HttpClient Http)
        {
            Dictionary<string, Dictionary<string, string>> replacements = new Dictionary<string, Dictionary<string, string>>
            {
                {
                    "Быт",
                    new Dictionary<string, string>()
                    {
                        {
                            "человека.                                                                                                                                      (а)",
                            "человека.\r(а)"
                        },
                        {
                            "Лев.7:20-21,25-27;32:30;",
                            "Лев.7:20-21,25-27;"
                        },
                        {
                            "2)Важность понимания видов Заветов.\rОбщий Завет – Божьи условия для пребывания ЛЮБОГО\rчеловека вместе с Богом(Эммануил).\rЛичный Завет – обещание Бога или человека в отношении\rчеловека или народа(не Завет спасения,Эммануила).",
                            "2)Важность понимания видов Заветов.\r(1)Общий Завет – Божьи условия для пребывания ЛЮБОГО\rчеловека вместе с Богом(Эммануил).\r(2)Личный Завет – обещание Бога или человека в отношении \rчеловека или народа(не Завет спасения, Эммануила)."
                        },
                        {
                            "1.Сторона Бога",
                            "(1)Сторона Бога"
                        },
                        {
                            "(1)Решение всех проблем человека во Христе",
                            "(а)Решение всех проблем человека во Христе"
                        },
                        {
                            "(2)Эммануил – вечное пребывание Бога с человеком.",
                            "(б)Эммануил – вечное пребывание Бога с человеком."
                        },
                        {
                            "2.Сторона человека.",
                            "(2)Сторона человека."
                        },
                        {
                            "(1)Принятие Божественности Христа.",
                            "(а)Принятие Божественности Христа."
                        },
                        {
                            "(2)Принятие прощения всех грехов",
                            "(б)Принятие прощения всех грехов"
                        },
                        {
                            "Выбор человека – принятие",
                            "(в)Выбор человека – принятие"
                        },
                        {
                            "Важность правильного понимания веры",
                            "3)Важность правильного понимания веры"
                        },
                        {
                            "Важность понимания праведности Божьей",
                            "3)Важность понимания праведности Божьей"
                        },
                        {
                            "Важность различения Религии и Евангелия",
                            "3)Важность различения Религии и Евангелия"
                        },
                        {
                            "РЕЛИГИЯ – спасение своими силами",
                            "(1)РЕЛИГИЯ – спасение своими силами"
                        },
                        {
                            "БЛАГАЯ ВЕСТЬ – спасение от Б.(Христос умер вместо меня)",
                            "(2)БЛАГАЯ ВЕСТЬ – спасение от Б.(Христос умер вместо меня)"
                        },
                        {
                            "12.\r\rВажность понимания спасения",
                            "12.\r\r8)Важность понимания спасения"
                        },
                        {
                            " меня)\rВажность понимания спасения",
                            " меня)\r3)Важность понимания спасения"
                        },
                        {
                            "В.З. – вера в грядущего Христа и Его Жертву",
                            "(1)В.З. – вера в грядущего Христа и Его Жертву"
                        },
                        {
                            "Н.З. – вера в пришедшего Иисуса как Христа и Его Жертву",
                            "(2)Н.З. – вера в пришедшего Иисуса как Христа и Его Жертву"
                        },
                        {
                            "Итог:",
                            "000)Итог:"
                        }
                    }
                },
                {
                    "ДеянОткр",
                    new Dictionary<string, string>()
                    {
                        {
                            "Деяния.\r1.Дн.1-2;",
                            "1.Деяния.\r1.Дн.1-2;"
                        },
                        {
                            "Соборное послание апостола Иакова.       ",
                            "2.Соборное послание апостола Иакова.       "
                        },
                        {
                            "1-е Соборное послание апостола Петра.",
                            "3.I Соборное послание апостола Петра."
                        },
                        {
                            "    2-е соборное послание апостола Петра. ",
                            "4.II соборное послание апостола Петра."
                        },
                        {
                            "1-е Соборное послание апостола Иоанна.",
                            "5.I Соборное послание апостола Иоанна."
                        },
                        {
                            "2-е и 3-е Соборное послание апостола Иоанна.",
                            "6.II и III Соборное послание апостола Иоанна."
                        },
                        {
                            "          Соборное послание апостола Иуды.",
                            "7.Соборное послание апостола Иуды."
                        },
                        {
                            "              6.Римлянам(58 г. от Р.Х.)",
                            "8.Римлянам(58 г. от Р.Х.)"
                        },
                        {
                            "           16.Обзор послания к Римлянам.",
                            "16.Обзор послания к Римлянам."
                        },
                        {
                            "            7.1-е послание Коринфянам.",
                            "9.I послание Коринфянам."
                        },
                        {
                            "1.1)1-е Кор.7:1-9; - особенная важность ",
                            "1)1-е Кор.7:1-9; - особенная важность "
                        },
                        {
                            "2.1)1-е Кор.7:10-17; - особенная ",
                            "2. Брак и семья II 1)1-е Кор.7:10-17; - особенная "
                        },
                        {
                            "13.1.Любовь ",
                            "I часть 1)Любовь "
                        },
                        {
                            "13.2.Любовь ",
                            "13.Понимание любви Божьей II часть 1)Любовь "
                        },
                        {
                            "13.3.Любовь ",
                            "13.Понимание любви Божьей III часть 1)Любовь "
                        },
                        {
                            "С.Д.<br>Спасение",
                            "С.Д.1)Спасение"
                        },
                        {
                            "15.Откровение(понимание будущего).",
                            "15_.Откровение(понимание будущего)."
                        },
                        {
                            "1.Отк.1-3; - Сохранение и передача Завета,Хр.",
                            "15.1.Отк.1 - 3; -Сохранение и передача Завета,Хр."
                        },
                        {
                            "2.Отк.4 - 5; -понимание Божьего суверенитета.",
                            "15.2.Отк.4 - 5; -понимание Божьего суверенитета."
                        },
                        {
                            "3.Отк.6-10; - 1-я половина Великой Скорби,\rначало судов Божьих,восхищение Церкви.",
                            "15.3.Отк.6 - 10; -1 - я половина Великой Скорби, начало судов Божьих, восхищение Церкви."
                        },
                        {
                            "4.Отк.11-12; - 1-я половина Великой Скорби,\rпонимание событий,восхищение Церкви.",
                            "15.4.Отк.11 - 12; -1 - я половина Великой Скорби, понимание событий, восхищение Церкви."
                        },
                        {
                            "5.Отк.13; - 2-я половина Великой Скорби,\rвоцарение антихриста и лже-пророка.",
                            "15.5.Отк.13; -2 - я половина Великой Скорби, воцарение антихриста и лже-пророка."
                        },
                        {
                            "6.Отк.14-18; - 2-я половина Великой Скорби,\rСуды Божьи,осуждение религии.",
                            "15.6.Отк.14 - 18; -2 - я половина Великой Скорби, Суды Божьи, осуждение религии."
                        },
                        {
                            "7.Отк.19-20; - Приход Христа,1000-е царство.",
                            "15.7.Отк.19 - 20; -Приход Христа, 1000 - е царство."
                        },
                        {
                            "8.Обьяснение Иисуса Христа,понимание будущего.",
                            "15.8.Обьяснение Иисуса Христа, понимание будущего."
                        },
                        {
                            "9.Отк.20; - Понимание суда Божьего.",
                            "15.9.Отк.20; - Понимание суда Божьего."
                        },
                        {
                            "10.Действие с.\rСатана(дьявол) – противник,клеветник.",
                            "15.10.Действие с.Сатана(дьявол) – противник,клеветник."
                        },
                        {
                            "11.Отк.21-22; - Понимание вечности.",
                            "15.11.Отк.21 - 22; -Понимание вечности."
                        },
                        {
                            "12.Обзор книги Откровение.",
                            "15.12.Обзор книги Откровение."
                        }
                    }
                },
                {
                    "ИсхСол",
                    new Dictionary<string, string>
                    {
                        {
                            "Праздник – особенный день акцентирующий внимание человека на определенном событии.",
                            "1)Праздник – особенный день акцентирующий внимание человека на определенном событии."
                        },
                        {
                            "29.Воцарение и Скиния Давидова. ",
                            "32.Воцарение и Скиния Давидова. "
                        },
                        {
                            "30.Царствование ",
                            "32.Царствование "
                        },
                        {
                            "31.Обзор жизни Давида",
                            "32.Обзор жизни Давида"
                        },
                        {
                            "Основы веры,1-7 урок.",
                            "Основы веры,1-7 урок.\r1)см. блок Основы Веры"
                        },
                        {
                            "Быт.16:11-16,17:21.",
                            "Быт.16:11-16,Быт.17:21."
                        }
                    }
                },
                {
                    "Осн",
                    new Dictionary<string, string>
                    {
                        {
                            "Евр.3:12-19(14);6:4-6;10:26;",
                            "Евр.3:12-19(14);Евр.6:4-6;10:26;"
                        },
                        {
                            "Лев.7:20-21,25-27;32:30;",
                            "Лев.7:20-21,25-27;"
                        },
                        {
                            "Еф.2:2;Дн.26:18;1-е Тим.2:25-26.",
                            "Еф.2:2;Дн.26:18."
                        },
                        {
                            ";1-е Тим.4:22",
                            ""
                        }
                    }
                }
            };

            replacements = await Http.GetFromJsonAsync<Dictionary<string, Dictionary<string, string>>>(
                LessonLoadConfig.GetReplacementsUrl(true));

            if (replacements.ContainsKey(unitId))
            {
                return MultipleReplace(input, replacements[unitId]);
            }
            return input;
        }
        private static string MultipleReplace(string text, Dictionary<string, string> replacements)
        {
            return Regex.Replace(
                text,
                "(" + String.Join("|", replacements.Keys).Replace("(", "[(]").Replace(")", "[)]") + ")",
                (Match m) => { return replacements[m.Value]; }
            );
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
