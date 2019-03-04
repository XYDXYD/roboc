namespace Simulation.Hardware.Cosmetic
{
	internal class MachineWithExhaustsComponent : IMachineWithExhaustsAudioComponent
	{
		public string exhaustAudioEvent
		{
			get;
			private set;
		}

		public string countModulationParameter
		{
			get;
			private set;
		}

		public string powerModulationParameter
		{
			get;
			private set;
		}

		public float powerRampUpTime
		{
			get;
			private set;
		}

		public float powerRampDownTime
		{
			get;
			private set;
		}

		public int aliveExhaustsCountLastFrame
		{
			get;
			set;
		}

		public int aliveExhaustsCount
		{
			get;
			set;
		}

		public float currentPower
		{
			get;
			set;
		}

		public MachineWithExhaustsComponent(bool isLocalPlayer)
		{
			exhaustAudioEvent = ((!isLocalPlayer) ? "Exhaust_Timeline_Enemy" : "Exhaust_Timeline");
			countModulationParameter = "COUNTS";
			powerModulationParameter = "Power";
			powerRampUpTime = 1f;
			powerRampDownTime = 1f;
		}
	}
}
