namespace Simulation
{
	internal struct ManaDrainingActivationData
	{
		public bool activate;

		public int machineId;

		public float drainRate;

		public void SetValues(bool activate_, int machineId_, float drainRate_)
		{
			activate = activate_;
			machineId = machineId_;
			drainRate = drainRate_;
		}
	}
}
