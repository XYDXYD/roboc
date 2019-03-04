using Simulation.BattleTracker;
using Svelto.ECS;

namespace Simulation.Hardware.Movement.Rotors
{
	internal class RotorBladeEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static RotorBladeEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[8]
			{
				new EntityViewBuilder<RemoveEntityNode>(),
				new EntityViewBuilder<RotorBladeNode>(),
				new EntityViewBuilder<RotorBladeGraphicsNode>(),
				new EntityViewBuilder<RotorBladeAudioNode>(),
				new EntityViewBuilder<CameraRelativeTurnDampingNode>(),
				new EntityViewBuilder<TopSpeedNode>(),
				new EntityViewBuilder<MovementStatsNode>(),
				new EntityViewBuilder<MovementTrackerNode>()
			};
		}

		public RotorBladeEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
