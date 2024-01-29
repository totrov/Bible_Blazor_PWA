using System.Threading.Tasks;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System;
using Bible_Blazer_PWA.Parameters.ParameterInitializers;
using Microsoft.AspNetCore.Components;
using BibleComponents;
using Bible_Blazer_PWA.ViewModels;

namespace Bible_Blazer_PWA.Parameters
{
    public class ParametersModel
    {
        Dictionary<string, PropertyInfo> parameterProps;
        private readonly Dictionary<Parameters, IConcreteParameterInitializer> parameterInitializers;
        private readonly LevelSpecificParameters levelSpecificParameters;
        public ParametersModel(DbParametersFacade dbParams)
        {
            _dbParams = dbParams;
            parameterProps = this.GetType().GetProperties().Where(p => p.GetCustomAttribute<ParameterAttribute>() is not null).ToDictionary(p => p.Name);

            var initializerType = typeof(IConcreteParameterInitializer);
            parameterInitializers = Assembly.GetAssembly(typeof(IConcreteParameterInitializer))
                .GetTypes().Where<Type>(t => initializerType.IsAssignableFrom(t) && t.IsClass)
                .Select(t => Activator.CreateInstance(t)).Cast<IConcreteParameterInitializer>()
                .ToDictionary(i => i.Parameter);
            levelSpecificParameters = new LevelSpecificParameters(dbParams);
        }

        internal async Task SetPropertyByName(string key, string value, bool updateDb = false)
        {
            if (parameterProps.ContainsKey(key))
            {
                bool proceedWithUpdate = true;
                if (updateDb)
                {
                    proceedWithUpdate = await _dbParams.SetParameterAsync(key, value);
                }
                if (proceedWithUpdate)
                {
                    var prop = parameterProps[key];
                    if (prop.PropertyType == typeof(string))
                        prop.SetValue(this, value);
                    else
                        prop.SetValue(this, string.IsNullOrEmpty(value) ? null : bool.Parse(value));
                }
            }
        }

        DbParametersFacade _dbParams;
        public void SetDbParametersFacade(DbParametersFacade dbParams)
        {
            _dbParams = dbParams;
        }

        public async Task InitFromDb()
        {
            LinkedList<Task> tasks = new LinkedList<Task>();
            foreach (var param in Enum.GetValues<Parameters>())
            {
                Task<string> getValueTask = _dbParams.GetParameterAsync(param, true);

                IGenericParameterInitializer initializer = new NullToEmptyParameterInitializer();
                if (parameterInitializers.TryGetValue(param, out var concreteInitializer))
                    initializer = concreteInitializer;

                Task initTask = getValueTask.ContinueWith(value => { parameterProps[param.ToString()].SetValue(this,
                    parameterProps[param.ToString()].PropertyType == typeof(string)
                    ? initializer.InitParam(value.Result)
                    : (string.IsNullOrEmpty(value.Result) ? null : bool.Parse(value.Result))); });
                tasks.AddLast(initTask);
            }
            await Task.WhenAll(tasks);
            await levelSpecificParameters.UpdateAllValues();
        }

        public string GetParameterForLevel(int level, LevelSpecificParametersGroup parameter)
        {
            return levelSpecificParameters.GetValue(level, parameter);
        }

        internal string GetParamPropByName(string paramName)
        {
            return (string)parameterProps[paramName].GetValue(this);
        }

        #region NotPersistedInDbParameters

        #region NotesEnabled
        private bool? _notesEnabled;
        [Parameter]
        public bool? NotesEnabled
        {
            get => _notesEnabled;
            set
            {
                _dbParams.SetParameterAsync(Parameters.NotesEnabled, value.ToString(), false);
                _notesEnabled = value;
            }
        }

        #endregion

        #region CollapseLevel
        private string _collapseLevel;
        [Parameter]
        public string CollapseLevel
        {
            get => _collapseLevel;
            set
            {
                _dbParams.SetParameterAsync(Parameters.CollapseLevel, value.ToString(), false);
                _collapseLevel = value;
            }
        }

        #endregion

        #endregion

        #region MainBackground
        private string _mainBackground;

        [Parameter]
        [Obsolete]
        public string MainBackground
        {
            get => "white";//_mainBackground;
            set
            {
                _dbParams.SetParameterAsync(Parameters.MainBackground, value.ToString());
                _mainBackground = value;
            }
        }
        #endregion

        #region ToolsBackground
        private string _toolsBackground;
        [Parameter]
        public string ToolsBg
        {
            get => _toolsBackground;
            set
            {
                _dbParams.SetParameterAsync(Parameters.ToolsBg, value.ToString());
                _toolsBackground = value;
            }
        }
        #endregion

        #region Tools

        #region AreToolsHidden
        private string _areToolsHidden;
        [Parameter]
        [Obsolete]
        public string HideTools
        {
            get => _areToolsHidden; set
            {
                _dbParams.SetParameterAsync(Parameters.HideTools, value);
                _areToolsHidden = value;
            }
        }
        #endregion

