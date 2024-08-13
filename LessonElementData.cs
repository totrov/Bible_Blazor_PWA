using Bible_Blazer_PWA.DataBase;
using Bible_Blazer_PWA.DataBase.DTO;
using Bible_Blazer_PWA.DomainObjects;
using Bible_Blazer_PWA.Facades;
using Bible_Blazer_PWA.Services.Parse;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;

namespace Bible_Blazer_PWA
{
    internal static class ElementDataProcessor
    {
        internal static string ProcessFirstLevel(string input, string firstLevelMatch)
        {
            if (firstLevelMatch == "000)")
                return input.Replace("000)", "");
            return input;
        }
    }
    public class LessonElementData : IAsyncInitializable
    {
        public string UnitId { get; set; }
        public string LessonId { get; set; }
        public string Value { get; set; }
        public LinkedList<LessonElementData> Children { get; set; }
        public ReadOnlyCollection<NoteDTO> Notes { get; private set; }
        private List<NoteDTO> NotesInternal { get; set; }
        public int Level { get; set; }
        public int[] Key { get; set; }

        public Task InitTask => initializationTask;

        private Task initializationTask;
        protected LessonElementData(int level, string value, int[] id, string unitId, string lessonId)
        {
            Level = level;
            Value = value;
            Key = id;
            UnitId = unitId;
            LessonId = lessonId;
        }


        private LessonElementData AddChild(LessonElementData parent, int level, int[] id, string value)
        {
            parent.Children ??= new LinkedList<LessonElementData>();
            var newChild = new LessonElementData(level, value, id, parent.UnitId, parent.LessonId);
            parent.Children.AddLast(newChild);
            return newChild;
        }

        public LessonElementData(string unitId, string lessonId, ILessonDataInitializationStrategy initialization)
        {
            UnitId = unitId;
            LessonId = lessonId;
            initialization.SetAddChildMethod(AddChild);
            initializationTask = initialization.Initialize(this);
        }

        public async Task<NoteDTO> AddNoteByValue(string value, DatabaseJSFacade db)
        {
            NoteDTO note = new NoteDTO(value, UnitId, LessonId, Key);
            await note.SaveToDbAsync(db);

            return AddNote(note);
        }

        public NoteDTO AddNote(NoteDTO note)
        {
            NotesInternal ??= new();
            NotesInternal.Add(note);
            
            note.OnAfterRemoval += () => NotesInternal.Remove(note);
            Notes ??= new(NotesInternal);
            return note;
        }

        public static async Task<LessonElementData> GetLessonCompositeAsync(int unitNumber, int id, DatabaseJSFacade db, HttpClient http)
        {
            var unitId = Unit.GetShortNameByUnitNumber(unitNumber);
            var idStringified = id.ToString();
            var versionDate = await new HttpFacade(http).GetVersionDateAsync();

            LessonElementData lessonElement = new(unitId, idStringified, new DBLessonDataInitializationStrategy(db, versionDate));
            await lessonElement.InitTask;
            if (!string.IsNullOrEmpty(lessonElement.Value))
            {
                return lessonElement;
            }

            var resultHandler = await db.GetRecordFromObjectStoreByKey<LessonDTO>("lessons", unitId, idStringified);
            var result = await resultHandler.GetTaskCompletionSourceWrapper();
            var ret = result.GetComposite(new DatabaseLessonElementDataStagingImplementer(db));
            await ret.InitTask;
            return ret;
        }

        public static string GetAnchor(int[] key) => $"le-{key[0]}-{key[1]}-{key[2]}";
        public string GetAnchor() => GetAnchor(Key);
    }
}
