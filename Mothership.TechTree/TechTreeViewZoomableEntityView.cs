using Svelto.ECS;

namespace Mothership.TechTree
{
	internal class TechTreeViewZoomableEntityView : EntityView
	{
		public ITechTreeZoomableComponent zoomableComponent;

		public ITechTreeViewDispatcherComponent dispatcherComponent;

		public ITechTreeViewScrollableComponent scrollableComponent;

		public IBoundsComponent boundsComponent;

		public TechTreeViewZoomableEntityView()
			: this()
		{
		}
	}
}
