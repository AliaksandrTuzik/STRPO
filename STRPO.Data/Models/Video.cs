using STRPO.Data.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace STRPO.Data.Models
{
    public class Video
    {
        public Snippet Snippet { get; set; }

        public ContentDetails ContentDetails { get; set; }

        public Statistics Statistics { get; set; }
        [Ignore]
        public Color Mark { get; set; }
    }
}
