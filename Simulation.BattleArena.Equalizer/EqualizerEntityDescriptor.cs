using Simulation.Hardware;
using Simulation.Sight;
using Svelto.ECS;

namespace Simulation.BattleArena.Equalizer
{
	internal class EqualizerEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static EqualizerEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[4]
			{
				new EntityViewBuilder<RemoveEntityNode>(),
				new EntityViewBuilder<EqualizerActivationNode>(),
				new EntityViewBuilder<EqualizerEffectsNode>(),
				new EntityViewBuilder<SpotterStructureEntityView>()
			};
		}

		public EqualizerEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
