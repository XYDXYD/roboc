using Simulation.BattleTracker;
using Svelto.ECS;

namespace Simulation.Hardware.Movement.Aerofoil
{
	internal class RemoteAerofoilEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static RemoteAerofoilEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[5]
			{
				new EntityViewBuilder<RemoteAerofoilGraphicsNode>(),
				new EntityViewBuilder<AerofoilAudioNode>(),
				new EntityViewBuilder<TopSpeedNode>(),
				new EntityViewBuilder<MovementStatsNode>(),
				new EntityViewBuilder<MovementTrackerNode>()
			};
		}

		public RemoteAerofoilEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
