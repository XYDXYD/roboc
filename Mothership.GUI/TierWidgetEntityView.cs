using Svelto.ECS;

namespace Mothership.GUI
{
	internal sealed class TierWidgetEntityView : EntityView
	{
		public ITierComponent TierComponent;

		public TierWidgetEntityView()
			: this()
		{
		}
	}
}
