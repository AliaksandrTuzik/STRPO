using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STRPO.Data.Models
{
    public class Statistics
    {
        public ulong ViewCount { get; set; }
        public ulong LikeCount { get; set; }
        public ulong DislikeCount { get; set; }
        public ulong FavoriteCount { get; set; }
        public ulong CommentCount { get; set; }
    }
}
