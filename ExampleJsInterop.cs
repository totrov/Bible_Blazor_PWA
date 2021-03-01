using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bible_Blazer_PWA
{
    public class ExampleJsInterop : IDisposable
    {
        private readonly IJSRuntime jsRuntime;
        private DotNetObjectReference<HelloHelper> objRef;

        public ExampleJsInterop(IJSRuntime jsRuntime)
        {
            this.jsRuntime = jsRuntime;
        }

        public ValueTask<string> CallHelloHelperSayHello(string name)
        {
            objRef = DotNetObjectReference.Create(new HelloHelper(name));

            return jsRuntime.InvokeAsync<string>(
                "exampleJsFunctions.sayHello",
                objRef);
        }

        public void Dispose()
        {
            objRef?.Dispose();
        }
    }
}
