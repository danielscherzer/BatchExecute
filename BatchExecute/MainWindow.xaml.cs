using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

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

		private async void RunBatches(object sender, RoutedEventArgs e)
		{
			await viewModel.ExecuteBatchesAsync();
		}

		private void StopBatches(object sender, RoutedEventArgs e)
		{

		}

		private void IsolatePar2(object sender, RoutedEventArgs e)
		{
		}
	}
}
