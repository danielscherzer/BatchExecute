using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;

namespace BatchExecute
{
	internal class DeleteSelectedCommand : ICommand
	{
		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}

		public bool CanExecute(object parameter)
		{
			return true;
			//var listBox = parameter as ListBox;
			//if (listBox is null) return false;
			//return -1 != listBox.SelectedIndex;
		}

		public void Execute(object parameter)
		{
			var listBox = parameter as ListBox;
			if (listBox is null) return;
			var source = listBox.ItemsSource as IList<string>;
			if (source is null) return;
			do
			{
				var i = listBox.SelectedIndex;
				if (-1 == i) break;
				source.RemoveAt(i);
			} while (true);
		}
	}
}
