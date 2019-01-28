using AutoUpdateViaGitHubRelease;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
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
			Task.Run(Update);
		}

		private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
		{
			MessageBox.Show(e.Exception.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
		}

		async Task Update()
		{
			var updater = new Updater();
			var assembly = Assembly.GetEntryAssembly();
			var currentVersion = assembly.GetName().Version;
			var tempDir = Path.GetTempPath();
			try
			{
				var newVersionAvailable = await updater.DownloadNewVersion("danielScherzer", "BatchExecute", currentVersion, tempDir);
				if (newVersionAvailable)
				{
					var destinationDir = Path.GetDirectoryName(assembly.Location);
					updater.StartUpdate(tempDir, destinationDir);
					Shutdown();
				}
			}
			catch
			{
			}
		}
	}
}
