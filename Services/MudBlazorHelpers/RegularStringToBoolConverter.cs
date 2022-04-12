using MudBlazor;
using System;

namespace Bible_Blazer_PWA.Services.MudBlazorHelpers
{
    public class RegularStringToBoolConverter : StringToBoolConverter
    {
        
        protected override string GetTrueString()
        {
            return "True";
        }
        protected override string GetFalseString()
        {
            return "False";
        }
        protected override string GetNullString()
        {
            return "False";
        }
    }
}

