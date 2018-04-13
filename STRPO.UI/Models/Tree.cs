using System.Collections.Generic;

namespace STRPO.UI.Models
{
    public class Tree
    {
        public bool IsCluster { get; internal set; }
        public object Value { get; internal set; }
        internal IEnumerable<Tree> Children { get; set; }
    }
}
