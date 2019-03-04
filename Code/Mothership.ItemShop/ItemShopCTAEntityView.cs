using Game.ECS.GUI.Components;
using Svelto.ECS;

namespace Mothership.ItemShop
{
	internal class ItemShopCTAEntityView : EntityView
	{
		public IShowComponent showComponent;

		public IItemShopCTAReasonComponent reasonComponent;

		public ItemShopCTAEntityView()
			: this()
		{
		}
	}
}
