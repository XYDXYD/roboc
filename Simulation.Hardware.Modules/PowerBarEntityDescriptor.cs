using Svelto.ECS;

namespace Simulation.Hardware.Modules
{
	internal class PowerBarEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static PowerBarEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[2]
			{
				new EntityViewBuilder<RemoveEntityNode>(),
				new EntityViewBuilder<PowerBarNode>()
			};
		}

		public PowerBarEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
