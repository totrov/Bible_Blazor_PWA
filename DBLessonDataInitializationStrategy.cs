using Bible_Blazer_PWA.DataBase;
using Bible_Blazer_PWA.Services.Parse;
using DocumentFormat.OpenXml.Office2010.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bible_Blazer_PWA
{
    public class DBLessonDataInitializationStrategy : ILessonDataInitializationStrategy
    {
        private readonly DatabaseJSFacade db;
        private readonly string unitId;
        private readonly string lessonId;
        private Func<LessonElementData, int, string, LessonElementData> addChildMethod;

        public DBLessonDataInitializationStrategy(DatabaseJSFacade dbFacade, string unitId, string lessonId)
        {
            this.db = dbFacade;
            this.unitId = unitId;
            this.lessonId = lessonId;
        }
        public async Task Initialize(LessonElementData lessonElementData)
        {
            var resultHandler = await db.GetRangeFromObjectStoreByKey<LessonElementDataDb>(
                "lessonElementData",
                unitId,
                lessonId,
                new[] { 0, 0, 0 }, new[] { 99, 99, 99 }
                );
            var result = await resultHandler.GetTaskCompletionSourceWrapper();

            if (result.Any())
            {
                lessonElementData.Value = result.First().Content;
                Stack<LessonElementData> stack = new();
                stack.Push(lessonElementData);
                int[] prevIdentifier = new[] { 0, 0, 0 };
                foreach (var lessonElement in result.Skip(1))
                {
                    CalcOffsetAndUpdatePrevIdentifier(prevIdentifier, lessonElement.Id);
                    addChildMethod(currentElement, lessonElement.Id[], result..Take(1).First().Content);
                }
            }
        }

        private int CalcOffsetAndUpdatePrevIdentifier(int[] prevIdentifier, int[] currentIdentifier)
        {
            int stackOffset = -1;
            bool previousEqual = true;
            for (int i = 0; i < currentIdentifier.Length; i++)
            {
                if (previousEqual)
                {
                    if (prevIdentifier[i] != currentIdentifier[i])
                    {
                        stackOffset++;
                        prevIdentifier[i] = currentIdentifier[i];
                        previousEqual = false;
                    }
                }
                else
                {
                    stackOffset++;
                    prevIdentifier[i] = currentIdentifier[i];
                }
            }
            return stackOffset;
        }

        public void SetAddChildMethod(Func<LessonElementData, int, string, LessonElementData> addChildMethod)
        {
            this.addChildMethod = addChildMethod;
        }
    }
}
