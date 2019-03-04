using Simulation.Sight;
using Svelto.ECS;

namespace Simulation
{
	internal class TeamBaseClientEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static TeamBaseClientEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[1]
			{
				new EntityViewBuilder<SpotterStructureEntityView>()
			};
		}

		public TeamBaseClientEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
