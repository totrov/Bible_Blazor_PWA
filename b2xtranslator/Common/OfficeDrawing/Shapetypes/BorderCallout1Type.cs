using System.Collections.Generic;

namespace b2xtranslator.OfficeDrawing.Shapetypes
{
    [OfficeShapeType(47)]
    public class BorderCallout1Type : ShapeType
    {
        public BorderCallout1Type()
        {
            this.ShapeConcentricFill = true;

            this.Joins = JoinStyle.miter;

            this.Path = "m@0@1l@2@3nfem,l21600,r,21600l,21600xe";

            this.Formulas = new List<string>();
            this.Formulas.Add("val #0");
            this.Formulas.Add("val #1");
            this.Formulas.Add("val #2");
            this.Formulas.Add("val #3");
            this.AdjustmentValues = "8280,24300,-1800,4050";
            this.ConnectorLocations = "@0,@1;10800,0;10800,21600;0,10800;21600,10800";

            this.Handles = new List<Handle>();
            var HandleOne = new Handle
            {
                position = "#0,#1"
            };
            this.Handles.Add(HandleOne);

            var HandleTwo = new Handle
            {
                position = "#2,#3"
            };
            this.Handles.Add(HandleTwo);
        }
    }
}
