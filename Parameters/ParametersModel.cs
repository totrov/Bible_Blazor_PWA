using System.Threading.Tasks;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System;
using Bible_Blazer_PWA.Parameters.ParameterInitializers;

namespace Bible_Blazer_PWA.Parameters
{
    public class ParametersModel
    {
        Dictionary<string, PropertyInfo> parameterProps;
        private readonly Dictionary<Parameters, IConcreteParameterInitializer> parameterInitializers;
        public ParametersModel(DbParametersFacade dbParams)
        {
            _dbParams = dbParams;
            parameterProps = this.GetType().GetProperties().Where(p => p.GetCustomAttribute<ParameterAttribute>() is not null).ToDictionary(p => p.Name);

            var initializerType = typeof(IConcreteParameterInitializer);
            parameterInitializers = Assembly.GetAssembly(typeof(IConcreteParameterInitializer))
                .GetTypes().Where<Type>(t => initializerType.IsAssignableFrom(t) && t.IsClass)
                .Select(t => Activator.CreateInstance(t)).Cast<IConcreteParameterInitializer>()
                .ToDictionary(i => i.Parameter);
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
                    parameterProps[key].SetValue(this, value);
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
                
                Task initTask = getValueTask.ContinueWith(value => { parameterProps[param.ToString()].SetValue(this, initializer.InitParam(value.Result)); });
                tasks.AddLast(initTask);
            }
            await Task.WhenAll(tasks);
        }

        internal string GetParamPropByName(string paramName)
        {
            return (string)parameterProps[paramName].GetValue(this);
        }

        #region MainBackground
        private string _mainBackground;

        [Parameter]
        public string MainBackground
        {
            get => _mainBackground;
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
        public string HideTools
        {
            get => _areToolsHidden; set
            {
                _dbParams.SetParameterAsync(Parameters.HideTools, value);
                _areToolsHidden = value;
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
        [Parameter]
        public string AreReferencesOpened
        {
            get => areReferencesOpened;
            set { areReferencesOpened = value; HandleOnReferenceToggle(); }
        }
        #endregion

        [Parameter]
        public string FontSize { get; set; }

        [Parameter]
        public string CollapseLevel { get; set; }

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

        #endregion
    }

}

