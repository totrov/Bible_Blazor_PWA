using MudBlazor.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bible_Blazer_PWA.Parameters
{
    public enum LevelSpecificParametersGroup
    {
        BackgroundColor,
        FontWeight,
        MarginTop,
        BodyBackgroundColor
    }
    public class LevelSpecificParameters
    {
        Dictionary<int, Dictionary<LevelSpecificParametersGroup, string>> valuesPerLevel;

        Dictionary<int, Dictionary<LevelSpecificParametersGroup, Parameters>> parametersGroupToParameterMappingPerLevel;
        Dictionary<Parameters, (int, LevelSpecificParametersGroup)> parameterToLevelAndParametersGroupMapping;
        Dictionary<string, string[]> colorsCache;

        private readonly DbParametersFacade dbParametersFacade;
        private const int levelCount = 3;

        public LevelSpecificParameters(DbParametersFacade dbParametersFacade)
        {
            InitDictionaries();
            this.dbParametersFacade = dbParametersFacade;
        }
        internal async Task UpdateAllValues()
        {
            foreach (var param in parameterToLevelAndParametersGroupMapping.Keys)
            {
                var parameterStringValue = await dbParametersFacade.GetParameterAsync(param);
                UpdateValue(param, parameterStringValue);
            }
        }
        internal void UpdateValue(Parameters parameter, string parameterStringValue)
        {
            if (!parameterToLevelAndParametersGroupMapping.ContainsKey(parameter))
                return;

            (var level, var parameterGroup) = parameterToLevelAndParametersGroupMapping[parameter];
            var calculatedValue = CalculateActualParameterValue(parameterGroup, parameterStringValue);
            valuesPerLevel[level][parameterGroup] = calculatedValue;
        }
        internal string GetValue(int level, LevelSpecificParametersGroup parameter)
        {
            if (parameter == LevelSpecificParametersGroup.BackgroundColor || parameter == LevelSpecificParametersGroup.BodyBackgroundColor)
            {
                string color = dbParametersFacade.ParametersModel.ToolsBg;
                if (color != null)
                {
                    if (!colorsCache.ContainsKey(color))
                    {
                        var originalColor = new MudColor(color);
                        var part = (1 - originalColor.L) / 100.0;

                        colorsCache.Add(color, new[] {
                            new MudColor(originalColor.H, originalColor.S, originalColor.L + part * 91, 1.0).Value,
                            new MudColor(originalColor.H, originalColor.S, originalColor.L + part * 96, 1.0).Value,
                            new MudColor(originalColor.H, originalColor.S, originalColor.L + part * 100, 1.0).Value
                        });
                    }
                    return colorsCache[color][level - 1];
                }                
            }
            return valuesPerLevel[level][parameter];
        }

        private void InitDictionaries()
        {
            colorsCache = new();

            valuesPerLevel = new();
            for (int level = 1; level <= levelCount; level++)
            {
                valuesPerLevel.Add(level, new());
            }

            CreateParameterToLevelAndParametersGroupMapping();
            CreateParametersGroupToParameterMappingPerLevel();
        }
        private void CreateParameterToLevelAndParametersGroupMapping()
        {
            parameterToLevelAndParametersGroupMapping = new()
            {
                { Parameters.FirstLevelBg, (1, LevelSpecificParametersGroup.BackgroundColor) },
                { Parameters.FirstLevelBodyBg, (1, LevelSpecificParametersGroup.BodyBackgroundColor) },
                { Parameters.FirstLevelFontWeight, (1, LevelSpecificParametersGroup.FontWeight) },
                { Parameters.FirstLevelMarginTop, (1, LevelSpecificParametersGroup.MarginTop) },

                { Parameters.SecondLevelBg, (2, LevelSpecificParametersGroup.BackgroundColor) },
                { Parameters.SecondLevelBodyBg, (2, LevelSpecificParametersGroup.BodyBackgroundColor) },
                { Parameters.SecondLevelFontWeight, (2, LevelSpecificParametersGroup.FontWeight) },
                { Parameters.SecondLevelMarginTop, (2, LevelSpecificParametersGroup.MarginTop) },

                { Parameters.ThirdLevelBg, (3, LevelSpecificParametersGroup.BackgroundColor) },
                { Parameters.ThirdLevelBodyBg, (3, LevelSpecificParametersGroup.BodyBackgroundColor) },
                { Parameters.ThirdLevelFontWeight, (3, LevelSpecificParametersGroup.FontWeight) },
                { Parameters.ThirdLevelMarginTop, (3, LevelSpecificParametersGroup.MarginTop) }
            };
        }
        private void CreateParametersGroupToParameterMappingPerLevel()
        {
            parametersGroupToParameterMappingPerLevel = new();
            foreach (var keyValue in parameterToLevelAndParametersGroupMapping)
            {
                (var level, var parameterGroup) = keyValue.Value;
                var parameter = keyValue.Key;

                Dictionary<LevelSpecificParametersGroup, Parameters> parameterGroupToParameterMappingForLevel;
                if (!parametersGroupToParameterMappingPerLevel.ContainsKey(level))
                {
                    parameterGroupToParameterMappingForLevel = new();
                    parametersGroupToParameterMappingPerLevel.Add(level, parameterGroupToParameterMappingForLevel);
                }
                else
                {
                    parameterGroupToParameterMappingForLevel = parametersGroupToParameterMappingPerLevel[level];
                }
                parameterGroupToParameterMappingForLevel.Add(parameterGroup, parameter);
            }
        }
        private static string CalculateActualParameterValue(LevelSpecificParametersGroup parametersGroup, string parameterStringValue)
        {
            static string calcMarginTop(string inputValue)
            {
                int MarginTopInt = int.TryParse(inputValue, out int marginTopParsed) ? marginTopParsed : 0;
                return MarginTopInt == 0 ? "0" : MarginTopInt.ToString() + "px";
            }

            return parametersGroup switch
            {
                LevelSpecificParametersGroup.BackgroundColor
                    => (parameterStringValue != null && parameterStringValue != "") ? parameterStringValue : "white",

                LevelSpecificParametersGroup.FontWeight
                    => (parameterStringValue != null && parameterStringValue != "") ? parameterStringValue : "300",

                LevelSpecificParametersGroup.MarginTop
                    => calcMarginTop(parameterStringValue),

                LevelSpecificParametersGroup.BodyBackgroundColor
                    => (parameterStringValue != null && parameterStringValue != "") ? parameterStringValue : "white",

                _ => parameterStringValue,
            };
        }
    }
}




