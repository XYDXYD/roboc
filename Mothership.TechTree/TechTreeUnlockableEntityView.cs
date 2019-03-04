using Svelto.ECS;

namespace Mothership.TechTree
{
	internal class TechTreeUnlockableEntityView : EntityView
	{
		public IGameObjectComponent gameObjectComponent;

		public ITechTreeItemIDsComponent iDsComponent;

		public ITechTreeItemCostComponent costComponent;

		public ITechTreeItemStateComponent stateComponent;

		public TechTreeUnlockableEntityView()
			: this()
		{
		}
	}
}
