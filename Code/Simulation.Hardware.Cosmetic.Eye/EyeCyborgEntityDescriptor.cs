using Svelto.ECS;

namespace Simulation.Hardware.Cosmetic.Eye
{
	internal class EyeCyborgEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static EyeCyborgEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[1]
			{
				new EntityViewBuilder<EyeCyborgNode>()
			};
		}

		public EyeCyborgEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
