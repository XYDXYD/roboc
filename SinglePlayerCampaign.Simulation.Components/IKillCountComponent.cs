using Svelto.ECS;

namespace SinglePlayerCampaign.Simulation.Components
{
	internal interface IKillCountComponent
	{
		DispatchOnSet<int> killCount
		{
			get;
			set;
		}
	}
}
