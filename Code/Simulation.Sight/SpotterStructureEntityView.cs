using Svelto.ECS;

namespace Simulation.Sight
{
	internal sealed class SpotterStructureEntityView : EntityView
	{
		public ISpotterComponent spotterComponent;

		public IOwnerTeamComponent ownerTeamComponent;

		public SpotterStructureEntityView()
			: this()
		{
		}
	}
}
