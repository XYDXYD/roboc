using Svelto.ECS;

namespace Simulation.Hardware.Weapons.Chaingun
{
	internal class ChaingunEntityGroupDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static ChaingunEntityGroupDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[2]
			{
				new EntityViewBuilder<SharedSpinDataNode>(),
				new EntityViewBuilder<LoadChaingunStatsNode>()
			};
		}

		public ChaingunEntityGroupDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
