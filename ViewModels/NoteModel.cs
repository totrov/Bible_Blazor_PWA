using Bible_Blazer_PWA.DataBase.DTO;
using System;
using System.Linq.Expressions;
using System.Reflection;

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
        public event Action OnSave;
        delegate void TypedPropertyChangeHandler<T>();

        public override PropertyInfo SetValue<T>(Expression<Func<NoteModel, T>> propertyExpression, T value)
        {
            var prop = base.SetValue(propertyExpression, value);
            OnPropertyChange?.Invoke();
            if (prop != null && prop.Name != nameof(X) && prop.Name != nameof(Y))
            {
                OnSave?.Invoke();
            }

            return prop;
        }
    }
}
