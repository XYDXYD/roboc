using Svelto.ECS;

namespace Mothership.TechTree
{
	internal class TechTreeItemEntityView : EntityView
	{
		public IGameObjectComponent gameObjectComponent;

		public ITechTreeItemPositionComponent positionComponent;

		public ITechTreeItemStateComponent stateComponent;

		public IKeyNavigationComponent navigationComponent;

		public ITechTreeItemSoundsComponent soundsComponent;

		public TechTreeItemEntityView()
			: this()
		{
		}
	}
}
