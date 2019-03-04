using Svelto.ECS;

namespace Simulation
{
	internal sealed class EmpLocatorCountdownManagementNode : EntityView
	{
		public IEmpStunActivationComponent activationComponent;

		public IEmpLocatorCoutdownComponent countdownComponent;

		public IEmpLocatorObjectComponent objectComponent;

		public EmpLocatorCountdownManagementNode()
			: this()
		{
		}
	}
}
