using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace Bible_Blazer_PWA.DataBase
{
    public class DatabaseJSFacade
    {
        public IJSRuntime JS { get; set; }//TODO exposed for debugging purposes. TB Encapsulated.
        internal bool IsInitialized
        {
            get
            {
                return _isInitialized;
            }
            set
            {
                _isInitialized = value;
                JS.InvokeVoidAsync("database.jsLog", $"set {value} value");
            }
        }
        internal bool _isInitialized { get; set; }

        public void JSLog(string str)
        {
            JS.InvokeVoidAsync("database.jsLog", str);
        }

        public void Alert(string str)
        {
            JS.InvokeVoidAsync("database.jsAlert", str);
        }

        public void SetJS(IJSRuntime js)
        {
            JS = js;
        }

        public async Task<IndexedDBResultHandler> CallVoidDbAsync(Action callback, string methodName, params object[] parameters)
        {
            IndexedDBResultHandler resultHandler = new IndexedDBResultHandler();
            resultHandler.OnDbResultOK += callback;
            DotNetObjectReference<IndexedDBResultHandler> resultHandlerReference = DotNetObjectReference.Create(resultHandler);

            if (parameters is not null)
            {
                await JS.InvokeVoidAsync($"database.{methodName}", parameters.Prepend(resultHandlerReference).ToArray());
            }
            else
            {
                await JS.InvokeVoidAsync($"database.{methodName}", resultHandlerReference);
            }

            return resultHandler;
        }
        public virtual async Task<IndexedDBResultHandler> ImportJson(string json, string objectStoreName, Action callback = null)
        {
            return await this.CallVoidDbAsync(callback, "importJson", json, objectStoreName, true);
        }
        public virtual async Task<IndexedDBResultHandler> ClearObjectStore(string objectSotre, Action callback = null)
        {
            return await this.CallVoidDbAsync(callback, "clearObjectStore", objectSotre, "lessons");
        }

        public async Task ImportJsonByURL(string url, string objectStore, Action callback = null)
        {
            await (await this.CallDbAsync<bool>(null, "importJsonByURL", url, objectStore)).GetTaskCompletionSourceWrapper();
        }

        public async Task<IndexedDBResultHandler<T>> CallDbAsync<T>(Action callback, string methodName, params object[] parameters)
        {
            IndexedDBResultHandler<T> resultHandler = new IndexedDBResultHandler<T>();
            resultHandler.OnDbResultOK += callback;
            DotNetObjectReference<IndexedDBResultHandler<T>> resultHandlerReference = DotNetObjectReference.Create(resultHandler);

            if (parameters is not null)
            {
                await JS.InvokeVoidAsync($"database.{methodName}", resultHandlerReference, parameters);
            }
            else
            {
                await JS.InvokeVoidAsync($"database.{methodName}", resultHandlerReference);
            }
            return resultHandler;
        }

        public async Task<IndexedDBResultHandler<T>> GetRecordFromObjectStoreByKey<T>(string objectStoreName, params object[] parameters)
        {
            return await this.CallDbAsync<T>(
                        null, "getRecordFromObjectStoreByKey", objectStoreName, parameters);
        }
        public async Task<IndexedDBResultHandler> DeleteRecordFromObjectStoreByKey(string objectStoreName, params object[] parameters)
        {
            return await CallDbAsync<bool>(
                        null, "deleteRecordFromObjectStoreByKey", objectStoreName, parameters);
        }
        public async Task<IndexedDBResultHandler<IEnumerable<T>>> GetAllFromObjectStore<T>(string objectStoreName)
        {
            return await this.CallDbAsync<IEnumerable<T>>(
                null, "getAllFromObjectStore", objectStoreName);
        }

        public async Task<IndexedDBResultHandler<IEnumerable<T>>> GetRangeFromObjectStoreByKey<T>(string objectStoreName, params object[] parameters)
        {
            return await this.CallDbAsync<IEnumerable<T>>(
                null, "getRangeFromObjectStoreByKey", objectStoreName, parameters);
        }
        public async Task<IndexedDBResultHandler<IEnumerable<T>>> GetRangeFromObjectStoreByIndex<T>(string objectStoreName, string indexName, params object[] parameters)
        {
            return await this.CallDbAsync<IEnumerable<T>>(
                null, "getRangeFromObjectStoreByIndex", objectStoreName, indexName, parameters);
        }
        public async Task<IndexedDBResultHandler<int>> GetCountFromObjectStoreByKey(string objectStoreName, params object[] parameters)
        {
            return await this.CallDbAsync<int>(
                        null, "getCountFromObjectStoreByKey", objectStoreName, parameters);
        }

        public async Task<IndexedDBResultHandler<bool>> SetKeyValueIntoObjectStore(string objectStoreName, object key, object value)
        {
            return await this.CallDbAsync<bool>(
                    null, "putKeyValueIntoObjectStore", objectStoreName, key, value);

        }
        public async Task<IndexedDBResultHandler<bool>> StartPutIntoObjectStore(string objectStoreName, object obj)
        {
            return await this.CallDbAsync<bool>(
                    null, "putIntoObjectStore", objectStoreName, obj);

        }
        public async Task<IndexedDBResultHandler<T>> StartPutIntoAutoincrementedObjectStore<T>(string objectStoreName, string key, object obj)
        {
            return await this.CallDbAsync<T>(
                    null, "putIntoAutoincrementedObjectStore", objectStoreName, key, obj);

        }
    }

    public class IndexedDBResultHandler
    {
        protected void DbResultOK() => OnDbResultOK?.Invoke();
        protected void Fail() => OnFail?.Invoke();
        public event Action OnDbResultOK;
        public event Action OnFail;

        [JSInvokable("SetStatus")]
        public void SetResult(bool _status)
        {
            if (_status)
            {
                DbResultOK();
            }
            else
            {
                Fail();
            }
        }

        public Task GetTaskCompletionSourceWrapper()
        {
            TaskCompletionSource taskCompletionSource = new TaskCompletionSource();
            OnDbResultOK += () => { taskCompletionSource.SetResult(); };
            OnFail += () => { taskCompletionSource.SetException(new Exception("Failed")); };
            return taskCompletionSource.Task;
        }
    }

    public class IndexedDBResultHandler<T> : IndexedDBResultHandler
    {
        public T Result { get; private set; }

        [JSInvokable("SetStatusAndResult")]
        public void SetResult(bool _status, T _result)
        {
            if (_status)
            {
                Result = _result;
            }
            base.SetResult(_status);
        }

        public async Task<T> GetTaskCompletionSourceWrapper()
        {
            TaskCompletionSource taskCompletionSource = new TaskCompletionSource();
            OnDbResultOK += () => { taskCompletionSource.SetResult(); };
            await taskCompletionSource.Task;
            return Result;
        }
    }
}
