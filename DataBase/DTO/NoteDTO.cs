namespace Bible_Blazer_PWA.DataBase.DTO
{
    public class NoteDTO
    {
        public enum NoteType
        {
            Regular,
            Attention,
            Question
        }

        public string Value { get; init; }
        public string Identifier { get; set; }
        public NoteType Type { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int TextSize { get; set; }
        public string MainColor { get; set; }
        public string TextColor { get; set; }
        public NoteDTO GetNoteDTO() => new()
        {
            Value = Value,
            Identifier = Identifier,
            Type = Type,
            X = X,
            Y = Y,
            TextSize = TextSize,
            MainColor = MainColor,
            TextColor = TextColor
        };
        private NoteDTO() { }
        public NoteDTO(NoteDTO dto)
        {
            Y = dto.Y;
            X = dto.X;
            Identifier = dto.Identifier;
            Value = dto.Value;
            Type = dto.Type;
            TextSize = dto.TextSize;
            MainColor = dto.MainColor;
            TextColor = dto.TextColor;
        }

        public NoteDTO(string value)
        {
            Value = value;
            Type = NoteType.Regular;
            X = 0;
            Y = 0;
            TextSize = 14;
            TextColor = "black";
            MainColor = "gold";
        }
    }
}
