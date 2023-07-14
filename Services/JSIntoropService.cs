using Bible_Blazer_PWA.Services.Parse;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Bible_Blazer_PWA.Services
{
    public class JSInteropService
    {
        public event Action OnTurnOverRequired;

        public async Task Init(IJSRuntime JS)
        {
            this.JS = JS;
            DotNetObjectReference<InteropObject> dotNetObjectReference = DotNetObjectReference.Create(new InteropObject(this));
            await JS.InvokeVoidAsync("registerInteropObject", dotNetObjectReference);
        }

        public class InteropObject
        {
            private readonly JSInteropService service;

            public InteropObject(JSInteropService service)
            {
                this.service = service;
            }
            [JSInvokable("FireVoidEvent")]
            public void FireVoidEvent(string eventName)
            {
                switch(eventName)
                {
                    case "TurnOverRequired":
                        service.OnTurnOverRequired?.Invoke();
                        break;
                    default:
                        break;
                }
                
            }
        }

        public IJSRuntime JS { get; private set; }
    }
}
