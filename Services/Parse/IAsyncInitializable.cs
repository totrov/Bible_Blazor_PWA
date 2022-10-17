using System.Threading.Tasks;

namespace Bible_Blazer_PWA.Services.Parse
{
    internal interface IAsyncInitializable
    {
        public Task InitTask { get; }
    }
}