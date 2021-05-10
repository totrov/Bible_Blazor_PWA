using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bible_Blazer_PWA.DataBase
{
    public class DatabaseJSFacade
    {
        protected IJSRuntime JS { get; set; }
        internal bool Result
        {
            get
            {
                return _isInitialized;
            }
            set {
                _isInitialized = value;
                JS.InvokeVoidAsync("database.jsLog", $"set {value} value");
            }
        }
        internal bool _isInitialized { get; set; }

        public async Task<string> Test()
        {
            return await JS.InvokeAsync<string>("database.showPrompt");
        }

        public bool IsInitialized { get { return _isInitialized; } }

        public async Task<IndexedDBResultHandler> Init(IJSRuntime js)
        {
            JS = js;
            return await CallVoidDbAsync(() => { _isInitialized = true; }, "initDatabase");
        }

        public async Task<IndexedDBResultHandler> CallVoidDbAsync(Action callback, string methodName)
        {
            IndexedDBResultHandler resultHandler = new IndexedDBResultHandler();
            resultHandler.OnDbResultOK += callback;
            DotNetObjectReference<IndexedDBResultHandler> resultHandlerReference = DotNetObjectReference.Create(resultHandler);

            await JS.InvokeVoidAsync($"database.{methodName}", resultHandlerReference);
            return resultHandler;
        }

        public async Task<IndexedDBResultHandler<T>> CallDbAsync<T>(Action callback, string methodName, params object[] parameters)
        {
            IndexedDBResultHandler<T> resultHandler = new IndexedDBResultHandler<T>();
            resultHandler.OnDbResultOK += callback;
            DotNetObjectReference<IndexedDBResultHandler<T>> resultHandlerReference = DotNetObjectReference.Create(resultHandler);

            await JS.InvokeVoidAsync($"database.{methodName}", resultHandlerReference, parameters);
            return resultHandler;
        }
    }

    public class IndexedDBResultHandler
    {
        protected void DbResultOK() => OnDbResultOK?.Invoke();
        public event Action OnDbResultOK;

        [JSInvokable("SetStatus")]
        public void SetResult(bool _status)
        {
            if (_status)
            {
                DbResultOK();
            }
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
    }
}
