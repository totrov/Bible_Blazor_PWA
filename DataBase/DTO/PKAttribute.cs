using System;

namespace Bible_Blazer_PWA.DataBase.DTO
{
    [AttributeUsage(AttributeTargets.Property)]
    internal class PKAttribute : Attribute
    {
        public PKAttribute(bool autoIncremented)
        {
            AutoIncremented = autoIncremented;
        }
        internal bool AutoIncremented { get; }
    }
}
