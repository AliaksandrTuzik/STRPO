using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STRPO.Data.Models
{
    public class Snippet
    {
        public DateTime PublishedAt { get; set; }
        public string ChannelId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        //"thumbnails": {
        //      (key): {
        //        "url": string,
        //        "width": unsigned integer,
        //        "height": unsigned integer
        //      }
        //},
        public string ChannelTitle { get; set; }
        public IEnumerable<string> Tags { get; set; }
        public string CategoryId { get; set; }
        public string LiveBroadcastContent { get; set; }
        public string DefaultLanguage { get; set; }
        //"localized": {
        //  "title": string,
        //  "description": string
        //},
        public string DefaultAudioLanguage { get; set; }
    }
}
