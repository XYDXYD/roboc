namespace Simulation.DeathEffects
{
	internal struct Kill
	{
		public readonly int victimId;

		public readonly int shooterId;

		public Kill(int victimId_, int shooterId_)
		{
			victimId = victimId_;
			shooterId = shooterId_;
		}
	}
}
