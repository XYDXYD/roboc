using Game.ECS.GUI.Components;
using Svelto.ECS;

namespace Mothership.ItemShop
{
	internal class ItemShopPopUpEntityView : EntityView
	{
		public IItemShopPopUpComponent itemShopPopUpComponent;

		public IItemShopBundleComponent bundleComponent;

		public IDialogChoiceComponent dialogChoiceComponent;

		public IShowComponent showComponent;

		public ItemShopPopUpEntityView()
			: this()
		{
		}
	}
}
