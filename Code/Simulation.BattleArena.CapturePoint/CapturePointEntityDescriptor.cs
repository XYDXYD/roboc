using Simulation.Sight;
using Svelto.ECS;

namespace Simulation.BattleArena.CapturePoint
{
	internal class CapturePointEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static CapturePointEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[4]
			{
				new EntityViewBuilder<AudioNode>(),
				new EntityViewBuilder<EffectsNode>(),
				new EntityViewBuilder<CaptureStateNode>(),
				new EntityViewBuilder<SpotterStructureEntityView>()
			};
		}

		public CapturePointEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
