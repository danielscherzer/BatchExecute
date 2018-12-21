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
		public static extern bool SetForegroundWindow(IntPtr hWnd);
		[DllImport("user32.dll")]
		public static extern IntPtr GetForegroundWindow();

		public enum ProcessWindowStyle
		{
			Normal, Maximized, MinimizedNoFocus
		}

		public static List<Process> GetDirectChildProcesses(int id)
		{
			var list = new List<Process>();
			Process[] procs = Process.GetProcesses();
			foreach (Process p in procs)
			{
				if (GetParentProcess(p.Id) == id)
				{
					list.Add(p);
				}
			}
			return list;
		}

		public static List<Process> GetChildProcesses(int id)
		{
			var output = new List<Process>();
			var list = GetDirectChildProcesses(id);
			//add direct children to output
			output.AddRange(list);
			foreach (Process p in list)
			{
				var children = GetChildProcesses(p.Id);
				output.AddRange(children);
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

		public static void RunAndWait(string fileName_, int iIdleTimeBeforeCloseMsec_
			, bool bCloseOnIdle_, ProcessWindowStyle ws_)
		{
			using (var process = Start(fileName_, ws_))
			{
				if (1 > iIdleTimeBeforeCloseMsec_)
				{
					//wait indefinitely for process exit
					process.WaitForExit();
				}
				else
				{
					WaitForIdleTime(process, iIdleTimeBeforeCloseMsec_);
					//has been idle for longer than parameter idle time
					if (bCloseOnIdle_)
					{
						//try to gracefully end process
						process.CloseMainWindow();
						if (!process.WaitForExit(1000))
						{
							process.Kill();
							process.WaitForExit(); //wait indefinitely otherwise old processes keep hanging around
						}
					}
				}
			}
		}

		private static void WaitForIdleTime(Process process, int idleTimeBeforeCloseMsec)
		{
			var processTree = GetChildProcesses(process.Id);
			processTree.Add(process);
			long TotalTicks()
			{
				processTree.ForEach((p) => p.Refresh());
				return processTree.Sum((p) => p.TotalProcessorTime.Ticks);
			}
			//polling if process did anything
			const int pollingInterval = 100;
			long lastTickCount = TotalTicks();
			for (int idleTime = 0; idleTime < idleTimeBeforeCloseMsec; idleTime += pollingInterval)
			{
				if (process.WaitForExit(pollingInterval)) return;
				var newTickCount = TotalTicks();
				//check if some processing has been done (any cpu ticks used) during polling interval
				if (newTickCount != lastTickCount)
				{
					//not idle during polling interval -> reset idle time
					idleTime = 0;
					lastTickCount = newTickCount;
				}
			}
		}

		private static int GetParentProcess(int id)
		{
			int parentPid = 0;
			try
			{
				using (ManagementObject mo = new ManagementObject("win32_process.handle='" + id.ToString() + "'"))
				{
					mo.Get();
					parentPid = Convert.ToInt32(mo["ParentProcessId"]);
				}
			}
			catch
			{
				parentPid = 0;
			}
			return parentPid;
		}

		private static Process Start(string fileName_, ProcessWindowStyle ws_)
		{
			ProcessStartInfo psi = new ProcessStartInfo(fileName_)
			{
				RedirectStandardOutput = false,
				UseShellExecute = true
			};
			switch (ws_)
			{
				case ProcessWindowStyle.Maximized:
					psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Maximized;
					break;
				case ProcessWindowStyle.MinimizedNoFocus:
					psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;
					break;
				default:
					psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
					break;
			}
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
