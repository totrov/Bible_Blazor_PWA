using Bible_Blazer_PWA.Components.Interactor;
using Bible_Blazer_PWA.Parameters;
using Bible_Blazer_PWA.Services.Menu;
using System;

namespace Bible_Blazer_PWA.Pages.Lesson
{
    public class LessonLevelHandler : GeneralHandler
    {
        public LessonLevelHandler(IconResolver iconResolver, Action postHandleAction) : base(iconResolver, postHandleAction) { }

        public override int StatesCount => 3;

        public override string Key => "LessonLevel";

        public override string GetIcon(int state)
        {
            return state switch
            {
                0 => iconResolver.GetIcon("3level"),
                1 => iconResolver.GetIcon("2level"),
                2 => iconResolver.GetIcon("1level"),
                _ => ""
            };
        }

        protected override void HandleInternal(int state)
        {
            switch (state)
            {
                case 0:
                    parameters.CollapseLevel = 3.ToString();
                    break;
                case 1:
                    parameters.CollapseLevel = 2.ToString();
                    break;
                case 2:
                    parameters.CollapseLevel = 1.ToString();
                    break;
                default:
                    throw new Exception($"State {state} has not implemented handler in {this.GetType().Name}");
            };
        }
    }
}
