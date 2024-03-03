using System.Collections.Generic;

namespace Bible_Blazer_PWA.Components.Interactor
{
    public class InteractionConfig
    {
        public Dictionary<int, decimal[]> SizeMap { get; init; }
        public InteractionConfig()
        {
            SizeMap = new()
            {
                { 2, new []{ 20M, 80M } },
                { 3, new []{ 13.3333333M, 86.666667M } },
                { 4, new []{ 10M, 90M } },
                { 5, new []{ 8M, 92M } },
                { 6, new []{ 6.66666667M, 93.3333333M } },
                { 7, new []{ 5.71428571M, 94.28571429M } },
                { 8, new []{ 5M, 95M } }
            };
        }
    }
}
