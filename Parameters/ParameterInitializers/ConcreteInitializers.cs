using System;
using System.Reflection.Metadata.Ecma335;

namespace Bible_Blazer_PWA.Parameters.ParameterInitializers
{
    public class NullToEmptyParameterInitializer : IGenericParameterInitializer
    {
        public virtual string InitParam(string previousValue)
        {
            return previousValue ?? "";
        }
    }

    public class HideToolsParameterInitializer : BooleanParameterInitializer, IConcreteParameterInitializer
    {
        public Parameters Parameter => Parameters.HideTools;

        public override bool DefaultValue()
        {
            return false;
        }
    }

    public class NotesEnabledParameterInitializer : NullableBooleanParameterInitializer, IConcreteParameterInitializer
    {
        public Parameters Parameter => Parameters.NotesEnabled;

        public override bool? DefaultValue()
        {
            return true;
        }
    }

    public class BibleTextAtTheBottomParameterInitializer : BooleanParameterInitializer, IConcreteParameterInitializer
    {
        public Parameters Parameter => Parameters.BibleTextAtTheBottom;

        public override bool DefaultValue()
        {
            return true;
        }
    }
    public class AreReferencesOpenedParameterInitializer : BooleanParameterInitializer, IConcreteParameterInitializer
    {
        public Parameters Parameter => Parameters.AreReferencesOpened;

        public override bool DefaultValue()
        {
            return true;
        }
    }

    public class CollapseLevelParameterInitializer : IntegerParameterInitializer, IConcreteParameterInitializer
    {
        public Parameters Parameter => Parameters.CollapseLevel;

        public override int DefaultValue()
        {
            return 3;
        }
    }

    public class FontSize : IntegerParameterInitializer, IConcreteParameterInitializer
    {
        public Parameters Parameter => Parameters.FontSize;

        public override int DefaultValue()
        {
            return 14;
        }
    }

    public class HideBlocksBordersParameterInitializer : BooleanParameterInitializer, IConcreteParameterInitializer
    {
        public Parameters Parameter => Parameters.HideBlocksBorders;

        public override bool DefaultValue()
        {
            return false;
        }
    }

    public class FirstLevelMarginTopParameterInitializer : IntegerParameterInitializer, IConcreteParameterInitializer
    {
        public Parameters Parameter => Parameters.FirstLevelMarginTop;

        public override int DefaultValue()
        {
            return 15;
        }
    }

    public class SecondLevelMarginTopParameterInitializer : IntegerParameterInitializer, IConcreteParameterInitializer
    {
        public Parameters Parameter => Parameters.SecondLevelMarginTop;

        public override int DefaultValue()
        {
            return 10;
        }
    }

    public class ThirdLevelMarginTopParameterInitializer : IntegerParameterInitializer, IConcreteParameterInitializer
    {
        public Parameters Parameter => Parameters.ThirdLevelMarginTop;

        public override int DefaultValue()
        {
            return 5;
        }
    }

    public class BlocksPaddingParameterInitializer : IConcreteParameterInitializer
    {
        public virtual Parameters Parameter => Parameters.BlocksPadding;

        public string InitParam(string previousValue)
        {
            if (int.TryParse(previousValue, out int i))
                return previousValue;
            string result;

            if (double.TryParse(previousValue, out double d)) //backward compatibility
            {
                result = Math.Round(d * 100, 0).ToString();
            }
            else
            {
                result = "125";
            }
            return result;
        }
    }

    public class BlocksPaddingLeftParameterInitializer : BlocksPaddingParameterInitializer
    {
        public override Parameters Parameter => Parameters.BlocksPaddingLeft;
    }

    public class BlocksPaddingRightParameterInitializer : BlocksPaddingParameterInitializer
    {
        public override Parameters Parameter => Parameters.BlocksPaddingRight;
    }

    public class StartVersesOnANewLineParameterInitializer : BooleanParameterInitializer, IConcreteParameterInitializer
    {
        public Parameters Parameter => Parameters.StartVersesOnANewLine;

        public override bool DefaultValue()
        {
            return true;
        }
    }

    public abstract class BooleanParameterInitializer : NullToEmptyParameterInitializer
    {
        public abstract bool DefaultValue();
        public override string InitParam(string previousValue)
        {
            string curValue = base.InitParam(previousValue);
            if (curValue != string.Empty && (curValue == "True" || curValue == "False"))
                return curValue;

            return DefaultValue() ? "True" : "False";
        }
    }

    public abstract class NullableBooleanParameterInitializer : IGenericParameterInitializer
    {
        public abstract bool? DefaultValue();
        public string InitParam(string previousValue)
        {
            if (string.IsNullOrEmpty(previousValue))
                return "";
            if (bool.TryParse(previousValue, out bool value))
                return previousValue;

            return DefaultValue().ToString();
        }
    }

    public abstract class IntegerParameterInitializer : IGenericParameterInitializer
    {
        public abstract int DefaultValue();
        public string InitParam(string previousValue)
        {
            if (int.TryParse(previousValue, out int value))
                return previousValue;

            return DefaultValue().ToString();
        }
    }

    public abstract class ColorIitializerBase : IGenericParameterInitializer
    {
        public abstract Parameters Parameter { get; }
        public abstract string DefaultValue { get; }

        public string InitParam(string previousValue)
        {
            if (previousValue.StartsWith("#"))
                return previousValue;
            return this.DefaultValue;
        }
    }

    public class ToolsBgParametersInitializer : ColorIitializerBase, IConcreteParameterInitializer
    {
        public override Parameters Parameter => Parameters.ToolsBg;

        public override string DefaultValue => "#594ae2ff";
    }

    public class InteractionButtonsFlags : IntegerParameterInitializer, IConcreteParameterInitializer
    {
        public Parameters Parameter => Parameters.InteractionButtonsFlags;

        public override int DefaultValue()
        {
            return (int)(
                InteractionButtons.Back
                | InteractionButtons.Forward
                | InteractionButtons.ExpandLess
                | InteractionButtons.ExpandMore
                | InteractionButtons.TurnOver
                | InteractionButtons.Close);
        }
    }

    public class BibleRefBgParametersInitializer : ColorIitializerBase, IConcreteParameterInitializer
    {
        public override Parameters Parameter => Parameters.BibleRefBgColor;

        public override string DefaultValue => "#f5f5dc";
    }

    public class BibleRefFontColorParametersInitializer : ColorIitializerBase, IConcreteParameterInitializer
    {
        public override Parameters Parameter => Parameters.BibleRefFontColor;

        public override string DefaultValue => "#212529";
    }

    public class BibleRefHighlightColorParametersInitializer : ColorIitializerBase, IConcreteParameterInitializer
    {
        public override Parameters Parameter => Parameters.BibleRefHighlightColor;
        public override string DefaultValue => "#594ae2ff";
    }
    public class BibleRefVersesNumbersColorParametersInitializer : ColorIitializerBase, IConcreteParameterInitializer
    {
        public override Parameters Parameter => Parameters.BibleRefVersesNumbersColor;
        public override string DefaultValue => "#594ae2ff";
    }

    public class BibleRefVersesIntervalsColorParametersInitializer : ColorIitializerBase, IConcreteParameterInitializer
    {
        public override Parameters Parameter => Parameters.BibleRefVersesIntervalsColor;

        public override string DefaultValue => "#ffffff";
    }
    public class HideBibleRefsTabsParametersInitializer : BooleanParameterInitializer, IConcreteParameterInitializer
    {
        public Parameters Parameter => Parameters.HideBibleRefTabs;

        public override bool DefaultValue()
        {
            return true;
        }
    }    
}
