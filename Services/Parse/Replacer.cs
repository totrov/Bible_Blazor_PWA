using Bible_Blazer_PWA.Config;
using Bible_Blazer_PWA.Facades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bible_Blazer_PWA.Services.Parse
{
    public class Replacer
    {
        private readonly Dictionary<string, string> mapping = new Dictionary<string, string>
        {
            {"Мф.","Мат" },
            {"1-е Кор.","1Кор" },
            {"1-Кор.","1Кор" },
            {"2-е Кор.","2Кор" },
            {"2-Кор.","2Кор" },
            {"Мк.","Мар" },
            {"Дн.","Деян" },
            {"1-е Тим","1Тим"},
            {"2-е Тим","2Тим"},
            {"1-е Ин","1Ин"},
            {"2-е Ин","2Ин"},
            {"1-е Пет","1Пет"},
            {"2-е Пет","2Пет"},
            {"1-я Цар.","1Цар" },
            {"2-я Цар.","2Цар" },
            {"3-я Цар.","3Цар" },
            {"4-я Цар.","4Цар" },
            {"Отк.","Откр."},
            {"1-е Фес","1Фес" },
            {"2-е Фес","2Фес" },
            {"Лк.","Лук" },
            {"Иуды", "Иуд" },
            {"Авк", "Авв" },
            {"Фил","Флп" }
        };
        Dictionary<string, Dictionary<string, string>> replacements = null;
        HttpClient Http;
        public Replacer(HttpClient Http)
        {
            this.Http = Http;
        }

        public async Task InitReplacements()
        {
            replacements = await new HttpFacade(Http).GetRepacementsFromJsonAsync();
        }

        public async Task<string> ApplyHighLevelReplacements(string input, string unitId)
        {
            if (replacements == null)
            {
                await InitReplacements();
            }
            if (replacements.ContainsKey(unitId))
            {
                return MultipleReplace(input, replacements[unitId]);
            }
            return input;
        }

        private string MultipleReplace(string text, Dictionary<string, string> replacements)
        {
            return Regex.Replace(
                text,
                "(" + String.Join("|", replacements.Keys).Replace("(", "[(]").Replace(")", "[)]") + ")",
                (Match m) => { return replacements[m.Value]; }
            );
        }

        internal string HandleBrackets(string stringToParse)
        {
            string buf = stringToParse;
            foreach (Match match in Regex.Matches(stringToParse, BibleRegexHelper.GetBracketsHandlerPattern()))
            {
                var bookName = match.Groups.Cast<Group>().Where(g => g.Name == "book").First().Value;
                var bracketsContent = match.Groups.Cast<Group>().Where(g => g.Name == "bracketsContent").First().Value;
                buf = buf.Replace("(" + bracketsContent + ")", "(" + bookName + bracketsContent + ")");
            }
            return buf;
        }

        internal string ReplaceBookNames(string stringToParse)
        {
            return mapping.Aggregate(stringToParse, (str, replacement) => { return str.Replace(replacement.Key, replacement.Value); });
        }
    }
}
