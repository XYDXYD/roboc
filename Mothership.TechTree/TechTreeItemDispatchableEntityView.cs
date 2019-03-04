using Svelto.ECS;

namespace Mothership.TechTree
{
	internal class TechTreeItemDispatchableEntityView : EntityView
	{
		public ITechTreeItemDispatcherComponent dispatcherComponent;

		public TechTreeItemDispatchableEntityView()
			: this()
		{
		}
	}
}
