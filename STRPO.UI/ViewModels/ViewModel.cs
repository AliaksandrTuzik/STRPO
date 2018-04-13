using STRPO.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STRPO.UI.ViewModels
{
    class ViewModel
    {
        private readonly IClusterizer _clusteriser;
        private readonly IDrawer _drawer;

        public ViewModel(
            IClusterizer clusterizer,
            IDrawer drawer)
        {
            _clusteriser = clusterizer;
            _drawer = drawer;
        }

        public async Task ShowData<TData>(IDataProvider<TData> dataProvider)
        {
            Log.WithTime("Started getting data");
            var data = await dataProvider.GetDataAsync();
            Log.WithTime("Ended getting data");

            Log.WithTime("Started clusterizing data");
            var clusterisedData = await _clusteriser.ClusterizeAsync(data);
            Log.WithTime("Ended clusterizing data");

            Log.WithTime("Started drawing data");
            _drawer.Draw(clusterisedData);
            Log.WithTime("Ended drawing data");
        }

        public interface IDataProvider<TData>
        {
            Task<IEnumerable<TData>> GetDataAsync();
        }

        public interface IClusterizer
        {
            Task<Tree> ClusterizeAsync<T>(IEnumerable<T> data);
        }

        public interface IDrawer
        {
            void Draw(Tree clusterisedData);
        }
    }
}
