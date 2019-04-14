using System.Windows;
using System.Windows.Input;

namespace BatchExecute
{
	class UpdateViewModel : Update
	{
		public UpdateViewModel()
		{
			_command = new DelegateCommand(_ => UpdateAndClose());
		}

		private void UpdateAndClose()
		{
			Execute();
			var app = Application.Current;
			app.Shutdown();
		}

		public ICommand Command => _command;

		private readonly DelegateCommand _command;
	}
}
