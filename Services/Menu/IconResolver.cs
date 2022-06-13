using MudBlazor;
using System;

namespace Bible_Blazer_PWA.Services.Menu
{
    public class IconResolver : IIconResolver
    {
        public string GetIcon(string icon)
        {
            return icon switch
            {
                "1level" => @Icons.Filled.Square,
                "2level" => @Icons.Filled.GridView,
                "3level" => @Icons.Filled.Apps,
                "CollapseBibleRef" => @Icons.Filled.ExpandLess,
                "ExpandBibleRef" => @Icons.Filled.ExpandMore,
                "IncreaseFontSize" => @Icons.Filled.TextIncrease,
                "DecreaseFontSize" => @Icons.Filled.TextDecrease,
                _ => "",
            };
        }
    }
}
