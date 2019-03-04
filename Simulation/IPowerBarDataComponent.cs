namespace Simulation
{
	internal interface IPowerBarDataComponent
	{
		float powerValue
		{
			get;
			set;
		}

		float powerPercent
		{
			get;
			set;
		}

		bool progressiveIncrementActive
		{
			get;
			set;
		}

		void PlayNotEnoughPowerAnimation();
	}
}
