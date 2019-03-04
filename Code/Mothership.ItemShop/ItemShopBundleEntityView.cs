using Game.ECS.GUI.Components;
using Svelto.ECS;

namespace Mothership.ItemShop
{
	internal class ItemShopBundleEntityView : EntityView
	{
		public IItemShopBundleComponent bundleComponent;

		public IItemShopBundleGuiComponent guiComponent;

		public IButtonComponent buttonComponent;

		public IShowComponent showComponent;

		public ItemShopBundleEntityView()
			: this()
		{
		}
	}
}
