using Svelto.ECS;

namespace Mothership.TechTree
{
	internal class TechTreeViewEntityView : EntityView
	{
		public IGameObjectComponent gameObjectComponent;

		public ITechTreeViewComponent techTreeViewComponent;

		public IBoundsComponent boundsComponent;

		public TechTreeViewEntityView()
			: this()
		{
		}
	}
}
