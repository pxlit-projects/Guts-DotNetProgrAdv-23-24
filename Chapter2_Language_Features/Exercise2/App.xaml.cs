using System.Configuration;
using System.Data;
using System.Windows;

namespace Exercise2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
	protected override void OnStartup(StartupEventArgs e)
   	{
       		var operationFactory = new MathOperationFactory();
	       	var mainWindow = new MainWindow(operationFactory);
	       	mainWindow.Show();
   	}
    }

}
