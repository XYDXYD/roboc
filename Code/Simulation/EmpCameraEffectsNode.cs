using Svelto.ECS;

namespace Simulation
{
	internal sealed class EmpCameraEffectsNode : EntityView
	{
		public IEmpCameraEffectsComponent empEffectsComponent;

		public EmpCameraEffectsNode()
			: this()
		{
		}
	}
}
