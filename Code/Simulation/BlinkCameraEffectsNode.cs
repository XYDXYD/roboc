using Svelto.ECS;

namespace Simulation
{
	internal sealed class BlinkCameraEffectsNode : EntityView
	{
		public IBlinkCameraEffectsComponent blinkCameraEffectsComponent;

		public BlinkCameraEffectsNode()
			: this()
		{
		}
	}
}
