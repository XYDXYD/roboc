using Svelto.ECS;

namespace Simulation.Hardware.Weapons
{
	internal sealed class CameraShakeDamageEntityView : EntityView
	{
		public ICameraShakeDamageComponent cameraShakeDamageComponent;

		public CameraShakeDamageEntityView()
			: this()
		{
		}
	}
}
