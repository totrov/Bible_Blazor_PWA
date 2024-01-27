using Bible_Blazer_PWA.Parameters;
using Bible_Blazer_PWA.Services.Menu;
using System;

namespace Bible_Blazer_PWA.Pages.Lesson
{
    public class NotesEditModeToggleHandler : GeneralHandler
    {
        public NotesEditModeToggleHandler(IconResolver iconResolver, Action postHandleAction) : base(iconResolver, postHandleAction) { }
        public override int StatesCount => 3;

        public override string Key => "NotesEditModeToggle";

        public override string GetIcon(int state)
        {
            return state switch
            {
                0 => iconResolver.GetIcon("TurnOnNotesEditMode"),
                1 => iconResolver.GetIcon("TurnOffNotesEditMode"),
                2 => iconResolver.GetIcon("DisableNotes"),
                _ => ""
            };
        }
        protected override void HandleInternal(int state)
        {
            switch (state)
            {
                case 0:
                    parameters.NotesEnabled = true;
                    break;
                case 1:
                    parameters.NotesEnabled = false;
                    break;
                case 2:
                    parameters.NotesEnabled = null;
                    break;
                default:
                    throw new Exception($"State {state} has not implemented handler in {this.GetType().Name}");
            };
        }
    }
}
