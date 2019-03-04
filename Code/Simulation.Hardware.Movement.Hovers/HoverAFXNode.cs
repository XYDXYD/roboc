using Svelto.ECS;

namespace Simulation.Hardware.Movement.Hovers
{
	internal class HoverAFXNode : EntityView
	{
		public IHoverInfoComponent info;

		public IHardwareDisabledComponent disabledComponent;

		public IVisibilityComponent visibilityComponent;

		public IPartLevelComponent levelComponent;

		public HoverAFXNode()
			: this()
		{
		}
	}
}
