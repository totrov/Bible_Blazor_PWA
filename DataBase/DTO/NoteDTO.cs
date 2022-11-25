using DocumentFormat.OpenXml.Office2010.PowerPoint;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Linq.Expressions;

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

        public int Opacity { get; set; }
        public string Value { get; protected set; }
        public string Identifier { get; protected set; }
        public NoteType Type { get; protected set; }
        public int X { get; protected set; }
        public int Y { get; protected set; }
        public int Width { get; protected set; }
        public int Height { get; protected set; }
        public int TextSize { get; protected set; }
        public string MainColor { get; protected set; }
        public string TextColor { get; protected set; }

        public event Action OnAfterRemoval;
        public NoteDTO GetNoteDTO() => new()
        {
            Value = Value,
            Identifier = Identifier,
            Type = Type,
            X = X,
            Y = Y,
            TextSize = TextSize,
            MainColor = MainColor,
            TextColor = TextColor,
            Width = Width,
            Height = Height,
        };

        protected NoteDTO() { }
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
            Width = dto.Width;
            Height = dto.Height;
            Opacity = dto.Opacity;
            OnAfterRemoval += dto.OnAfterRemoval;
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
            Width = 230;
            Height = 150;
            Opacity = 10;
        }
        public void Remove()
        {
            OnAfterRemoval?.Invoke();
            OnAfterRemoval = null;
        }
    }

    public class NoteDtoSetter<This> : NoteDTO where This : NoteDTO
    {
        readonly NoteDTO noteDTO;
        public NoteDtoSetter(NoteDTO noteDTO) : base(noteDTO)
        {
            this.noteDTO = noteDTO;
        }
        public virtual void SetValue<T>(Expression<Func<This, T>> propertyExpression, T value)
        {
            if (propertyExpression.Body is not MemberExpression memberexpression)
            {
                throw new Exception("lambda for parsing must be presented as member expression");
            }
            GetType().GetProperty(memberexpression.Member.Name)?.SetValue(this, value);
            typeof(NoteDTO).GetProperty(memberexpression.Member.Name)?.SetValue(noteDTO, value);
        }
    }

}
