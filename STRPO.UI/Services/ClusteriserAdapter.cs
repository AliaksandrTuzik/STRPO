using STRPO.Clusterisation;
using STRPO.UI.Models;
using STRPO.UI.ViewModels;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using STRPO.Clusterisation.Models;

namespace STRPO.UI.Services
{
    class ClusteriserAdapter : ViewModel.IClusterizer
    {
        private readonly Clusteriser _clusteriser;

        public ClusteriserAdapter(Clusteriser clusteriser)
        {
            _clusteriser = clusteriser;
        }
        public async Task<Tree> ClusterizeAsync<T>(IEnumerable<T> data)
        {
            return Map(await _clusteriser.ClusterizeAsync(data));
        }

        private Tree Map(TreeWithSilhouette treeWithSilhouette)
        {
            if (treeWithSilhouette == null)
            {
                return null;
            }

            return new Tree
            {
                IsCluster = treeWithSilhouette.IsCluster,
                Value = treeWithSilhouette.Value,
                Children = treeWithSilhouette.Children.Select(tree => Map(tree))
            };
        }
    }
}
