using Bible_Blazer_PWA.Shared;
using System.Threading.Tasks;

namespace Bible_Blazer_PWA.DomainObjects
{
    public class ParametersModel
    {
        DbParametersFacade _dbParams;
        public void SetDbParametersFacade(DbParametersFacade dbParams)
        {
            _dbParams = dbParams;
        }

        public async Task InitFromDb()
        {
            string mainBackgroundParameterString = await _dbParams.GetParameterAsync(DbParametersFacade.Parameters.MainBackground);
            MainBackground = mainBackgroundParameterString != null ? mainBackgroundParameterString : "";

            string ToolsBackgroundString = await _dbParams.GetParameterAsync(DbParametersFacade.Parameters.ToolsBg);
            ToolsBackground = ToolsBackgroundString != null ? ToolsBackgroundString : "";

            string hideToolsParameterString = await _dbParams.GetParameterAsync(DbParametersFacade.Parameters.HideTools);
            AreToolsHidden = hideToolsParameterString != null && hideToolsParameterString == "True";

            #region FirstLevel
            string FirstLevelBackgroundString = await _dbParams.GetParameterAsync(DbParametersFacade.Parameters.FirstLevelBg);
            FirstLevelBackground = FirstLevelBackgroundString != null ? FirstLevelBackgroundString : "";
            string FirstLevelBodyBackgroundString = await _dbParams.GetParameterAsync(DbParametersFacade.Parameters.FirstLevelBodyBg);
            FirstLevelBodyBackground = FirstLevelBodyBackgroundString != null ? FirstLevelBodyBackgroundString : "";
            string FirstLevelFontWeightString = await _dbParams.GetParameterAsync(DbParametersFacade.Parameters.FirstLevelFontWeight);
            FirstLevelFontWeight = FirstLevelFontWeightString != null ? FirstLevelFontWeightString : "";
            string FirstLevelMarginTopString = await _dbParams.GetParameterAsync(DbParametersFacade.Parameters.FirstLevelMarginTop);
            FirstLevelMarginTop = int.TryParse(FirstLevelMarginTopString, out int firstLevelMarginTop) ? firstLevelMarginTop : 0;
            #endregion

            #region SecondLevel
            string SecondLevelBackgroundString = await _dbParams.GetParameterAsync(DbParametersFacade.Parameters.SecondLevelBg);
            SecondLevelBackground = SecondLevelBackgroundString != null ? SecondLevelBackgroundString : "";
            string SecondLevelBodyBackgroundString = await _dbParams.GetParameterAsync(DbParametersFacade.Parameters.SecondLevelBodyBg);
            SecondLevelBodyBackground = SecondLevelBodyBackgroundString != null ? SecondLevelBodyBackgroundString : "";
            string SecondLevelFontWeightString = await _dbParams.GetParameterAsync(DbParametersFacade.Parameters.SecondLevelFontWeight);
            SecondLevelFontWeight = SecondLevelFontWeightString != null ? SecondLevelFontWeightString : "";
            string SecondLevelMarginTopString = await _dbParams.GetParameterAsync(DbParametersFacade.Parameters.SecondLevelMarginTop);
            SecondLevelMarginTop = int.TryParse(SecondLevelMarginTopString, out int secondLevelMarginTop) ? secondLevelMarginTop : 0;
            #endregion

            #region ThirdLevel
            string ThirdLevelBackgroundString = await _dbParams.GetParameterAsync(DbParametersFacade.Parameters.ThirdLevelBg);
            ThirdLevelBackground = ThirdLevelBackgroundString != null ? ThirdLevelBackgroundString : "";
            string ThirdLevelBodyBackgroundString = await _dbParams.GetParameterAsync(DbParametersFacade.Parameters.ThirdLevelBodyBg);
            ThirdLevelBodyBackground = ThirdLevelBodyBackgroundString != null ? ThirdLevelBodyBackgroundString : "";
            string ThirdLevelFontWeightString = await _dbParams.GetParameterAsync(DbParametersFacade.Parameters.ThirdLevelFontWeight);
            ThirdLevelFontWeight = ThirdLevelFontWeightString != null ? ThirdLevelFontWeightString : "";
            string ThirdLevelMarginTopString = await _dbParams.GetParameterAsync(DbParametersFacade.Parameters.ThirdLevelMarginTop);
            ThirdLevelMarginTop = int.TryParse(ThirdLevelMarginTopString, out int thirdLevelMarginTop) ? thirdLevelMarginTop : 0;
            #endregion
        }



        #region MainBackground
        private string _mainBackground;
        public string MainBackground
        {
            get => _mainBackground;
            set
            {
                _dbParams.SetParameterAsync(DbParametersFacade.Parameters.MainBackground, value.ToString());
                _mainBackground = value;
            }
        }
        #endregion

        #region ToolsBackground
        private string _toolsBackground;
        public string ToolsBackground
        {
            get => _toolsBackground;
            set
            {
                _dbParams.SetParameterAsync(DbParametersFacade.Parameters.ToolsBg, value.ToString());
                _toolsBackground = value;
            }
        }
        #endregion

