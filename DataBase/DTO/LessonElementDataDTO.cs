using System;

namespace Bible_Blazer_PWA.DataBase.DTO
{
    public class LessonElementDataDTO
    {
        public int[] Id { get; set; }
        public string UnitId { get; set; }
        public string LessonId { get; set; }
        public string Content { get; set; }
        public DateTime VersionDate { get; set; }
    }
}
