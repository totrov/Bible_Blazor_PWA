namespace Bible_Blazer_PWA
{
    public class LessonContainer
    {
        public string LinkName { get; set; }
        public string Content { get; set; }
        private string[] GetLines()
        {
            return Content.Split("<br>");
        }
        public LessonComposite GetComposite()
        {
            return new LessonComposite(this.GetLines()); 
        }
    }
}