        #region AreToolsHidden
        private bool _areToolsHidden;
        public bool AreToolsHidden
        {
            get => _areToolsHidden; set
            {
                _dbParams.SetParameterAsync(DbParametersFacade.Parameters.HideTools, value.ToString());
                _areToolsHidden = value;
            }
        }
        #endregion

        #region FirstLevel
        #region FirstLevelMarginTop
        private int _firstLevelMarginTop;
        public int FirstLevelMarginTop
        {
            get => _firstLevelMarginTop;
            set
            {
                _dbParams.SetParameterAsync(DbParametersFacade.Parameters.FirstLevelMarginTop, value.ToString());
                _firstLevelMarginTop = value;
            }
        }
        #endregion

        #region FirstLevelFontWeight
        private string _firstLevelFontWeight;
        public string FirstLevelFontWeight
        {
            get => _firstLevelFontWeight;
            set
            {
                _dbParams.SetParameterAsync(DbParametersFacade.Parameters.FirstLevelFontWeight, value.ToString());
                _firstLevelFontWeight = value;
            }
        }
        #endregion

        #region FirstLevelBodyBackground
        private string _firstLevelBodyBackground;
        public string FirstLevelBodyBackground
        {
            get => _firstLevelBodyBackground;
            set
            {
                _dbParams.SetParameterAsync(DbParametersFacade.Parameters.FirstLevelBodyBg, value.ToString());
                _firstLevelBodyBackground = value;
            }
        }
        #endregion

        #region FirstLevelBackground
        private string _firstLevelBackground;
        public string FirstLevelBackground
        {
            get => _firstLevelBackground;
            set
            {
                _dbParams.SetParameterAsync(DbParametersFacade.Parameters.FirstLevelBg, value.ToString());
                _firstLevelBackground = value;
                FirstLevelBodyBackground = value;
            }
        }
        #endregion
        #endregion

        #region SecondLevel
        #region SecondLevelMarginTop
        private int _secondLevelMarginTop;
        public int SecondLevelMarginTop
        {
            get => _secondLevelMarginTop;
            set
            {
                _dbParams.SetParameterAsync(DbParametersFacade.Parameters.SecondLevelMarginTop, value.ToString());
                _secondLevelMarginTop = value;
            }
        }
        #endregion

        #region SecondLevelFontWeight
        private string _secondLevelFontWeight;
        public string SecondLevelFontWeight
        {
            get => _secondLevelFontWeight;
            set
            {
                _dbParams.SetParameterAsync(DbParametersFacade.Parameters.SecondLevelFontWeight, value.ToString());
                _secondLevelFontWeight = value;
            }
        }
        #endregion

        #region SecondLevelBodyBackground
        private string _secondLevelBodyBackground;
        public string SecondLevelBodyBackground
        {
            get => _secondLevelBodyBackground;
            set
            {
                _dbParams.SetParameterAsync(DbParametersFacade.Parameters.SecondLevelBodyBg, value.ToString());
                _secondLevelBodyBackground = value;
            }
        }
        #endregion

        #region SecondLevelBackground
        private string _secondLevelBackground;
        public string SecondLevelBackground
        {
            get => _secondLevelBackground;
            set
            {
                _dbParams.SetParameterAsync(DbParametersFacade.Parameters.SecondLevelBg, value.ToString());
                _secondLevelBackground = value;
                SecondLevelBodyBackground = value;
            }
        }
        #endregion
        #endregion

        #region ThirdLevel
        #region ThirdLevelMarginTop
        private int _thirdLevelMarginTop;
        public int ThirdLevelMarginTop
        {
            get => _thirdLevelMarginTop;
            set
            {
                _dbParams.SetParameterAsync(DbParametersFacade.Parameters.ThirdLevelMarginTop, value.ToString());
                _thirdLevelMarginTop = value;
            }
        }
        #endregion

        #region ThirdLevelFontWeight
        private string _thirdLevelFontWeight;
        public string ThirdLevelFontWeight
        {
            get => _thirdLevelFontWeight;
            set
            {
                _dbParams.SetParameterAsync(DbParametersFacade.Parameters.ThirdLevelFontWeight, value.ToString());
                _thirdLevelFontWeight = value;
            }
        }
        #endregion

        #region ThirdLevelBodyBackground
        private string _thirdLevelBodyBackground;
        public string ThirdLevelBodyBackground
        {
            get => _thirdLevelBodyBackground;
            set
            {
                _dbParams.SetParameterAsync(DbParametersFacade.Parameters.ThirdLevelBodyBg, value.ToString());
                _thirdLevelBodyBackground = value;
            }
        }
        #endregion

        #region ThirdLevelBackground
        private string _thirdLevelBackground;
        public string ThirdLevelBackground
        {
            get => _thirdLevelBackground;
            set
            {
                _dbParams.SetParameterAsync(DbParametersFacade.Parameters.ThirdLevelBg, value.ToString());
                _thirdLevelBackground = value;
                ThirdLevelBodyBackground = value;
            }
        }
        #endregion
        #endregion
    }
}
