namespace Simulation.Hardware.Cosmetic
{
	internal interface IMachineWithExhaustsAudioComponent
	{
		string exhaustAudioEvent
		{
			get;
		}

		string countModulationParameter
		{
			get;
		}

		string powerModulationParameter
		{
			get;
		}

		float powerRampUpTime
		{
			get;
		}

		float powerRampDownTime
		{
			get;
		}

		int aliveExhaustsCountLastFrame
		{
			get;
			set;
		}

		int aliveExhaustsCount
		{
			get;
			set;
		}

		float currentPower
		{
			get;
			set;
		}
	}
}
