using Svelto.ECS;

namespace Simulation.Hardware.Cosmetic.Eye
{
	internal class EyeVigilantEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static EyeVigilantEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[1]
			{
				new EntityViewBuilder<EyeVigiliantNode>()
			};
		}

		public EyeVigilantEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
