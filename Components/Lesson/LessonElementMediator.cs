using Bible_Blazer_PWA;
using Bible_Blazer_PWA.BibleReferenceParse;
using Bible_Blazer_PWA.Components.Interactor;
using Bible_Blazer_PWA.Components.Interactor.AddNote;
using Bible_Blazer_PWA.Components.Interactor.EditNote;
using Bible_Blazer_PWA.DataBase;
using Bible_Blazer_PWA.DataBase.DTO;
using Bible_Blazer_PWA.DataSources;
using Bible_Blazer_PWA.DomainObjects;
using Bible_Blazer_PWA.Parameters;
using Bible_Blazer_PWA.ViewModels;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Bible_Blazer_PWA.Components.Interactor.BibleReferencesWriter.Interaction;
using static Bible_Blazer_PWA.Components.Interactor.BibleReferencesWriter.Interaction.BibleReferencesWriterInteractionModel;
using static Bible_Blazer_PWA.Components.Interactor.EditNote.EditNoteModel;
using BibleRefModel = Bible_Blazer_PWA.Components.Interactor.BibleReferencesWriter.Interaction.BibleReferencesWriterInteractionModel;
using Parameters = Bible_Blazer_PWA.Parameters.Parameters;

namespace BibleComponents
{
    public class LessonElementMediator
    {
        #region Shared
        internal LessonElementHeader Header { get; set; }
        internal LessonElementBody Body { get; set; }
        internal ParametersModel Parameters { get; set; }
        internal LessonElementData ElementData { get; set; }
        internal VersesProvider VersesProvider { get; set; }
        internal LinkedList<LessonElementToken> LessonElementTokens { get; set; }
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
                        await VersesProvider.LoadVerses();
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
        private BibleReferencesWriterInteractionModel BibleRefsWriterModel = null;

        #endregion

        #region Getters
        internal IEnumerable<BibleReference> BibleReferences
        {
            get => VersesProvider.BibleReferences;
        }

        internal bool HasBibleReferences
        {
            get
            {
                return BibleReferences.Any();
            }
        }
        
        internal bool ShouldDrawBody { get => GetShouldDrawBody(); }
        internal string Border { get => Parameters.HideBlocksBorders == "True" ? "border:none;" : ""; }
        #endregion

        #region PublicMethods
        public void RefreshBody()
        {
            StateHasChanged?.Invoke(typeof(LessonElementBody));
        }
        public void Toggle()
        {
            IsOpen = !IsOpen;
            RefreshBody();
        }
        public void ToggleReferences()
        {
            RefsAreOpen = !RefsAreOpen;
            RefreshBody();
        }

        public event Action<Type> StateHasChanged;
        public void Activate(int number)
        {
            if (Parameters.BibleTextAtTheBottom == "True")
            {
                if (number == -1)
                {
                    BibleRefsWriterModel?.MouseLeave();
                    return;
                }

                BibleRefsWriterModel = BibleRefModel.WithParameters<VersesProviderReferenceNumber>.Apply(new (VersesProvider, number));
                return;
            }
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
            StateHasChanged?.Invoke(typeof(InteractionPanel));
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
            AddNoteModel.WithParameters<AddNoteModel.ElementMediator>.Apply(new(this));
        }

        public void SetEditNote(NoteModel model)
        {
            EditNoteModel.WithParameters<MediatorNoteModel>.Apply(new(this, model));
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
