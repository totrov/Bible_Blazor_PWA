using Bible_Blazer_PWA.Parameters;
using Bible_Blazer_PWA.Services.Menu;
using System;

namespace Bible_Blazer_PWA.Pages.Lesson
{
    public class FontSizeDecreaseHandler : GeneralHandler
    {
        public FontSizeDecreaseHandler(IconResolver iconResolver, Action postHandleAction, ParametersModel parameters) : base(iconResolver, postHandleAction, parameters) { }

        public override int StatesCount => 1;

        public override string Key => "FintSizeDecrease";

        public override string GetIcon(int state)
        {
            return state switch
            {
                0 => iconResolver.GetIcon("DecreaseFontSize"),
                _ => ""
            };
        }

        protected override void HandleInternal(int state)
        {
            if (int.TryParse(parameters.FontSize, out int currentSize))
            {
                parameters.FontSize = (currentSize - 1).ToString();
            }
            else
            {
                parameters.FontSize = 14.ToString();
            }
        }
    }
}
