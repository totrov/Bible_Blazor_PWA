using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bible_Blazer_PWA.DataSources
{
    public class LessonDS
    {
        private LinkedList<LessonBlock> _blocks;
        private readonly DataBase.DatabaseJSFacade db;

        public class LessonInfo
        {
            public string UnitId { get; set; }
            public string Id { get; set; }
            public string Name { get; set; }
            public string Content { get; set; }
        }

        public class LessonBlock
        {
            public string Name { get; set; }
            public IEnumerable<LessonInfo> Lessons { get; set; }
        }

        public class LessonUnit
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }

        public LessonDS(DataBase.DatabaseJSFacade _facade)
        {
            db = _facade;
        }

        public async Task<LinkedList<LessonBlock>> GetBlocks()
        {
            if (_blocks is null)
            {
                TaskCompletionSource<IEnumerable<LessonUnit>> tcs = new TaskCompletionSource<IEnumerable<LessonUnit>>();

                var lessonUnitsResult = await db.GetAllFromObjectStore<LessonUnit>("lessonUnits");
                lessonUnitsResult.OnDbResultOK += () => { tcs.SetResult(lessonUnitsResult.Result); };
                var lessonUnits = await tcs.Task;

                _blocks = new LinkedList<LessonBlock>();
                foreach (var lessonUnit in lessonUnits)
                {
                    _blocks.AddLast(new LessonBlock
                    {
                        Name = lessonUnit.Name,
                        Lessons = (await this.GetLessonsForBlock(lessonUnit.Id)).OrderBy(x => Convert.ToInt32(x.Id))
                    });
                }
            }
            return _blocks;
        }

        private async Task<IEnumerable<LessonInfo>> GetLessonsForBlock(string unitId)
        {
            TaskCompletionSource<IEnumerable<LessonInfo>> tcs = new TaskCompletionSource<IEnumerable<LessonInfo>>();

            var lessonsResult = await db.GetRangeFromObjectStoreByKey<LessonInfo>("lessons", unitId, "0", "999999");
            lessonsResult.OnDbResultOK += () => { tcs.SetResult(lessonsResult.Result); };
            return await tcs.Task;
        }
    }
}

