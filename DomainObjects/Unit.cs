using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bible_Blazer_PWA.DomainObjects
{
    public class Unit
    {
        private static readonly Dictionary<int, (string, string)> UnitEnum = new Dictionary<int, (string, string)>
        {
            {1, ("Бытие", "Быт")},
            {2, ("Деяния - Откровение", "ДеянОткр") },
            {3, ("Евангелия", "Евн") },
            {4, ("Исход - Соломон", "ИсхСол") },
            {5, ("Основы веры", "Осн") },
            {6, ("Пророки", "Прор") },
        };

        public static int GetUnitNumberByShortName(string shortName)
        {
            return UnitEnum.Where(x => x.Value.Item2 == shortName).FirstOrDefault().Key;
        }

        public static string GetShortNameByUnitNumber(int unitNumber)
        {
            return UnitEnum[unitNumber].Item2;
        }

        public static string GetNameByUnitNumber(int unitNumber)
        {
            return UnitEnum[unitNumber].Item1;
        }
    }
}
