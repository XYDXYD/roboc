namespace Simulation
{
	internal interface IAutoHealGuiComponent
	{
		float soundTimer
		{
			get;
			set;
		}

		int previousSecond
		{
			get;
			set;
		}

		bool animationPlayed
		{
			get;
			set;
		}

		bool autoRegenSoundPlayed
		{
			get;
			set;
		}
	}
}
