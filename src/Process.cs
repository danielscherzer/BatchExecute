using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;

namespace BatchExecute
{
	public class ProcessRun
	{
		[DllImport("user32.dll")]
		private static extern bool SetForegroundWindow(IntPtr hWnd);
		[DllImport("user32.dll")]
		private static extern IntPtr GetForegroundWindow();

		public enum ProcessWindowStyle
		{
			Normal, Maximized, MinimizedNoFocus
		}

		private static List<Process> GetChildProcesses(int id, IEnumerable<Tuple<int, Process>> allProcesses)
		{
			var output = new List<Process>();
			foreach (var process in allProcesses)
			{
				if (process.Item1 == id)
				{
					//add direct child to output
					output.Add(process.Item2);
					//recursion for child processes of child
					var childPid = process.Item2.Id;
					output.AddRange(GetChildProcesses(childPid, allProcesses));
				}
			}
			return output;
		}

		public static void Run(string fileName_, ProcessWindowStyle ws_)
		{
			try
			{
				using (var process = Start(fileName_, ws_))
				{ }
			}
			catch (Exception)
			{ }
		}

		public static void RunAndWait(string fileName, int idleTimeMsec
			, bool closeAfterIdleTime, ProcessWindowStyle ws, Func<bool> cancel)
		{
			using (var process = Start(fileName, ws))
			{
				if (1 > idleTimeMsec)
				{
					//wait indefinitely for process exit
					process.WaitForExit();
				}
				else
				{
					WaitForIdleTime(process, idleTimeMsec, cancel);
					//has been idle for longer than parameter idle time
					if (!process.HasExited && closeAfterIdleTime)
					{
						//try to gracefully end process
						if (!process.CloseMainWindow())
						{
							process.Kill();
						}
					}
				}
			}
		}

		private static void WaitForIdleTime(Process process, int idleTimeMsec, Func<bool> cancel)
		{
			long TotalTicks()
			{
				//if you do this only once, wait some time after process start to assume children have been created
				var allProcesses = from p in Process.GetProcesses() select new Tuple<int, Process>(GetParentProcess(p.Id), p);
				var processTree = GetChildProcesses(process.Id, allProcesses); //this is time consuming (large enough polling interval), but process could father new child any time
				processTree.Add(process);
				return processTree.Sum((p) => p.HasExited ? 0 : p.TotalProcessorTime.Ticks);
			}
			//polling if process did anything
			const int pollingInterval = 500;
			long lastTickCount = 0;
			for (int idleTime = 0; idleTime < idleTimeMsec; idleTime += pollingInterval)
			{
				var newTickCount = TotalTicks();
				//check if some processing has been done (any cpu ticks used) during polling interval
				if (newTickCount > lastTickCount)
				{
					//not idle during polling interval -> reset idle time
					idleTime = 0;
					lastTickCount = newTickCount;
				}
				if (process.WaitForExit(pollingInterval)) return;
				if (cancel != null) { if (cancel.Invoke()) return; }
			}
		}

		private static int GetParentProcess(int id)
		{
			try
			{
				using (var mo = new ManagementObject("win32_process.handle='" + id.ToString() + "'"))
				{
					mo.Get();
					return Convert.ToInt32(mo["ParentProcessId"]);
				}
			}
			catch
			{
				return 0;
			}
		}

		private static Process Start(string fileName_, ProcessWindowStyle ws_)
		{
			ProcessStartInfo psi = new(fileName_)
			{
				RedirectStandardOutput = false,
				UseShellExecute = true
			};
			psi.WindowStyle = ws_ switch
			{
				ProcessWindowStyle.Maximized => System.Diagnostics.ProcessWindowStyle.Maximized,
				ProcessWindowStyle.MinimizedNoFocus => System.Diagnostics.ProcessWindowStyle.Minimized,
				_ => System.Diagnostics.ProcessWindowStyle.Normal,
			};
			try
			{
				IntPtr hwnd = GetForegroundWindow();
				Process process = Process.Start(psi);
				if (ProcessWindowStyle.MinimizedNoFocus == ws_)
				{
					process.WaitForInputIdle();
					SetForegroundWindow(hwnd);
				}
				return process;
			}
			catch (Exception)
			{
				return null;
			}
		}
	}
}
