using Svelto.ECS;

namespace Simulation.Hardware.Cosmetic.Eye
{
	internal class EyeCatEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static EyeCatEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[1]
			{
				new EntityViewBuilder<EyeCatNode>()
			};
		}

		public EyeCatEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
