using Svelto.ECS;

namespace Simulation.Hardware
{
	internal class SkinnedMeshEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static SkinnedMeshEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[1]
			{
				new EntityViewBuilder<SkinnedMeshNode>()
			};
		}

		public SkinnedMeshEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
