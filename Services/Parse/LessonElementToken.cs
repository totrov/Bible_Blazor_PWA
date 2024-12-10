namespace Bible_Blazer_PWA.BibleReferenceParse
{
    public class LessonElementToken
    {
        public string Text { get; set; }
        public TokenType Type { get; set; }
    }

    public class ListItemToken : LessonElementToken
    {
        public ListItemToken()
        {
            Text = "<br /><span>• </span>";
            Type = TokenType.ListItem;
        }
    }
}
