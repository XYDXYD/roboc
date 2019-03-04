namespace Simulation
{
	internal class AutoHealGuiImplementor : IAutoHealGuiComponent
	{
		public float soundTimer
		{
			get;
			set;
		}

		public int previousSecond
		{
			get;
			set;
		}

		public bool animationPlayed
		{
			get;
			set;
		}

		public bool autoRegenSoundPlayed
		{
			get;
			set;
		}
	}
}
