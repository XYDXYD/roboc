using Svelto.ECS;

namespace Simulation
{
	internal sealed class EmpLocatorStunNode : EntityView
	{
		public IEmpStunActivationComponent activationComponent;

		public IEmpLocatorOwnerComponent ownerComponent;

		public IEmpLocatorRangeComponent rangeComponent;

		public IEmpLocatorTransformComponent transformComponent;

		public IEmpStunDurationComponent stunDurationComponent;

		public EmpLocatorStunNode()
			: this()
		{
		}
	}
}
