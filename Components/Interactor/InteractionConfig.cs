using System.Collections.Generic;

namespace Bible_Blazer_PWA.Components.Interactor
{
    public class InteractionConfig
    {
        public Dictionary<int, string[]> SizeMap { get; init; }
        public InteractionConfig()
        {
            SizeMap = new()
            {
                { 2, new []{ $"20%", $"80%" } },
                { 3, new []{ $"13.3333333%", $"86.666667%" } },
                { 4, new []{ $"10%", $"90%" } },
                { 5, new []{ $"8%", $"92%" } },
                { 6, new []{ $"6.66666667%", $"93.3333333%" } },
                { 7, new []{ $"5.71428571%", $"94.28571429%" } },
                { 8, new []{ $"5%" , $"95%" } }
            };
        }
    }
}
