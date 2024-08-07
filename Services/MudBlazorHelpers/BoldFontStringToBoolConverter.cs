﻿using MudBlazor;
using System;

namespace Bible_Blazer_PWA.Services.MudBlazorHelpers
{
    public class BoldFontStringToBoolConverter : StringToBoolConverter
    {
        
        protected override string GetTrueString()
        {
            return "bold";
        }
        protected override string GetFalseString()
        {
            return "normal";
        }
        protected override string GetNullString()
        {
            return "normal";
        }
    }
}

