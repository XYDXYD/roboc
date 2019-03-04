using Simulation.Hardware;
using Svelto.ECS;

namespace Simulation.Sight
{
	internal sealed class SpotterMachineEntityView : EntityView
	{
		public ISpotterComponent spotterComponent;

		public IFrustumComponent frustrumComponent;

		public IMachineOwnerComponent ownerComponent;

		public IOwnerTeamComponent teamComponent;

		public SpotterMachineEntityView()
			: this()
		{
		}
	}
}
