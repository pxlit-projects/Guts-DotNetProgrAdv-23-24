using System.Windows;
using Exercise2.ViewModel;

namespace Exercise2
{
    public partial class MainWindow : Window
    {
        private readonly IMainViewModel _viewModel;

        public MainWindow(IMainViewModel viewModel)
        {
            InitializeComponent();

            _viewModel = viewModel;
            DataContext = _viewModel;
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel.Load();
        }
    }
}
