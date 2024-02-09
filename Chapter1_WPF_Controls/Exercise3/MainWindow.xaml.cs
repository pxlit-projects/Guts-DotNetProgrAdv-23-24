using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Exercise3
{
    public partial class MainWindow : Window
    {
        private readonly double _maximumWidth;
        private const int GrowOrShrinkAmount = 10;

        public MainWindow()
        {
            InitializeComponent();

            _maximumWidth = paperCanvas.Width - fluidRectangle.Margin.Left;
        }

        private void GrowButton_Click(object sender, RoutedEventArgs e)
        {
            fluidRectangle.Width = Math.Min(fluidRectangle.Width + GrowOrShrinkAmount, _maximumWidth);
        }

        private void ShrinkButton_Click(object sender, RoutedEventArgs e)
        {
            fluidRectangle.Width = Math.Max(fluidRectangle.Width - GrowOrShrinkAmount, 0);
        }
    }

}