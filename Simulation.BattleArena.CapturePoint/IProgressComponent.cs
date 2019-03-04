namespace Simulation.BattleArena.CapturePoint
{
	internal interface IProgressComponent
	{
		float progressPercent
		{
			get;
			set;
		}

		float maxProgress
		{
			get;
			set;
		}
	}
}
