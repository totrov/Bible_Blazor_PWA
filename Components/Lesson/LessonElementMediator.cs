using Bible_Blazer_PWA;
using Bible_Blazer_PWA.BibleReferenceParse;
using Bible_Blazer_PWA.Components.Lesson;
using Bible_Blazer_PWA.DomainObjects;
using Bible_Blazer_PWA.Parameters;
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
        internal ParametersModel Parameters { get; set; }
        internal LessonElementData ElementData { get; set; }
        internal Parser Parser { get; set; }
        internal DbParametersFacade DbParamFacade { get; set; }
        internal bool IsOpen { get; set; } = true;
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
        internal Dictionary<string, IEnumerable<BibleService.VersesView>> VersesViewsDictionary { get => _versesViewsDictionary; }
        internal string Border { get => Parameters.HideBlocksBorders == "True" ? "border:none;" : ""; }
        #endregion

        #region PublicMethods
        public void Toggle()
        {
            IsOpen = !IsOpen;
        }
        public void ToggleReferences()
        {
            RefsAreOpen = !RefsAreOpen;
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
            }
            else
            {
                Tabs?.ActivatePanel(number);
            }
            StateHasChanged?.Invoke(typeof(LessonElementBody));
        }

        #endregion

        #region ParametersForLevel
        internal async Task InitParametersForLevel()
        {
            Parameters[] parametersForLevel;
            switch (ElementData.Level)
            {
                case 1:
                    parametersForLevel = new Parameters[]
                    {
                        Bible_Blazer_PWA.Parameters.Parameters.FirstLevelBg,
                        Bible_Blazer_PWA.Parameters.Parameters.FirstLevelBodyBg,
                        Bible_Blazer_PWA.Parameters.Parameters.FirstLevelFontWeight,
                        Bible_Blazer_PWA.Parameters.Parameters.FirstLevelMarginTop
                    };
                    break;
                case 2:
                    parametersForLevel = new Parameters[]
                    {
                        Bible_Blazer_PWA.Parameters.Parameters.SecondLevelBg,
                        Bible_Blazer_PWA.Parameters.Parameters.SecondLevelBodyBg,
                        Bible_Blazer_PWA.Parameters.Parameters.SecondLevelFontWeight,
                        Bible_Blazer_PWA.Parameters.Parameters.SecondLevelMarginTop
                    };
                    break;
                default:
                    parametersForLevel = new Parameters[]
                    {
                        Bible_Blazer_PWA.Parameters.Parameters.ThirdLevelBg,
                        Bible_Blazer_PWA.Parameters.Parameters.ThirdLevelBodyBg,
                        Bible_Blazer_PWA.Parameters.Parameters.ThirdLevelFontWeight,
                        Bible_Blazer_PWA.Parameters.Parameters.ThirdLevelMarginTop
                    };
                    break;
            }
            var BackgroundColorString = await DbParamFacade.GetParameterAsync(parametersForLevel[0]);
            BackgroundColor = (BackgroundColorString != null && BackgroundColorString != "") ? BackgroundColorString : "white";
            var FontWeightString = await DbParamFacade.GetParameterAsync(parametersForLevel[2]);
            FontWeight = (FontWeightString != null && FontWeightString != "") ? FontWeightString : "300";
            var MarginTopString = await DbParamFacade.GetParameterAsync(parametersForLevel[3]);
            int MarginTopInt = int.TryParse(MarginTopString, out int marginTopParsed) ? marginTopParsed : 0;
            MarginTop = MarginTopInt == 0 ? "0" : MarginTopInt.ToString() + "px";


            var BodyBackgroundColorString = await DbParamFacade.GetParameterAsync(parametersForLevel[1]);
            BodyBackgroundColor = (BodyBackgroundColorString != null && BodyBackgroundColorString != "") ? BodyBackgroundColorString : "white";
        }
        public string FontWeight { get; set; } = "300";
        public string BackgroundColor { get; set; } = "white";
        public string MarginTop { get; set; } = "0";
        public string BodyBackgroundColor { get; set; } = "white";
        #endregion



        internal LessonElementMediator()
        {
        }
    }
}
