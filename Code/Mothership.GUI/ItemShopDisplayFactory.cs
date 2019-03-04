using Game.ECS.GUI.Implementors;
using Mothership.ItemShop;
using Simulation;
using Svelto.ECS;
using Svelto.Factories;
using UnityEngine;

namespace Mothership.GUI
{
	internal static class ItemShopDisplayFactory
	{
		public static void Build(IEntityFactory entityFactory, IGameObjectFactory goFactory, IGUIInputController guiInputController)
		{
			GameObject val = goFactory.Build("GUI_ItemShop");
			int instanceID = val.GetInstanceID();
			ItemShopDisplayImplementor component = val.GetComponent<ItemShopDisplayImplementor>();
			component.Initialize(instanceID);
			GUIDisplayImplementor gUIDisplayImplementor = new GUIDisplayImplementor(GuiScreens.ItemShopScreen, HudStyle.Full, doesntHideOnSwitch: false, hasBackground: true, isScreenBlurred: false, ShortCutMode.OnlyGUINoSwitching, TopBarStyle.FullScreenInterface, component.isShown);
			val.SetActive(false);
			entityFactory.BuildEntity<ItemShopDisplayEntityDescriptor>(instanceID, new object[1]
			{
				component
			});
			guiInputController.AddDisplayScreens(new IGUIDisplay[1]
			{
				gUIDisplayImplementor
			});
			BuildPopup(entityFactory, component.popup);
		}

		private static void BuildPopup(IEntityFactory entityFactory, GameObject go)
		{
			int instanceID = go.GetInstanceID();
			ItemShopPopUpImplementor component = go.GetComponent<ItemShopPopUpImplementor>();
			component.Initialize(instanceID);
			entityFactory.BuildEntity<ItemShopPopUpEntityDescriptor>(instanceID, new object[1]
			{
				component
			});
		}
	}
}
