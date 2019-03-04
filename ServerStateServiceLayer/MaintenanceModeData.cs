namespace ServerStateServiceLayer
{
	internal class MaintenanceModeData
	{
		public bool isInMaintenance
		{
			get;
			private set;
		}

		public string serverMessage
		{
			get;
			private set;
		}

		public MaintenanceModeData(bool isInMaintenanceLcl, string messageLcl)
		{
			isInMaintenance = isInMaintenanceLcl;
			serverMessage = messageLcl;
		}
	}
}
