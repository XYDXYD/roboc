namespace Services.Web.Photon
{
	public class GetRobotBayCustomisationsResponse
	{
		public string SpawnEffectId
		{
			get;
			private set;
		}

		public string DeathEffectId
		{
			get;
			private set;
		}

		public string BaySkinId
		{
			get;
			private set;
		}

		public GetRobotBayCustomisationsResponse(string spawnEffectId_, string deathEffectId_, string baySkinId_)
		{
			SpawnEffectId = spawnEffectId_;
			DeathEffectId = deathEffectId_;
			BaySkinId = baySkinId_;
		}
	}
}
