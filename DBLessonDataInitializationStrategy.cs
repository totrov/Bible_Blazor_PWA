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
        private readonly DateTime minimumVersionDate;
        private Func<LessonElementData, int, string, LessonElementData> addChildMethod;

        public DBLessonDataInitializationStrategy(DatabaseJSFacade dbFacade, string unitId, string lessonId, DateTime minimumVersionDate)
        {
            this.db = dbFacade;
            this.unitId = unitId;
            this.lessonId = lessonId;
            this.minimumVersionDate = minimumVersionDate;
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

            if (result.Any() && result.All(lesson => lesson?.VersionDate >= minimumVersionDate))
            {
                PutLessonElementDatasIntoHierarchy(result, lessonElementData);
            }
        }

        private void PutLessonElementDatasIntoHierarchy(IEnumerable<LessonElementDataDb> elements, LessonElementData rootElement)
        {
            rootElement.Value = elements.First().Content;
            LessonElementData lastAddedElement = rootElement;
            Stack<LessonElementData> stack = new();
            stack.Push(rootElement);
            int[] prevIdentifier = new[] { 0, 0, 0 };

            foreach (var element in elements.Skip(1))
            {
                int stackOffset = CalcOffsetAndUpdatePrevIdentifier(prevIdentifier, element.Id);
                if (stackOffset == 1)
                {
                    stack.Push(lastAddedElement);
                    lastAddedElement = addChildMethod(lastAddedElement, stack.Count - 1, element.Content);
                }
                else
                {
                    UpdateStack(stack, stackOffset);
                    lastAddedElement = addChildMethod(stack.Peek(), stack.Count - 1, element.Content);
                }
            }
        }

        private static void UpdateStack(Stack<LessonElementData> stack, int stackOffset)
        {
            for (int i = 0; i > stackOffset; i--)
            {
                stack.Pop();
            }
        }

        private static int CalcOffsetAndUpdatePrevIdentifier(int[] prevIdentifier, int[] currentIdentifier)
        {
            int stackOffset = 0;
            bool isChildOfPrev = false;
            for (int i = 0; i < currentIdentifier.Length; i++)
            {
                if ((prevIdentifier[i] == 0 ^ currentIdentifier[i] == 0) && !isChildOfPrev)
                {
                    if (prevIdentifier[i] == 0)
                    {
                        isChildOfPrev = true;
                    }
                    else
                    {
                        stackOffset--;
                    }
                }
                prevIdentifier[i] = currentIdentifier[i];
            }
            return isChildOfPrev ? 1 : stackOffset;
        }

        public void SetAddChildMethod(Func<LessonElementData, int, string, LessonElementData> addChildMethod)
        {
            this.addChildMethod = addChildMethod;
        }
    }
}