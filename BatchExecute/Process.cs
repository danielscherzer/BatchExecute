using System;
using System.Collections.Generic;
using System.Diagnostics;
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

		public static long ChildrenTotalProcessorTime(int id)
		{
			long ticks = 0;
			var children = GetChildProcesses(id);
			foreach (Process p in children)
			{
				ticks += p.TotalProcessorTime.Ticks;
			}
			return ticks;
		}

		public static void Run(string fileName_, ProcessWindowStyle ws_)
		{
			try
			{
				Process process = Start(fileName_, ws_);
				process.Close();
			}
			catch (Exception)
			{ }
		}

		public static void RunAndWait(string fileName_, int iIdleTimeBeforeCloseMsec_, bool bCloseOnIdle_
			, ProcessWindowStyle ws_)
		{
			try
			{
				Process process = Start(fileName_, ws_);
				if (1 > iIdleTimeBeforeCloseMsec_)
				{
					//wait indefinitely for process exit
					process.WaitForExit();
				}
				else
				{
					process.Refresh();
					long lastTickCount = CurrentProcessTreeTicks(process);
					//polling if process did anything
					const int pollingInterval = 100;
					int idleTime = 0;
					while (!process.WaitForExit(pollingInterval))
					{
						//process still running after iWaitTime
						process.Refresh();
						//check if no processing has been done (any cpu ticks used) during polling interval
						long tickCount = CurrentProcessTreeTicks(process);
						if (tickCount == lastTickCount)
						{
							//process was idle during polling interval -> add to idle time
							idleTime += pollingInterval;
							if (idleTime > iIdleTimeBeforeCloseMsec_)
							{
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
								//stop waiting for process -> return and let process be on it's own
								break;
							}
						}
						else
						{
							//not idle during polling interval -> no idle time
							idleTime = 0;
						}
						lastTickCount = CurrentProcessTreeTicks(process);
					}
					process.Close();
				}
			}
			catch (Exception)
			{ }
		}

		private static long CurrentProcessTreeTicks(Process process)
		{
			long i = process.TotalProcessorTime.Ticks + ChildrenTotalProcessorTime(process.Id);
			//Console.WriteLine(i);
			return i;
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
