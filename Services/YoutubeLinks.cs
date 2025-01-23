using Bible_Blazer_PWA.DataBase.DTO;
using Bible_Blazer_PWA.Facades;
using Bible_Blazer_PWA.Services.Parse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bible_Blazer_PWA.Services
{
    public class YoutubeLinks : IAsyncInitializable
    {
        private readonly HttpFacade http;
        private List<YoutubeLinkDTO> _links;
        private List<YoutubeLinkDTO> links { get => _links = _links ?? new(); set => _links = value; }
        private Task _initTask;
        public Task InitTask => _initTask;

        public YoutubeLinks(HttpFacade http)
        {
            this.http = http;
            _initTask = http.GetYouTubeMapFromJsonAsync().ContinueWith(async (YTLinks) => this.links = new(await YTLinks)); ;
        }

        public IEnumerable<YoutubeLinkDTO> SelectWhere(Func<YoutubeLinkDTO, bool> whereCondition)
            => links.Where(l => whereCondition(l)).Select(link =>
            {
                link.Url = Regex.Replace(link.Url, "[?]si=.*$", "");
                return link;
            });

        public void DeleteWhere(Func<YoutubeLinkDTO, bool> whereCondition)
            => links.RemoveAll(l => whereCondition(l));

        public void AddLink(string name, string urlOrId, string lessonNumber, string unitId, bool isMain, bool addFirst)
        {
            var match = Regex.Match(urlOrId, @"(?:^|\W)(?:youtube(?:-nocookie)?\.com/(?:.*[?&]v=|v/|e(?:mbed)?/|[^/]+/.+/)|youtu\.be/)([\w-]+)")?.Groups?.Values?.LastOrDefault();
            if (addFirst)
            {
                links.Insert(0, new YoutubeLinkDTO
                {
                    Url = "https://www.youtube.com/embed/" + match?.Value ?? string.Empty,
                    LessonNumber = lessonNumber,
                    UnitId = unitId,
                    Name = name,
                    IsMain = isMain
                });
            }
            else
            {
                links.Add(new YoutubeLinkDTO
                {
                    Url = "https://www.youtube.com/embed/" + match?.Value ?? string.Empty,
                    LessonNumber = lessonNumber,
                    UnitId = unitId,
                    Name = name,
                    IsMain = isMain
                });
            }
        }
    }
}