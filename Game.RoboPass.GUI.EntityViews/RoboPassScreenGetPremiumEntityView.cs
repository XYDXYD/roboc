using Game.ECS.GUI.Components;
using Svelto.ECS;

namespace Game.RoboPass.GUI.EntityViews
{
	internal class RoboPassScreenGetPremiumEntityView : EntityView
	{
		public IButtonComponent buttonComponent;

		public IUIElementVisibleComponent uiElementVisibleComponent;

		public RoboPassScreenGetPremiumEntityView()
			: this()
		{
		}
	}
}
