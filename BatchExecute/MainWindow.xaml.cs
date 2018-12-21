using BatchExecute.Properties;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

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
			DataContext = this;
			BatchFiles = new ObservableCollection<string>(Settings.Default.BatchFiles.Cast<string>());
			DoneBatchFiles = new ObservableCollection<string>(Settings.Default.DoneBatchFiles.Cast<string>());
			WindowStyles = new ObservableCollection<string>(Enum.GetNames(typeof(ProcessRun.ProcessWindowStyle)));
		}

		public ObservableCollection<string> BatchFiles { get; }
		public ObservableCollection<string> DoneBatchFiles { get; }
		public ObservableCollection<string> WindowStyles { get; }

		private void ClearBatchFiles(object sender, RoutedEventArgs e) => BatchFiles.Clear();

		private void ClearDoneBatchFiles(object sender, RoutedEventArgs e) => DoneBatchFiles.Clear();

		private void File_Drop(object sender, DragEventArgs e)
		{
			var fileNames = (string[])e.Data.GetData(DataFormats.FileDrop);
			foreach (var fileName in fileNames)
			{
				BatchFiles.Add(fileName);
			}
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			Settings.Default.BatchFiles = new System.Collections.Specialized.StringCollection();
			Settings.Default.BatchFiles.AddRange(BatchFiles.ToArray());
			Settings.Default.DoneBatchFiles = new System.Collections.Specialized.StringCollection();
			Settings.Default.DoneBatchFiles.AddRange(DoneBatchFiles.ToArray());
			Settings.Default.Save();
		}
	}
}
