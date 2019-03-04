using Svelto.ECS;

namespace Simulation
{
	internal sealed class EmpTargetingLocatorEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static EmpTargetingLocatorEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[3]
			{
				new EntityViewBuilder<EmpLocatorCountdownManagementNode>(),
				new EntityViewBuilder<EmpLocatorStunNode>(),
				new EntityViewBuilder<EmpLocatorEffectsNode>()
			};
		}

		public EmpTargetingLocatorEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
