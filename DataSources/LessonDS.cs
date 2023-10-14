using Bible_Blazer_PWA.DataBase.DTO;
using Bible_Blazer_PWA.Diagnostics;
using Bible_Blazer_PWA.Facades;
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
            public LessonDTO lessonDTO { get; set; }
            public LinkedList<LessonRecordingLink> lessonRecordingLinks { get; set; }
        }

        public class LessonBlock
        {
            public string Name { get; set; }
            public SortedDictionary<int, LessonLightweightDTO> Lessons { get; set; }
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

        internal class BlockNameComparer : IComparer<string>
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

        public async Task<SortedDictionary<string, LessonBlock>> GetBlocks(DBCache cache)
        {
            if (_blocks is null)
            {
                _blocks = new SortedDictionary<string, LessonBlock>(new BlockNameComparer());
                if (await cache.TryPopulateFromCache<SortedDictionary<string, LessonBlock>, LessonBlock>(DBCache.LessonMenuBlocks, _blocks))
                    return _blocks;

                TaskCompletionSource<IEnumerable<LessonUnit>> tcs = new TaskCompletionSource<IEnumerable<LessonUnit>>();

                var lessonUnitsResult = await db.GetAllFromObjectStore<LessonUnit>("lessonUnits");
                lessonUnitsResult.OnDbResultOK += () => { tcs.SetResult(lessonUnitsResult.Result); };
                var lessonUnits = await tcs.Task;

                foreach (var lessonUnit in lessonUnits)
                {
                    _blocks.Add(lessonUnit.Name, new LessonBlock
                    {
                        Name = lessonUnit.Name,
                        Lessons = await this.GetLessonLightweightDTOForBlock(lessonUnit.Id)
                    });
                }
                _ = cache.SetToCache(DBCache.LessonMenuBlocks, _blocks);
            }

            return _blocks;
        }

        private async Task<IEnumerable<LessonDTO>> GetLessonsForBlock(string unitId)
        {
            TaskCompletionSource<IEnumerable<LessonDTO>> tcs = new TaskCompletionSource<IEnumerable<LessonDTO>>();

            var lessonsResult = await db.GetRangeFromObjectStoreByKey<LessonDTO>("lessons", unitId, "0", "999999");
            lessonsResult.OnDbResultOK += () => { tcs.SetResult(lessonsResult.Result); };
            return await tcs.Task;
        }

        public async Task<SortedDictionary<int, LessonLightweightDTO>> GetLessonLightweightDTOForBlock(string unitId)
        {
            SortedDictionary<int, LessonLightweightDTO> ret = new SortedDictionary<int, LessonLightweightDTO>();
            TaskCompletionSource<IEnumerable<LessonLightweightDTO>> tcs = new TaskCompletionSource<IEnumerable<LessonLightweightDTO>>();

            var lessonsResult = await db.GetRangeFromObjectStoreByKey<LessonLightweightDTO>("lessons", unitId, "0", "999999");
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

