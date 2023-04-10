using Bible_Blazer_PWA;
using Bible_Blazer_PWA.BibleReferenceParse;
using Bible_Blazer_PWA.Components.Interactor.AddNote;
using Bible_Blazer_PWA.DataBase;
using Bible_Blazer_PWA.DataBase.DTO;
using Bible_Blazer_PWA.DomainObjects;
using Bible_Blazer_PWA.Parameters;
using Bible_Blazer_PWA.ViewModels;
using BibleComponents;
using DocumentFormat.OpenXml.EMMA;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Parameters = Bible_Blazer_PWA.Parameters.Parameters;

namespace BibleComponents
{
    public class LessonElementMediator
    {
        #region Shared
        internal LessonElementHeader Header { get; set; }
        internal LessonElementBody Body { get; set; }
        internal LessonCenteredContainer LessonCenteredContainer { get; set; }
        internal ParametersModel Parameters { get; set; }
        internal LessonElementData ElementData { get; set; }
        internal Parser Parser { get; set; }
        private DbParametersFacade dbParametersFacade;
        internal DbParametersFacade DbParamFacade
        {
            get => dbParametersFacade;
            set
            {
                dbParametersFacade = value;
                value.OnChangeAsync += async (parameter, _) =>
                {
                    if (parameter == Bible_Blazer_PWA.Parameters.Parameters.StartVersesOnANewLine)
                    {
                        _versesLoaded = false;
                        await LoadVerses();
                        StateHasChanged?.Invoke(typeof(LessonElementReferences));
                    }
                };
            }
        }
        internal DatabaseJSFacade DbFacade { get; set; }
        public bool IsOpen { get; set; } = true;
        internal bool RefsAreOpen { get; set; } = true;
        internal int CurrentPopoverIndex { get; set; } = -1;
        internal MudTabs Tabs { get; set; }
        #endregion

        #region Own
        private LinkedList<BibleReference> _references;
        public BibleService Bible { get; set; }
        Dictionary<string, IEnumerable<BibleService.VersesView>> _versesViewsDictionary;
        bool _versesLoaded = false;

        #endregion

        #region Getters
        internal LinkedList<BibleReference> BibleReferences
        {
            get
            {
                if (_references == null)
                {
                    _references = Parser.ParseTextLineWithBibleReferences(ElementData.Value.ToString()).GetBibleReferences();
                }
                return _references;
            }
        }
        internal bool HasBibleReferences
        {
            get
            {
                return BibleReferences.Any();
            }
        }
        internal bool VersesLoaded { get => _versesLoaded; }
        internal bool ShouldDrawBody { get => GetShouldDrawBody(); }
        internal Dictionary<string, IEnumerable<BibleService.VersesView>> VersesViewsDictionary { get => _versesViewsDictionary; }
        internal string Border { get => Parameters.HideBlocksBorders == "True" ? "border:none;" : ""; }
        #endregion

        #region PublicMethods
        public void Toggle()
        {
            IsOpen = !IsOpen;
            StateHasChanged?.Invoke(typeof(LessonElementBody));
        }
        public void ToggleReferences()
        {
            RefsAreOpen = !RefsAreOpen;
            StateHasChanged?.Invoke(typeof(LessonElementBody));
        }
        public async Task LoadVerses()
        {
            _versesViewsDictionary = new Dictionary<string, IEnumerable<BibleService.VersesView>>();
            foreach (BibleReference reference in BibleReferences)
            {
                _versesViewsDictionary.Add(reference.ToString(), await Bible.GetVersesFromReference(reference));
            }
            _versesLoaded = true;
        }
        public event Action<Type> StateHasChanged;
        public void Activate(int number)
        {
            if (Parameters.HideBibleRefTabs == "True")
            {
                CurrentPopoverIndex = number;
                StateHasChanged?.Invoke(typeof(LessonElementRefPopovers));
            }
            else
            {
                Tabs?.ActivatePanel(number);
            }
        }

        public async Task AddNote(string _value)
        {
            (await ElementData.AddNoteByValue(_value, DbFacade)).OnAfterRemoval += () => { StateHasChanged?.Invoke(typeof(LessonElementBody)); };
            StateHasChanged?.Invoke(typeof(LessonElementBody));
            StateHasChanged?.Invoke(typeof(LessonCenteredContainer));
        }
        public async Task InitNotes()
        {
            var notes = await (await DbFacade.GetRangeFromObjectStoreByIndex<NoteDTO>(
                "notes",
                "lessonElement",
                ElementData.UnitId,
                ElementData.LessonId,
                ElementData.Key,
                ElementData.Key
                )).GetTaskCompletionSourceWrapper();
            foreach (var note in notes)
            {
                note.OnAfterRemoval += () => { StateHasChanged?.Invoke(typeof(LessonElementBody)); };
                ElementData.AddNote(note);
            }
        }
        public void OpenAddNote()
        {
            var model = new AddNoteModel() { ElelementForNoteAdding = this };
            LessonCenteredContainer.SetInteractionModel(model);
            model.OnClose += () => StateHasChanged?.Invoke(typeof(LessonCenteredContainer));
            StateHasChanged?.Invoke(typeof(LessonCenteredContainer));
        }

        public void SetEditNote(NoteModel model)
        {
            Parameters.NoteForEdit = model;
            StateHasChanged?.Invoke(typeof(LessonCenteredContainer));
        }

        #endregion

        #region ParametersForLevel

        public string FontWeight
        {
            get => Parameters.GetParameterForLevel(ElementData.Level, LevelSpecificParametersGroup.FontWeight);
        }
        public string BackgroundColor
        {
            get => Parameters.GetParameterForLevel(ElementData.Level, LevelSpecificParametersGroup.BackgroundColor);
        }
        public string MarginTop
        {
            get => Parameters.GetParameterForLevel(ElementData.Level, LevelSpecificParametersGroup.MarginTop);
        }
        public string BodyBackgroundColor
        {
            get => Parameters.GetParameterForLevel(ElementData.Level, LevelSpecificParametersGroup.BodyBackgroundColor);
        }
        #endregion



        internal LessonElementMediator()
        {
        }

        private bool GetShouldDrawBody()
        {
            if (!IsOpen)
                return false;
            if (ElementData.Children != null)
            {
                return ElementData.Level < int.Parse(Parameters.CollapseLevel);
            }
            else
            {
                return HasBibleReferences && Parameters.HideBibleRefTabs != "True" && RefsAreOpen;
            }
        }
    }
}
