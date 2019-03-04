using Svelto.ECS;

namespace Simulation.Hardware
{
	internal sealed class MachineInputEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static MachineInputEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[2]
			{
				new EntityViewBuilder<RemoveEntityNode>(),
				new EntityViewBuilder<MachineInputNode>()
			};
		}

		public MachineInputEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
