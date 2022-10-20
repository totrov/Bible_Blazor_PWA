using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bible_Blazer_PWA.DataSources
{
    public class LessonDS
    {
        private SortedDictionary<string, LessonBlock> _blocks;
        private readonly DataBase.DatabaseJSFacade db;

        public class LessonContainer
        {
            public LessonInfo info { get; set; }
            public LinkedList<LessonRecordingLink> lessonRecordingLinks { get; set; }
        }

        public class LessonInfo
        {
            public string UnitId { get; set; }
            public string Id { get; set; }
            public string Name { get; set; }
            public string Content { get; set; }
        }
        public class LessonInfoLightweight
        {
            public string UnitId { get; set; }
            public string Id { get; set; }
            public string Name { get; set; }
        }

        public class LessonBlock
        {
            public string Name { get; set; }
            public SortedDictionary<int, LessonInfoLightweight> Lessons { get; set; }
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

        private class BlockNameComparer : IComparer<string>
        {
            private int Order(char firstChar) => firstChar switch
            {
                'Б' => 10,
                'И' => 20,
                'П' => 30,
                'Е' => 40,
                'Д' => 50,
                'О' => 60,
                _ => 9999999
            };
            public int Compare(string x, string y)
            {
                return Order(x[0]) - Order(y[0]);
            }
        }

        public async Task<SortedDictionary<string, LessonBlock>> GetBlocks()
        {
            if (_blocks is null)
            {
                TaskCompletionSource<IEnumerable<LessonUnit>> tcs = new TaskCompletionSource<IEnumerable<LessonUnit>>();

                var lessonUnitsResult = await db.GetAllFromObjectStore<LessonUnit>("lessonUnits");
                lessonUnitsResult.OnDbResultOK += () => { tcs.SetResult(lessonUnitsResult.Result); };
                var lessonUnits = await tcs.Task;

                _blocks = new SortedDictionary<string, LessonBlock>(new BlockNameComparer());
                foreach (var lessonUnit in lessonUnits)
                {
                    _blocks.Add(lessonUnit.Name, new LessonBlock
                    {
                        Name = lessonUnit.Name,
                        Lessons = await this.GetLessonInfoLightweightForBlock(lessonUnit.Id)
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

        public async Task<SortedDictionary<int, LessonInfoLightweight>> GetLessonInfoLightweightForBlock(string unitId)
        {
            SortedDictionary<int, LessonInfoLightweight> ret = new SortedDictionary<int, LessonInfoLightweight>();
            TaskCompletionSource<IEnumerable<LessonInfoLightweight>> tcs = new TaskCompletionSource<IEnumerable<LessonInfoLightweight>>();

            var lessonsResult = await db.GetRangeFromObjectStoreByKey<LessonInfoLightweight>("lessons", unitId, "0", "999999");
            lessonsResult.OnDbResultOK += () => { tcs.SetResult(lessonsResult.Result); };
            var lessons = await tcs.Task;
            foreach (var lesson in lessons)
            {
                ret.Add(Convert.ToInt32(lesson.Id), lesson);
            }
            return ret;
        }
    }

    public class LessonRecordingLink
    {
        public string Author { get; set; }
        Uri url { get; set; }
    }
}

