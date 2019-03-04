using Svelto.ECS;

namespace Simulation.SinglePlayer
{
	internal class AIWaypointListNode : EntityView
	{
		public IAIWaypointListComponent aiWaypointListComponent;

		public AIWaypointListNode()
			: this()
		{
		}
	}
}
