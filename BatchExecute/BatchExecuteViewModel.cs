using BatchExecute.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace BatchExecute
{
	class BatchExecuteViewModel
	{
		public BatchExecuteViewModel()
		{
			BatchFiles = new ObservableCollection<string>(Settings.Default.BatchFiles.Cast<string>());
			DoneBatchFiles = new ObservableCollection<string>(Settings.Default.DoneBatchFiles.Cast<string>());
			WindowStyles = Enum.GetValues(typeof(ProcessRun.ProcessWindowStyle)).Cast<ProcessRun.ProcessWindowStyle>();
		}

		public ObservableCollection<string> BatchFiles { get; }
		public ObservableCollection<string> DoneBatchFiles { get; }
		public IEnumerable<ProcessRun.ProcessWindowStyle> WindowStyles { get; }

		internal void Save()
		{
			Settings.Default.BatchFiles = new System.Collections.Specialized.StringCollection();
			Settings.Default.BatchFiles.AddRange(BatchFiles.ToArray());
			Settings.Default.DoneBatchFiles = new System.Collections.Specialized.StringCollection();
			Settings.Default.DoneBatchFiles.AddRange(DoneBatchFiles.ToArray());
			Settings.Default.Save();
		}

		internal void Redo()
		{
			foreach (var batch in DoneBatchFiles)
			{
				BatchFiles.Add(batch);
			}
			DoneBatchFiles.Clear();
		}

		internal async Task ExecuteBatchesAsync()
		{
			while(BatchFiles.Count > 0)
			{
				var batchFile = BatchFiles[0];
				await Task.Run(() => ProcessRun.RunAndWait(batchFile, 1000, true, BatchWindowStyle));
				BatchFiles.RemoveAt(0);
				DoneBatchFiles.Add(batchFile);
			}
		}

		private ProcessRun.ProcessWindowStyle BatchWindowStyle => (ProcessRun.ProcessWindowStyle)Settings.Default.WindowStyle;
	}
}
