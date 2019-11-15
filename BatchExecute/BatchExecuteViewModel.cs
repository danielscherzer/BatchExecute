using BatchExecute.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Zenseless.Patterns;

namespace BatchExecute
{
	class BatchExecuteViewModel : NotifyPropertyChanged
	{
		public BatchExecuteViewModel()
		{
			BatchFiles = new ObservableCollection<string>(Settings.Default.BatchFiles.Cast<string>());
			DoneBatchFiles = new ObservableCollection<string>(Settings.Default.DoneBatchFiles.Cast<string>());
			WindowStyles = Enum.GetValues(typeof(ProcessRun.ProcessWindowStyle)).Cast<ProcessRun.ProcessWindowStyle>();
		}

		public ObservableCollection<string> BatchFiles { get; }
		public ObservableCollection<string> DoneBatchFiles { get; }
		public ProcessRun.ProcessWindowStyle WindowStyle => (ProcessRun.ProcessWindowStyle)Settings.Default.WindowStyle;
		public bool Run
		{
			get => _running;
			private set => SetNotify(ref _running, value);
		}

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

		internal async Task ExecuteBatchAsync()
		{
			cancel = false;
			Run = true;
			while(BatchFiles.Count > 0)
			{
				var file = BatchFiles[0];
				await Task.Run(() =>
				{
					var idleTime = Settings.Default.DetectIdle ? Settings.Default.IdleWaitTime : 0;
					var close = Settings.Default.CloseAfterIdleTime;
					ProcessRun.RunAndWait(file, idleTime, close, WindowStyle, () => cancel);
				});
				if (cancel) break;
				BatchFiles.RemoveAt(0);
				DoneBatchFiles.Add(file);
			}
			cancel = false;
			Run = false;
		}

		internal void CancelBatch() => cancel = true;

		private volatile bool cancel = false;
		private bool _running = false;
	}
}
