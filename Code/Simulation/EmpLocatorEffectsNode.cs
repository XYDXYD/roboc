using Svelto.ECS;

namespace Simulation
{
	internal sealed class EmpLocatorEffectsNode : EntityView
	{
		public IEmpLocatorEffectsComponent effectsComponent;

		public IEmpLocatorTransformComponent transformComponent;

		public IEmpLocatorOwnerComponent ownerComponent;

		public IEmpStunDurationComponent stunDurationComponent;

		public IEmpStunActivationComponent activationComponent;

		public EmpLocatorEffectsNode()
			: this()
		{
		}
	}
}
