using Game.ECS.GUI.Components;
using Svelto.ECS;

namespace Mothership.ItemShop
{
	internal class ItemShopDisplayEntityView : EntityView
	{
		public IItemShopDisplayComponent itemShopDisplayComponent;

		public IShowComponent showComponent;

		public ItemShopDisplayEntityView()
			: this()
		{
		}
	}
}
