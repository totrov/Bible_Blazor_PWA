using DocumentFormat.OpenXml.Office2010.PowerPoint;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Bible_Blazer_PWA.DataBase.DTO
{
    public class NoteDTO : DTOBase<NoteDTO>
    {
        public enum NoteType
        {
            Regular,
            Attention,
            Question
        }
        public string UnitId { get; set; }
        public string LessonId { get; set; }
        public int[] ElementId { get; set; }
        public int Opacity { get; set; }
        public string Value { get; set; }
        [PK(autoIncremented: true)]
        public int? Id { get;  set; }
        public NoteType Type { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int TextSize { get; set; }
        public string MainColor { get; set; }
        public string TextColor { get; set; }

        public event Action OnAfterRemoval;
        public NoteDTO GetNoteDTO() => new()
        {
            Value = Value,
            Id = Id,
            UnitId = UnitId,
            LessonId = LessonId,
            ElementId = ElementId,
            Type = Type,
            X = X,
            Y = Y,
            TextSize = TextSize,
            MainColor = MainColor,
            TextColor = TextColor,
            Width = Width,
            Height = Height,
        };

        public NoteDTO() { }
        public NoteDTO(NoteDTO dto)
        {
            Y = dto.Y;
            X = dto.X;
            Id = dto.Id;
            UnitId = dto.UnitId;
            LessonId = dto.LessonId;
            ElementId = dto.ElementId;
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

        public NoteDTO(string value, string unitId, string lessonId, int[] elementId)
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
            UnitId = unitId;
            LessonId = lessonId;
            ElementId = elementId;
        }
        public async Task Remove(DatabaseJSFacade db)
        {
            await RemoveFromDbAsync(db);
            OnAfterRemoval?.Invoke();
            OnAfterRemoval = null;
        }

        protected override string GetObjectStoreName()
        {
            return "notes";
        }
    }

    public class NoteDtoSetter<This> : NoteDTO where This : NoteDTO
    {
        readonly NoteDTO noteDTO;
        public NoteDtoSetter(NoteDTO noteDTO) : base(noteDTO)
        {
            this.noteDTO = noteDTO;
        }
        public virtual PropertyInfo SetValue<T>(Expression<Func<This, T>> propertyExpression, T value)
        {
            if (propertyExpression.Body is not MemberExpression memberexpression)
            {
                throw new Exception("lambda for parsing must be presented as member expression");
            }
            GetType().GetProperty(memberexpression.Member.Name)?.SetValue(this, value);
            var prop = typeof(NoteDTO).GetProperty(memberexpression.Member.Name);
            prop?.SetValue(noteDTO, value);
            return prop;
        }
    }

}
