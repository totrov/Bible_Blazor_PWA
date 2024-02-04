using Bible_Blazer_PWA.Parameters;
using Bible_Blazer_PWA.Services.Menu;
using System;

namespace Bible_Blazer_PWA.Pages.Lesson
{
    public class FontSizeIncreaseHandler : GeneralHandler
    {
        public FontSizeIncreaseHandler(IconResolver iconResolver, Action postHandleAction) : base(iconResolver, postHandleAction) { }

        public override int StatesCount => 1;

        public override string Key => "FintSizeIncrease";

        public override string GetIcon(int state)
        {
            return state switch
            {
                0 => iconResolver.GetIcon("IncreaseFontSize"),
                _ => ""
            };
        }

        protected override void HandleInternal(int state)
        {
            if (int.TryParse(parameters.FontSize, out int currentSize))
            {
                parameters.FontSize = (currentSize + 1).ToString();
            }
            else
            {
                parameters.FontSize = 14.ToString();
            }
        }
    }
}
