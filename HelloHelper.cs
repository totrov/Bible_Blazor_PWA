using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bible_Blazer_PWA
{
    public class HelloHelper
    {
        public HelloHelper(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        [JSInvokable]
        public string SayHello() => $"Hello, {Name}!";
    }
}
