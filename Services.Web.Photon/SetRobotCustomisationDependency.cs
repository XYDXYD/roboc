namespace Services.Web.Photon
{
	public class SetRobotCustomisationDependency
	{
		public string RobotUniqueID
		{
			get;
			private set;
		}

		public uint SlotID
		{
			get;
			private set;
		}

		public string BaySkinID
		{
			get;
			private set;
		}

		public string SpawnEffectID
		{
			get;
			private set;
		}

		public string DeathEffectID
		{
			get;
			private set;
		}

		public SetRobotCustomisationDependency(uint SlotID_, string robotUniqueID_, string baySkinID_, string spawnEffectID_, string deathEffectID_)
		{
			SlotID = SlotID_;
			RobotUniqueID = robotUniqueID_;
			BaySkinID = baySkinID_;
			SpawnEffectID = spawnEffectID_;
			DeathEffectID = deathEffectID_;
		}
	}
}
