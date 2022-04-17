using Bible_Blazer_PWA.Shared;
using System.Threading.Tasks;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System;

namespace Bible_Blazer_PWA.Parameters
{
    public class ParametersModel
    {
        Dictionary<string, PropertyInfo> parameterProps;
        public ParametersModel(DbParametersFacade dbParams)
        {
            _dbParams = dbParams;
            parameterProps = this.GetType().GetProperties().Where(p => p.GetCustomAttribute<ParameterAttribute>() is not null).ToDictionary(p => p.Name);
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
            string mainBackgroundParameterString = await _dbParams.GetParameterAsync(Parameters.MainBackground);
            MainBackground = mainBackgroundParameterString != null ? mainBackgroundParameterString : "";

            string ToolsBackgroundString = await _dbParams.GetParameterAsync(Parameters.ToolsBg);
            ToolsBg = ToolsBackgroundString != null ? ToolsBackgroundString : "";

            string hideToolsParameterString = await _dbParams.GetParameterAsync(Parameters.HideTools);
            HideTools = (hideToolsParameterString != null && hideToolsParameterString == "True").ToString();

            AreReferencesOpened = await _dbParams.GetParameterAsync(Parameters.AreReferencesOpened) ?? "True";
            CollapseLevel = await _dbParams.GetParameterAsync(Parameters.CollapseLevel) ?? "3";
            FontSize = await _dbParams.GetParameterAsync(Parameters.FontSize) ?? "14";
            
            BlocksPadding = await _dbParams.GetParameterAsync(Parameters.BlocksPadding) ?? "1.25";
            BlocksPadding = BlocksPadding == "" ? "1,25" : BlocksPadding;
            BlocksPaddingLeft = await _dbParams.GetParameterAsync(Parameters.BlocksPaddingLeft) ?? "";
            BlocksPaddingLeft = BlocksPaddingLeft == "" ? "1,25" : BlocksPaddingLeft;

            HideBlocksBorders = await _dbParams.GetParameterAsync(Parameters.HideBlocksBorders) ?? "False";

            #region FirstLevel
            string FirstLevelBackgroundString = await _dbParams.GetParameterAsync(Parameters.FirstLevelBg);
            FirstLevelBg = FirstLevelBackgroundString != null ? FirstLevelBackgroundString : "";
            string FirstLevelBodyBackgroundString = await _dbParams.GetParameterAsync(Parameters.FirstLevelBodyBg);
            FirstLevelBodyBg = FirstLevelBodyBackgroundString != null ? FirstLevelBodyBackgroundString : "";
            string FirstLevelFontWeightString = await _dbParams.GetParameterAsync(Parameters.FirstLevelFontWeight);
            FirstLevelFontWeight = FirstLevelFontWeightString != null ? FirstLevelFontWeightString : "";
            string FirstLevelMarginTopString = await _dbParams.GetParameterAsync(Parameters.FirstLevelMarginTop);
            FirstLevelMarginTop = (int.TryParse(FirstLevelMarginTopString, out int firstLevelMarginTop) ? firstLevelMarginTop : 0).ToString();
            #endregion

            #region SecondLevel
            string SecondLevelBackgroundString = await _dbParams.GetParameterAsync(Parameters.SecondLevelBg);
            SecondLevelBg = SecondLevelBackgroundString != null ? SecondLevelBackgroundString : "";
            string SecondLevelBodyBackgroundString = await _dbParams.GetParameterAsync(Parameters.SecondLevelBodyBg);
            SecondLevelBodyBg = SecondLevelBodyBackgroundString != null ? SecondLevelBodyBackgroundString : "";
            string SecondLevelFontWeightString = await _dbParams.GetParameterAsync(Parameters.SecondLevelFontWeight);
            SecondLevelFontWeight = SecondLevelFontWeightString != null ? SecondLevelFontWeightString : "";
            string SecondLevelMarginTopString = await _dbParams.GetParameterAsync(Parameters.SecondLevelMarginTop);
            SecondLevelMarginTop = (int.TryParse(SecondLevelMarginTopString, out int secondLevelMarginTop) ? secondLevelMarginTop : 0).ToString();
            #endregion

            #region ThirdLevel
            string ThirdLevelBackgroundString = await _dbParams.GetParameterAsync(Parameters.ThirdLevelBg);
            ThirdLevelBg = ThirdLevelBackgroundString != null ? ThirdLevelBackgroundString : "";
            string ThirdLevelBodyBackgroundString = await _dbParams.GetParameterAsync(Parameters.ThirdLevelBodyBg);
            ThirdLevelBodyBg = ThirdLevelBodyBackgroundString != null ? ThirdLevelBodyBackgroundString : "";
            string ThirdLevelFontWeightString = await _dbParams.GetParameterAsync(Parameters.ThirdLevelFontWeight);
            ThirdLevelFontWeight = ThirdLevelFontWeightString != null ? ThirdLevelFontWeightString : "";
            string ThirdLevelMarginTopString = await _dbParams.GetParameterAsync(Parameters.ThirdLevelMarginTop);
            ThirdLevelMarginTop = (int.TryParse(ThirdLevelMarginTopString, out int thirdLevelMarginTop) ? thirdLevelMarginTop : 0).ToString();
            #endregion
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
    }
}
