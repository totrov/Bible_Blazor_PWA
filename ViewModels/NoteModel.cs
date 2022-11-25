using Bible_Blazer_PWA.DataBase.DTO;
using System;
using System.Linq.Expressions;

namespace Bible_Blazer_PWA.ViewModels
{
    public class NoteModel : NoteDtoSetter<NoteModel>
    {
        public NoteModel(NoteDTO noteDTO) : base(noteDTO)
        {
            OnAfterRemoval += () => { OnPropertyChange = null; };
        }

        public bool IsEditOn { get; set; }
        public bool IsCollapsed { get; set; }
        public event Action OnPropertyChange;

        public override void SetValue<T>(Expression<Func<NoteModel, T>> propertyExpression, T value)
        {
            base.SetValue(propertyExpression, value);
            OnPropertyChange?.Invoke();
        }
    }
}
