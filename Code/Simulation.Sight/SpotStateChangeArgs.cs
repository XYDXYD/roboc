namespace Simulation.Sight
{
	internal struct SpotStateChangeArgs
	{
		public readonly int machineId;

		public readonly bool isSpotted;

		public SpotStateChangeArgs(int machineId, bool isSpotted)
		{
			this.machineId = machineId;
			this.isSpotted = isSpotted;
		}
	}
}