        #region BibleTextAtTheBottom
        private string _bibleTextAtTheBottom;
        [Parameter]
        public string BibleTextAtTheBottom
        {
            get => _bibleTextAtTheBottom; set
            {
                _dbParams.SetParameterAsync(Parameters.BibleTextAtTheBottom, value);
                _bibleTextAtTheBottom = value;
            }
        }
        #endregion

        #region ReferencesOpened
        private string areReferencesOpened;

        public event EventHandler OnReferenceToggle;
        protected virtual void HandleOnReferenceToggle()
        {
            EventHandler handler = OnReferenceToggle;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        internal void HandleParameterChange(Parameters parameter, string parameterValue)
        {
            levelSpecificParameters.UpdateValue(parameter, parameterValue);
        }

        [Parameter]
        public string AreReferencesOpened
        {
            get => areReferencesOpened;
            set { areReferencesOpened = value; HandleOnReferenceToggle(); }
        }
        #endregion

        #region FontSize
        private string _fontSize;

        [Parameter]
        public string FontSize
        {
            get => _fontSize;
            set
            {
                _dbParams.SetParameterAsync(Parameters.FontSize, value.ToString());
                _fontSize = value;
            }
        }
        #endregion

        #endregion

        #region FirstLevel
        #region FirstLevelMarginTop
        private string _firstLevelMarginTop;
        [Parameter]
        public string FirstLevelMarginTop
        {
            get => _firstLevelMarginTop;
            set
            {
                _dbParams.SetParameterAsync(Parameters.FirstLevelMarginTop, value);
                _firstLevelMarginTop = value;
            }
        }
        #endregion

        #region FirstLevelFontWeight
        private string _firstLevelFontWeight;
        [Parameter]
        public string FirstLevelFontWeight
        {
            get => _firstLevelFontWeight;
            set
            {
                _dbParams.SetParameterAsync(Parameters.FirstLevelFontWeight, value.ToString());
                _firstLevelFontWeight = value;
            }
        }
        #endregion

        #region FirstLevelBodyBackground
        private string _firstLevelBodyBackground;
        [Parameter]
        public string FirstLevelBodyBg
        {
            get => _firstLevelBodyBackground;
            set
            {
                _dbParams.SetParameterAsync(Parameters.FirstLevelBodyBg, value.ToString());
                _firstLevelBodyBackground = value;
            }
        }
        #endregion

        #region FirstLevelBackground
        private string _firstLevelBackground;
        [Parameter]
        public string FirstLevelBg
        {
            get => _firstLevelBackground;
            set
            {
                _dbParams.SetParameterAsync(Parameters.FirstLevelBg, value.ToString());
                _firstLevelBackground = value;
                FirstLevelBodyBg = value;
            }
        }
        #endregion
        #endregion

        #region SecondLevel
        #region SecondLevelMarginTop
        private string _secondLevelMarginTop;
        [Parameter]
        public string SecondLevelMarginTop
        {
            get => _secondLevelMarginTop;
            set
            {
                _dbParams.SetParameterAsync(Parameters.SecondLevelMarginTop, value);
                _secondLevelMarginTop = value;
            }
        }
        #endregion

        #region SecondLevelFontWeight
        private string _secondLevelFontWeight;
        [Parameter]
        public string SecondLevelFontWeight
        {
            get => _secondLevelFontWeight;
            set
            {
                _dbParams.SetParameterAsync(Parameters.SecondLevelFontWeight, value);
                _secondLevelFontWeight = value;
            }
        }
        #endregion

        #region SecondLevelBodyBackground
        private string _secondLevelBodyBackground;
        [Parameter]
        public string SecondLevelBodyBg
        {
            get => _secondLevelBodyBackground;
            set
            {
                _dbParams.SetParameterAsync(Parameters.SecondLevelBodyBg, value);
                _secondLevelBodyBackground = value;
            }
        }
        #endregion

        #region SecondLevelBackground
        private string _secondLevelBackground;
        [Parameter]
        public string SecondLevelBg
        {
            get => _secondLevelBackground;
            set
            {
                _dbParams.SetParameterAsync(Parameters.SecondLevelBg, value);
                _secondLevelBackground = value;
                SecondLevelBodyBg = value;
            }
        }
        #endregion
        #endregion

        #region ThirdLevel
        #region ThirdLevelMarginTop
        private string _thirdLevelMarginTop;
        [Parameter]
        public string ThirdLevelMarginTop
        {
            get => _thirdLevelMarginTop;
            set
            {
                _dbParams.SetParameterAsync(Parameters.ThirdLevelMarginTop, value);
                _thirdLevelMarginTop = value;
            }
        }
        #endregion

        #region ThirdLevelFontWeight
        private string _thirdLevelFontWeight;
        [Parameter]
        public string ThirdLevelFontWeight
        {
            get => _thirdLevelFontWeight;
            set
            {
                _dbParams.SetParameterAsync(Parameters.ThirdLevelFontWeight, value);
                _thirdLevelFontWeight = value;
            }
        }
        #endregion

        #region ThirdLevelBodyBackground
        private string _thirdLevelBodyBackground;
        [Parameter]
        public string ThirdLevelBodyBg
        {
            get => _thirdLevelBodyBackground;
            set
            {
                _dbParams.SetParameterAsync(Parameters.ThirdLevelBodyBg, value);
                _thirdLevelBodyBackground = value;
            }
        }
        #endregion

        #region ThirdLevelBackground
        private string _thirdLevelBackground;
        [Parameter]
        public string ThirdLevelBg
        {
            get => _thirdLevelBackground;
            set
            {
                _dbParams.SetParameterAsync(Parameters.ThirdLevelBg, value);
                _thirdLevelBackground = value;
                ThirdLevelBodyBg = value;
            }
        }
        #endregion
        #endregion

        #region Blocks

        #region BlocksPadding

        #region GeneralPadding
        private string _blocksPadding;
        [Parameter]
        public string BlocksPadding
        {
            get => _blocksPadding;
            set
            {
                _dbParams.SetParameterAsync(Parameters.BlocksPadding, value);
                _blocksPadding = value;
            }
        }
        #endregion

        #region BlocksPaddingLeft
        private string _blocksPaddingLeft;
        [Parameter]
        public string BlocksPaddingLeft
        {
            get => _blocksPaddingLeft;
            set
            {
                _dbParams.SetParameterAsync(Parameters.BlocksPaddingLeft, value);
                _blocksPaddingLeft = value;
            }
        }
        #endregion

        #region BlocksPaddingRight
        private string _blocksPaddingRight;
        [Parameter]
        public string BlocksPaddingRight
        {
            get => _blocksPaddingRight;
            set
            {
                _dbParams.SetParameterAsync(Parameters.BlocksPaddingRight, value);
                _blocksPaddingRight = value;
            }
        }
        #endregion

        #endregion

        #region BlocksBorder

        private string _hideBlocksBorders;
        [Parameter]
        public string HideBlocksBorders
        {
            get => _hideBlocksBorders; set
            {
                _dbParams.SetParameterAsync(Parameters.HideBlocksBorders, value);
                _hideBlocksBorders = value;
            }
        }

        #endregion

        #endregion

        #region BibleReferences

        private string _startVersesOnANewLine;
        [Parameter]
        public string StartVersesOnANewLine
        {
            get => _startVersesOnANewLine; set
            {
                _dbParams.SetParameterAsync(Parameters.StartVersesOnANewLine, value);
                _startVersesOnANewLine = value;
            }
        }

        private string _bibleRefBgColor;
        [Parameter]
        public string BibleRefBgColor
        {
            get => _bibleRefBgColor;
            set
            {
                _dbParams.SetParameterAsync(Parameters.BibleRefBgColor, value);
                _bibleRefBgColor = value;
            }
        }

        private string _bibleRefFontColor;
        [Parameter]
        public string BibleRefFontColor
        {
            get => _bibleRefFontColor;
            set
            {
                _dbParams.SetParameterAsync(Parameters.BibleRefFontColor, value);
                _bibleRefFontColor = value;
            }
        }

        private string _bibleRefHighlightColor;
        [Parameter]
        public string BibleRefHighlightColor
        {
            get => _bibleRefHighlightColor;
            set
            {
                _dbParams.SetParameterAsync(Parameters.BibleRefHighlightColor, value);
                _bibleRefHighlightColor = value;
            }
        }

        private string _bibleRefVersesNumbersColor;
        [Parameter]
        public string BibleRefVersesNumbersColor
        {
            get => _bibleRefVersesNumbersColor;
            set
            {
                _dbParams.SetParameterAsync(Parameters.BibleRefVersesNumbersColor, value);
                _bibleRefVersesNumbersColor = value;
            }
        }

        private string _bibleRefVersesIntervalsColor;
        [Parameter]
        public string BibleRefVersesIntervalsColor
        {
            get => _bibleRefVersesIntervalsColor;
            set
            {
                _dbParams.SetParameterAsync(Parameters.BibleRefVersesIntervalsColor, value);
                _bibleRefVersesIntervalsColor = value;
            }
        }

        private string _hideBibleRefTabs;
        [Obsolete]
        [Parameter]
        public string HideBibleRefTabs
        {
            get => "True"; set
            {
                _dbParams.SetParameterAsync(Parameters.HideBibleRefTabs, value);
                _hideBibleRefTabs = value;
                OnHideBibleRefTabs?.Invoke();
            }
        }
        public event Action OnHideBibleRefTabs;
        #endregion
    }

}

