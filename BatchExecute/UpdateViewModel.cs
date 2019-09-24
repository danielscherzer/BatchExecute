using AutoUpdateViaGitHubRelease;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using Zenseless.Patterns;

namespace BatchExecute
{
	class UpdateViewModel : NotifyPropertyChanged
	{
		public UpdateViewModel()
		{
			var update = new Update("danielScherzer", "BatchExecute", Assembly.GetExecutingAssembly(), Path.GetTempPath());
			update.PropertyChanged += (s, a) => Available = update.Available;

			void UpdateAndClose()
			{
				update.Install();
				var app = Application.Current;
				app.Shutdown();
			}

			_command = new DelegateCommand(_ => UpdateAndClose(), _ => Available);
		}

		private void SetAvailable()
		{
			Application.Current.Dispatcher.Invoke(() => 
			{
				Available = true;
				_command.RaiseCanExecuteChanged();
			});
		}

		public bool Available { get => _available; private set => SetNotify(ref _available, value, _ => _command.RaiseCanExecuteChanged()); }
		public ICommand Command => _command;

		private bool _available = false;
		private readonly DelegateCommand _command;
	}
}
