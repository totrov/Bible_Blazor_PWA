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
        private IndexedDBResultHandler resultHandler;
        private DotNetObjectReference<IndexedDBResultHandler> resultHandlerReference;

        public async Task<string> Test()
        {
            return await JS.InvokeAsync<string>("database.showPrompt");
        }

        public bool IsInitialized { get { return _isInitialized; } }

        public async Task Init(IJSRuntime js)
        {
            JS = js;
            resultHandler = new IndexedDBResultHandler(this);
            resultHandlerReference = DotNetObjectReference.Create(resultHandler);
            await JS.InvokeVoidAsync("database.initDatabase", resultHandlerReference);
        }
    }

    public class IndexedDBResultHandler
    {
        private DatabaseJSFacade db;
        public IndexedDBResultHandler(DatabaseJSFacade _db)
        {
            db = _db;
        }

        [JSInvokable]
        public void SetResult(bool _result)
        {
            db.Result = _result;
            if (_result)
            {
                db._isInitialized = true;
            }
        }
    }
}
