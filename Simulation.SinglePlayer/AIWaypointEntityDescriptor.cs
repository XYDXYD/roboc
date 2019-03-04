using Svelto.ECS;

namespace Simulation.SinglePlayer
{
	internal class AIWaypointEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static AIWaypointEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[1]
			{
				new EntityViewBuilder<AIWaypointListNode>()
			};
		}

		public AIWaypointEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
