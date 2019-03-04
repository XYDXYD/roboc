using Simulation.Sight;
using Svelto.ECS;

namespace Simulation.Hardware.Modules.Sight
{
	internal sealed class RadarTagNode : EntityView
	{
		public IRigidBodyComponent rbComponent;

		public ISpottableComponent spottableComponent;

		public IOwnerTeamComponent teamComponent;

		public RadarTagNode()
			: this()
		{
		}
	}
}
