using Svelto.ECS;

namespace Simulation.Hardware.Cosmetic.Eye
{
	internal class EyeNode : EntityView
	{
		public IEyeComponent eyeComponent;

		public IHardwareDisabledComponent hardwareDisabledComponent;

		public IVisibilityComponent visibilityComponent;

		public IHardwareOwnerComponent hardwareOwnerComponent;

		public ITransformComponent transformComponent;

		public EyeNode()
			: this()
		{
		}
	}
}
