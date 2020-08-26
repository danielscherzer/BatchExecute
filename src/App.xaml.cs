using System.Windows;

namespace BatchExecute
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{

		public App()
		{
		}

		private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
		{
			MessageBox.Show(e.Exception.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
		}
	}
}