//for (int level = 1; level <= levelCount; level++)
//{


//    foreach (var parametersGroup in Enum.GetValues(typeof(LevelSpecificParametersGroup)).Cast<LevelSpecificParametersGroup>())
//    {
//        mappingForCurrentLevel.Add(parametersGroup, Parameters.ThirdLevelBg);
//        mappingForCurrentLevel.Add(LevelSpecificParametersGroup.BodyBackgroundColor, Parameters.ThirdLevelBodyBg);
//        mappingForCurrentLevel.Add(LevelSpecificParametersGroup.FontWeight, Parameters.ThirdLevelFontWeight);
//        mappingForCurrentLevel.Add(LevelSpecificParametersGroup.MarginTop, Parameters.ThirdLevelMarginTop);
//    }

//    levelAndParametersGroup_Parameter_Mapping.Add(level, mappingForCurrentLevel);
//}


//internal async Task UpdateAllValues()
//{
//    for (int level = 1; level <= levelCount; level++)
//    {
//        var currentLevelMapping = levelAndParametersGroup_Parameter_Mapping[level];
//        Parameters parameter;
//        Dictionary<LevelSpecificParametersGroup, string> valuesPerParameterGroup = new();

//        foreach (var parametersGroup in Enum.GetValues(typeof(LevelSpecificParametersGroup)).Cast<LevelSpecificParametersGroup>())
//        {
//            parameter = currentLevelMapping[parametersGroup];
//            var parameterStringValue = await dbParametersFacade.GetParameterAsync(parameter);
//            var paramValue = CalculateActualParameterValue(parametersGroup, parameterStringValue);
//            valuesPerParameterGroup.Add(parametersGroup, paramValue);
//        }
//    }
//}