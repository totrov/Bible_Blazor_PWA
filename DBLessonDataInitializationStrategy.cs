using Bible_Blazer_PWA.DataBase;
using Bible_Blazer_PWA.DataBase.DTO;
using Bible_Blazer_PWA.Services.Parse;
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
        private Func<LessonElementData, int, int[], string, LessonElementData> addChildMethod;

        public DBLessonDataInitializationStrategy(DatabaseJSFacade dbFacade, DateTime minimumVersionDate)
        {
            db = dbFacade;
            this.minimumVersionDate = minimumVersionDate;
        }
        public async Task Initialize(LessonElementData lessonElementData)
        {
            var resultHandler = await db.GetRangeFromObjectStoreByKey<LessonElementDataDTO>(
                "lessonElementData",
                lessonElementData.UnitId,
                lessonElementData.LessonId,
                new[] { 0, 0, 0 }, new[] { 99, 99, 99 }
                );
            var result = await resultHandler.GetTaskCompletionSourceWrapper();

            if (result.Any() && result.All(lesson => lesson?.VersionDate >= minimumVersionDate))
            {
                PutLessonElementDatasIntoHierarchy(result, lessonElementData);
            }
        }

        private void PutLessonElementDatasIntoHierarchy(IEnumerable<LessonElementDataDTO> elements, LessonElementData rootElement)
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
                    lastAddedElement = addChildMethod(
                        lastAddedElement,
                        stack.Count - 1,
                        element.Id,
                        element.Content);
                }
                else
                {
                    UpdateStack(stack, stackOffset);
                    lastAddedElement = addChildMethod(
                        stack.Peek(),
                        stack.Count - 1,
                        element.Id,
                        element.Content);
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

        public void SetAddChildMethod(Func<LessonElementData, int, int[], string, LessonElementData> addChildMethod)
        {
            this.addChildMethod = addChildMethod;
        }
    }
}