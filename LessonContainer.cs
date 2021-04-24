namespace Bible_Blazer_PWA
{
    public class LessonContainer
    {
        public string LinkName { get; set; }
        public string Content { get; set; }
        public string[] getLines()
        {
            return Content.Split("<br>");
        }
    }
}
