namespace Simulation.TeamBuff
{
	public class PlayerCubesBuffedDependency
	{
		public int machineId
		{
			get;
			private set;
		}

		public int currentHealth
		{
			get;
			private set;
		}

		public int initialTotalHealth
		{
			get;
			private set;
		}

		public float buffAmount
		{
			get;
			private set;
		}

		public PlayerCubesBuffedDependency(int machineId_, int currentHealth_, int initialHealth_, float buffAmount_)
		{
			machineId = machineId_;
			currentHealth = currentHealth_;
			initialTotalHealth = initialHealth_;
			buffAmount = buffAmount_;
		}
	}
}
