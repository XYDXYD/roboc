namespace SinglePlayerServiceLayer.Requests.Photon
{
	internal struct CommunityRobotData
	{
		public string robotGuid;

		public byte[] machineModel;

		public byte[] colorModel;

		public string robotName;

		public CommunityRobotData(string robotGuid, byte[] machineModel, byte[] colorModel, string robotName)
		{
			this.robotGuid = robotGuid;
			this.machineModel = machineModel;
			this.robotName = robotName;
			this.colorModel = colorModel;
		}
	}
}
