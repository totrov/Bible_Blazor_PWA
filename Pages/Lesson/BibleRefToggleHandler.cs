using Bible_Blazer_PWA.Parameters;
using Bible_Blazer_PWA.Services.Menu;
using System;

namespace Bible_Blazer_PWA.Pages.Lesson
{
    public class BibleRefToggleHandler : GeneralHandler
    {
        public BibleRefToggleHandler(IconResolver iconResolver, Action postHandleAction, ParametersModel parameters) : base(iconResolver, postHandleAction, parameters) { }

        public override int StatesCount => 2;

        public override string Key => "BibleRefToggle";

        public override string GetIcon(int state)
        {
            return state switch
            {
                0 => iconResolver.GetIcon("ExpandBibleRef"),
                1 => iconResolver.GetIcon("CollapseBibleRef"),
                _ => ""
            };
        }

        protected override void HandleInternal(int state)
        {
            switch (state)
            {
                case 0:
                    parameters.AreReferencesOpened = "True";
                    break;
                case 1:
                    parameters.AreReferencesOpened = "False";
                    break;
                default:
                    throw new Exception($"State {state} has not implemented handler in {this.GetType().Name}");
            };
        }
    }
}
