using AutoUpdateViaGitHubRelease;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using Zenseless.Patterns;

namespace BatchExecute
{
	internal class UpdateViewModel : NotifyPropertyChanged
	{
		public UpdateViewModel()
		{
			var update = new Update();
			update.PropertyChanged += (s, a) => Available = update.Available;
			var assembly = Assembly.GetExecutingAssembly();
			update.CheckDownloadNewVersionAsync("danielScherzer", "BatchExecute", assembly.GetName().Version, Path.GetTempPath());


			void UpdateAndClose()
			{
				update.StartInstall(Path.GetDirectoryName(assembly.Location));
				var app = Application.Current;
				app.Shutdown();
			}

			_command = new DelegateCommand(_ => UpdateAndClose(), _ => Available);
		}

		public bool Available { get => _available; private set => Set(ref _available, value, _ => _command.RaiseCanExecuteChanged()); }
		public ICommand Command => _command;

		private bool _available = false;
		private readonly DelegateCommand _command;
	}
}
