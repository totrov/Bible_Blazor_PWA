using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bible_Blazer_PWA.Services.Parse
{
    public static class Replacer
    {
        private static readonly Dictionary<string, string> mapping = new Dictionary<string, string>
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
            {"Лк.","Лук" }

        };

        internal static string ReplaceBookNames(string stringToParse)
        {
            return mapping.Aggregate(stringToParse, (str, replacement) => { return str.Replace(replacement.Key, replacement.Value); });
        }
    }
}
