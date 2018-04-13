using STRPO.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;
using System.Reflection;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Windows.UI;
using STRPO.UI.ViewModels;
using STRPO.Data;
using STRPO.Data.Models;

namespace STRPO.UI.Services
{
    class DataProviderAdapter: ViewModel.IDataProvider<FlatVideoData>
    {
        private readonly DataProvider _dataProvider;

        public DataProviderAdapter(DataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public Task<IEnumerable<FlatVideoData>> GetDataAsync()
        {
            return _dataProvider.GetVideoDataAsync();
        }
    }
}
