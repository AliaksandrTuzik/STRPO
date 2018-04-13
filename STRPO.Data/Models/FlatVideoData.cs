using STRPO.Data.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace STRPO.Data.Models
{
    public class FlatVideoData
    {
        #region Statistics
        //public ulong ViewCount { get; set; }
        //public ulong LikeCount { get; set; }
        //public ulong DislikeCount { get; set; }
        //public ulong FavoriteCount { get; set; }
        //public ulong CommentCount { get; set; }
        #endregion Statistics

        #region ContentDetails
        #endregion ContentDetails

        #region Snippet
        #region PublishedAt
        public int PublishedAtHour { get; set; }
        public int PublishedAtDay { get; set; }
        public int PublishedAtMonth { get; set; }
        public int PublishedAtYear { get; set; }
        #endregion PublishedAt
        public string ChannelId { get; set; }
        public string Title { get; set; }
        //public IEnumerable<string> Description { get; set; }
        public string ChannelTitle { get; set; }
        public IEnumerable<string> Tags { get; set; }
        //[Ignore]
        public string CategoryId { get; set; }
        [Ignore]
        public Color Mark { get; set; }
        public string LiveBroadcastContent { get; set; }
        public string DefaultLanguage { get; set; }
        public string DefaultAudioLanguage { get; set; }
        #endregion Snippet
    }
}
