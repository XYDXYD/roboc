using System;
using System.Diagnostics;

namespace Utility
{
	internal sealed class ExternalProcessesUtility
	{
		public static bool CheckIfApplicationRunning(string applicationName)
		{
			try
			{
				Process[] processes = Process.GetProcesses();
				Process[] array = processes;
				foreach (Process process in array)
				{
					if (!process.HasExited && process.ProcessName == applicationName)
					{
						return true;
					}
				}
			}
			catch (Exception ex)
			{
				Console.Log("CheckIfApplicationRunning error: " + ex.Message);
				return true;
			}
			return false;
		}
	}
}
