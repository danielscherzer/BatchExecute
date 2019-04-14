using AutoUpdateViaGitHubRelease;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace BatchExecute
{
	class Update : NotifyPropertyChanged
	{
		public Update()
		{
			var assembly = Assembly.GetEntryAssembly();
			currentVersion = assembly.GetName().Version;
			tempDir = Path.GetTempPath();
			destinationDir = Path.GetDirectoryName(assembly.Location);
			Task.Run(DownloadNewVersion);
		}

		public bool Available { get => _available; private set => SetNotify(ref _available, value); }

		public void Execute()
		{
			try
			{
				if (Available)
				{
					updater.StartUpdate(tempDir, destinationDir);
				}
			}
			catch
			{
			}
		}

		private Updater updater = new Updater();
		private bool _available = false;
		private readonly Version currentVersion;
		private readonly string tempDir;
		private readonly string destinationDir;

		async Task DownloadNewVersion()
		{
			try
			{
				Available = await updater.DownloadNewVersion("danielScherzer", "BatchExecute", currentVersion, tempDir);
			}
			catch
			{
			}
		}
	}
}
