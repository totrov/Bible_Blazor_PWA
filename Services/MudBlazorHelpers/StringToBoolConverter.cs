using MudBlazor;
using System;

namespace Bible_Blazer_PWA.Services.MudBlazorHelpers
{
    public abstract class StringToBoolConverter : BoolConverter<string>
    {

        public StringToBoolConverter()
        {
            SetFunc = OnSet;
            GetFunc = OnGet;
        }

        protected abstract string GetTrueString();
        protected abstract string GetFalseString();
        protected abstract string GetNullString();

        private string OnGet(bool? value)
        {
            try
            {
                return (value == true) ? GetTrueString() : GetFalseString();
            }
            catch (Exception e)
            {
                UpdateGetError("Conversion error: " + e.Message);
                return GetNullString();
            }
        }

        private bool? OnSet(string arg)
        {
            if (arg == null)
                return null;
            try
            {
                if (arg == GetTrueString())
                    return true;
                if (arg == GetFalseString())
                    return false;
                else
                    return null;
            }
            catch (FormatException e)
            {
                UpdateSetError("Conversion error: " + e.Message);
                return null;
            }
        }

    }
}

