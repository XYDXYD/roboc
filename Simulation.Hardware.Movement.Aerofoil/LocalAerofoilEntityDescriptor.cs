using Simulation.BattleTracker;
using Svelto.ECS;

namespace Simulation.Hardware.Movement.Aerofoil
{
	internal class LocalAerofoilEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static LocalAerofoilEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[6]
			{
				new EntityViewBuilder<LocalAerofoilGraphicsNode>(),
				new EntityViewBuilder<LocalAerofoilComponentNode>(),
				new EntityViewBuilder<AerofoilAudioNode>(),
				new EntityViewBuilder<TopSpeedNode>(),
				new EntityViewBuilder<MovementStatsNode>(),
				new EntityViewBuilder<MovementTrackerNode>()
			};
		}

		public LocalAerofoilEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
