using Svelto.ECS;

namespace Simulation.Hardware.Modules
{
	internal class ModuleEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static ModuleEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[2]
			{
				new EntityViewBuilder<RemoveEntityNode>(),
				new EntityViewBuilder<HardwareHealthStatusNode>()
			};
		}

		public ModuleEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
