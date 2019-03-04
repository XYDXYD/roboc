using Svelto.ECS;

namespace Simulation
{
	internal sealed class CameraControlNode : EntityView
	{
		public ICameraControlComponent cameraControlComponent;

		public CameraControlNode()
			: this()
		{
		}
	}
}
