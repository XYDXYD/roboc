namespace Simulation
{
	internal struct HealingAppliedData
	{
		public int healerId;

		public int targetId;

		public int weaponKey;

		public HealingAppliedData(int healerId, int targetId, int weaponKey)
		{
			this.healerId = healerId;
			this.targetId = targetId;
			this.weaponKey = weaponKey;
		}
	}
}
