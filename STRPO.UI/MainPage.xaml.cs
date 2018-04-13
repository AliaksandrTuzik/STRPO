using STRPO.Data.Models;
using STRPO.UI.Models;
using STRPO.UI.Services;
using STRPO.UI.ViewModels;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace STRPO.UI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly DataProviderAdapter _dataProvider;

        ViewModel ViewModel { get; set; }

        public MainPage()
        {
            InitializeComponent();

            _dataProvider = new DataProviderAdapter(new Data.DataProvider());

            var drawer = new ClusteredDataDrawer(MyCanvas);
            var clusteriser = new ClusteriserAdapter(new Clusterisation.Clusteriser());

            ViewModel = new ViewModel(clusteriser, drawer);
        }

        //private Tree GetTestTree()
        //{
        //    var root = new Tree();

        //    var first = (new Tree()).AddEmptyNodes(2).AddParentNodes(2);

        //    var second = new Tree()
        //        .AddChildren(
        //            new Tree().AddEmptyNodes(2),
        //            new Tree().AddEmptyNodes(3)
        //        );

        //    root.AddChildren(first, second, new Tree());

        //    return root;
        //}

        private async void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            (sender as Button).IsEnabled = false;

            ProgressRing.Visibility = Windows.UI.Xaml.Visibility.Visible;

            await ViewModel.ShowData(_dataProvider);

            ProgressRing.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            (sender as Button).IsEnabled = true;
        }
    }
}
