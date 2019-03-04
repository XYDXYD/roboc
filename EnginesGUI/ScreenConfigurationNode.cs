using Svelto.ECS;

namespace EnginesGUI
{
	internal sealed class ScreenConfigurationNode : EntityView
	{
		public readonly IScreenConfigComponent screenConfigComponent;

		public readonly IPanelSizeComponent panelSizeComponent;

		public ScreenConfigurationNode()
			: this()
		{
		}
	}
}
