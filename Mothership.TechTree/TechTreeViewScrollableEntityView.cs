using Svelto.ECS;

namespace Mothership.TechTree
{
	internal class TechTreeViewScrollableEntityView : EntityView
	{
		public ITechTreeZoomableComponent zoomableComponent;

		public ITechTreeViewScrollableComponent scrollableComponent;

		public ITechTreeViewDispatcherComponent dispatcherComponent;

		public IBoundsComponent boundsComponent;

		public TechTreeViewScrollableEntityView()
			: this()
		{
		}
	}
}
