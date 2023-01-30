using Bible_Blazer_PWA.Pages;
using DocumentFormat.OpenXml.Office2021.DocumentTasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading.Tasks;
using static MudBlazor.CategoryTypes;
using Task = System.Threading.Tasks.Task;

namespace Bible_Blazer_PWA.DataBase.DTO
{
    public abstract class DTOBase<T> where T : DTOBase<T>
    {
        internal Dictionary<PKAttribute, PropertyInfo> PKs;
        public DTOBase()
        {
            foreach (var prop in typeof(T).GetProperties())
            {
                var pk = prop.GetCustomAttributes(true).OfType<PKAttribute>().FirstOrDefault();
                if (pk is not null)
                {
                    PKs ??= new Dictionary<PKAttribute, PropertyInfo>();
                    PKs.Add(pk, prop);
                }
            }
        }
        protected abstract string GetObjectStoreName();
        public async virtual Task SaveToDbAsync(DatabaseJSFacade db)
        {
            var autoIncPKs = PKs.Where(pair => pair.Key.AutoIncremented);
            if (autoIncPKs.Any())
            {
                var PKProperty = autoIncPKs.First().Value;
                var m_StartPutIntoAutoincrementedObjectStore = db.GetType().GetMethod("StartPutIntoAutoincrementedObjectStore")
                    .MakeGenericMethod(PKProperty.PropertyType);
                Type IndexedDBResultHandler_T_unresolved = typeof(IndexedDBResultHandler<>);
                Type IndexedDBResultHandler_T = IndexedDBResultHandler_T_unresolved.MakeGenericType(PKProperty.PropertyType);
                Task startPutTask = (Task)m_StartPutIntoAutoincrementedObjectStore.Invoke(db, new object[]{ GetObjectStoreName(), PKProperty.GetValue(this), this});
                
                await startPutTask;
                var resultProperty = startPutTask.GetType().GetProperty("Result");
                var handler = resultProperty.GetValue(startPutTask);

                var m_GetTaskCompletionSourceWrapper = handler.GetType().GetMethod("GetTaskCompletionSourceWrapper");
                Task taskCompletionSourceWrapperTask = (Task)m_GetTaskCompletionSourceWrapper.Invoke(handler, null);
                await taskCompletionSourceWrapperTask;
                resultProperty = taskCompletionSourceWrapperTask.GetType().GetProperty("Result");
                var key = resultProperty.GetValue(taskCompletionSourceWrapperTask);

                PKProperty.SetValue(this, key);
            }
            else
            {
                var resultHandler = await db.StartPutIntoObjectStore(GetObjectStoreName(), this);
                await resultHandler.GetTaskCompletionSourceWrapper();
            }
        }

        public async virtual Task RemoveFromDbAsync(DatabaseJSFacade db)
        {
            var pkValue = typeof(T).GetProperties().FirstOrDefault(prop => Attribute.IsDefined(prop, typeof(PKAttribute)))?.GetValue(this);
            if (pkValue is null)
            {
                Debug.Assert(false); return;
            }
            var resultHandler = await db.DeleteRecordFromObjectStoreByKey(GetObjectStoreName(), pkValue);
            await resultHandler.GetTaskCompletionSourceWrapper();
        }
    }
}
