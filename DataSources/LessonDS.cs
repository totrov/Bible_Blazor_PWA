using System.Collections.Generic;

namespace Bible_Blazer_PWA.DataSources
{
    public class LessonDS
    {
        private IEnumerable<LessonBlock> _blocks;
        public class LessonInfo
        {
            public int Number { get; set; }
            public string Name { get; set; }
        }
        public class LessonBlock
        {
            public string Name { get; set; }
            public IEnumerable<LessonInfo> Lessons { get; set; }
        }
        public LessonDS()
        {
            _blocks = new LessonBlock[]
            {
                new LessonBlock
                {
                    Name = "Бытие",
                    Lessons = new LessonInfo[]{ 
                        new LessonInfo { Name = "6 урок", Number=6},
                        new LessonInfo { Name = "7 урок", Number=7},
                        new LessonInfo { Name = "8 урок", Number=8},
                        new LessonInfo { Name = "9 урок", Number=9}
                    }                    
                },
                new LessonBlock
                {
                    Name = "Исход - Соломон",
                    Lessons = new LessonInfo[]{
                        new LessonInfo { Name = "6 урок", Number=6},
                        new LessonInfo { Name = "7 урок", Number=7}
                    }                    
                }
                //Пророки
                //Евангелия
                //Деяния - Откровение
                //Основы веры
            };
        }

        public IEnumerable<LessonBlock> GetBlocks()
        {
            return _blocks;
        }
    }
}

