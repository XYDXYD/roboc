using SinglePlayerCampaign.Simulation.Components;

namespace SinglePlayerCampaign.Simulation.Implementors
{
	internal class IsKillRequirementImplementor : IIsKillRequirementComponent
	{
		public bool isKillRequirement
		{
			get;
			private set;
		}

		public IsKillRequirementImplementor(bool isKillRequirement_)
		{
			isKillRequirement = isKillRequirement_;
		}
	}
}
