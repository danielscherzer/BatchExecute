using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BatchExecute
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			viewModel = new BatchExecuteViewModel();
			DataContext = viewModel;
		}

		private BatchExecuteViewModel viewModel;

		private void IsSomethingSelected(object sender, CanExecuteRoutedEventArgs e)
		{
			bool CanExecute()
			{
				var listBox = e.Source as ListBox;
				if (listBox is null) return false;
				return -1 != listBox.SelectedIndex;
			}
			e.CanExecute = CanExecute();
		}

		private void CutCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			var listBox = e.Source as ListBox;
			if (listBox is null) return;
			var source = listBox.ItemsSource as IList<string>;
			if (source is null) return;
			var dataString = new StringBuilder();
			do
			{
				var i = listBox.SelectedIndex;
				if (-1 == i) break;
				dataString.AppendLine(source[i]);
				source.RemoveAt(i);
			} while (true);
			Clipboard.SetText(dataString.ToString());
		}

		private void DeleteSelectedCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			var listBox = e.Source as ListBox;
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

		private void PasteCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = Clipboard.ContainsText();
		}

		private void PasteCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			var listBox = e.Source as ListBox;
			if (listBox is null) return;
			var source = listBox.ItemsSource as IList<string>;
			if (source is null) return;
			var dataString = Clipboard.GetText();
			var lines = dataString.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
			var index = listBox.SelectedIndex + 1;
			foreach(var line in lines)
			{
				source.Insert(index, line.Trim());
				index++;
			}
		}

		private void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			var listBox = e.Source as ListBox;
			if (listBox is null) return;
			var source = listBox.ItemsSource as IList<string>;
			if (source is null) return;
			foreach (var item in listBox.SelectedItems)
			{
				string path = Path.GetDirectoryName(item.ToString());
				if (Directory.Exists(path))
				{
					Process.Start("explorer.exe", '"' + path + '"');
				}
			}
		}

		private void RunAllCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			var listBox = e.Source as ListBox;
			if (listBox is null) return;
			var source = listBox.ItemsSource as IList<string>;
			if (source is null) return;
			foreach (var item in listBox.SelectedItems)
			{
				ProcessRun.Run(item.ToString(), viewModel.WindowStyle);
			}
		}

		private void ClearBatchFiles(object sender, RoutedEventArgs e) => viewModel.BatchFiles.Clear();

		private void ClearDoneBatchFiles(object sender, RoutedEventArgs e) => viewModel.DoneBatchFiles.Clear();

		private void File_Drop(object sender, DragEventArgs e)
		{
			var fileNames = (string[])e.Data.GetData(DataFormats.FileDrop);
			foreach (var fileName in fileNames)
			{
				viewModel.BatchFiles.Add(fileName);
			}
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) => viewModel.Save();

		private void Redo(object sender, RoutedEventArgs e) => viewModel.Redo();

		private async void Run(object sender, RoutedEventArgs e) => await viewModel.ExecuteBatchAsync();

		private void Stop(object sender, RoutedEventArgs e) => viewModel.CancelBatch();

		private void IsolatePar2(object sender, RoutedEventArgs e)
		{
			batchList.Select(".par2");
			batchList.InvertSelection();
			batchList.DeleteSelected();
			batchList.Select(".vol");
			batchList.DeleteSelected();
		}

		private void Selection_TextChanged(object sender, TextChangedEventArgs e)
		{
			var textBox = sender as TextBox;
			if (textBox is null) return;
			batchList.Select(textBox.Text);
		}
	}
}
