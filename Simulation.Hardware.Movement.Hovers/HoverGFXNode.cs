using Svelto.ECS;

namespace Simulation.Hardware.Movement.Hovers
{
	internal class HoverGFXNode : EntityView
	{
		public ITransformComponent transform;

		public IHoverGFXComponent gfx;

		public IHoverInfoComponent info;

		public IHardwareDisabledComponent disabledComponent;

		public IVisibilityComponent visibilityComponent;

		public HoverGFXNode()
			: this()
		{
		}
	}
}
