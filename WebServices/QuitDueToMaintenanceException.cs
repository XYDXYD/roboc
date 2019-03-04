using System;

namespace WebServices
{
	public class QuitDueToMaintenanceException : Exception
	{
		public static string CODE_ERROR = CODEERROR.MAINTENANCE.ToString("d");

		public QuitDueToMaintenanceException(string message)
			: base("Error code " + CODE_ERROR + ": Maintenance Status -" + message)
		{
		}
	}
}
