using Svelto.ECS;

namespace Simulation.Hardware.Modules
{
	internal sealed class ModuleEntityGroupDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static ModuleEntityGroupDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[1]
			{
				new EntityViewBuilder<ModuleGroupLastShotTimeNode>()
			};
		}

		public ModuleEntityGroupDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
