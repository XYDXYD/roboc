using Svelto.ECS;

namespace SinglePlayerCampaign.Simulation.Components
{
	internal interface ITimeComponent
	{
		float elapsedTime
		{
			get;
			set;
		}

		DispatchOnSet<bool> timeRunning
		{
			get;
		}
	}
}
