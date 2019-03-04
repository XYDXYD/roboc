using Game.ECS.GUI.Implementors;
using Simulation;
using Svelto.Factories;
using Svelto.IoC;
using System.Collections;
using UnityEngine;

namespace Mothership
{
	internal static class RealMoneyStoreScreenFactory
	{
		private const int MAXIMUM_NUM_CC_SLOTS = 4;

		private const int MAXIMUM_NUM_PREMIUM_SLOTS = 4;

		private const int MAXIMUM_NUM_POSSIBLE_ROBOPASS_ITEMS = 4;

		public static IEnumerator Build(IGameObjectFactory goFactory, IGUIInputController guiInputController, RealMoneyStorePresenter presenter, IRealMoneyStoreItemDataSource dataSource, IRealMoneyStorePossibleRoboPassItemsDataSource possibleItemsDataSource, IContainer container)
		{
			GameObject screenGo = goFactory.Build("GUI_Store");
			RealMoneyStoreScreen screen = screenGo.GetComponent<RealMoneyStoreScreen>();
			screen.Initialize(screenGo.GetInstanceID());
			GUIDisplayImplementor guiDisplay = new GUIDisplayImplementor(GuiScreens.RealMoneyStoreScreen, HudStyle.HideAll, doesntHideOnSwitch: false, hasBackground: false, isScreenBlurred: false, ShortCutMode.OnlyGUINoSwitching, TopBarStyle.FullScreenInterface, screen.isShown);
			guiInputController.AddDisplayScreens(new IGUIDisplay[1]
			{
				guiDisplay
			});
			presenter.SetView(screen);
			presenter.SetDataSources(dataSource, possibleItemsDataSource);
			RealMoneyStoreItemCardView cardView;
			for (int i = 0; i < 4; i++)
			{
				cardView = InitialiseSlotItem(i, goFactory, screen.PremiumStoreItemTemplate, screen.ContainerPremiumItems, RealMoneyStoreSlotDisplayType.PremiumRow, dataSource);
				presenter.AddCardViewButtonListener(cardView, RealMoneyStoreSlotDisplayType.PremiumRow);
			}
			for (int j = 0; j < 4; j++)
			{
				cardView = InitialiseSlotItem(j, goFactory, screen.CosmeticCreditStoreItemTemplate, screen.ContainerCosmeticCreditItems, RealMoneyStoreSlotDisplayType.CosmeticCreditsRow, dataSource);
				presenter.AddCardViewButtonListener(cardView, RealMoneyStoreSlotDisplayType.CosmeticCreditsRow);
			}
			cardView = InitialiseSlotItem(0, goFactory, screen.RoboPassTemplate, screen.ContainerRoboPass, RealMoneyStoreSlotDisplayType.Robopass, dataSource);
			presenter.AddCardViewButtonListener(cardView, RealMoneyStoreSlotDisplayType.Robopass);
			GameObject iteminfoviewGO = goFactory.Build("GUI_ConfirmPurchase_SingleItem");
			iteminfoviewGO.get_transform().set_parent(screenGo.get_transform());
			iteminfoviewGO.get_transform().set_localScale(new Vector3(1f, 1f, 1f));
			RealMoneyStoreItemInfoView iteminfoview = iteminfoviewGO.GetComponent<RealMoneyStoreItemInfoView>();
			iteminfoview.Initialise(iteminfoviewGO.GetInstanceID());
			presenter.SetItemInfoView(iteminfoview);
			iteminfoview.get_gameObject().SetActive(false);
			presenter.AddInfoViewButtonListeners(iteminfoview);
			for (int k = 0; k < 4; k++)
			{
				InitialisePossibleRoboPassItem(k, goFactory, iteminfoview.RoboPassPossibleItemTemplateGO, iteminfoview.RoboPassPossibleItemsContainer, possibleItemsDataSource);
			}
			screen.get_gameObject().SetActive(false);
			screen.ContainerRoboPass.set_repositionNow(true);
			yield return null;
		}

		private static void InitialisePossibleRoboPassItem(int slotIndex, IGameObjectFactory gofactory, GameObject template, UIGrid container, IRealMoneyStorePossibleRoboPassItemsDataSource dataSource)
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			GameObject val = gofactory.Build(template);
			val.get_transform().set_parent(container.get_transform());
			val.get_transform().set_localScale(new Vector3(1f, 1f, 1f));
			val.set_name("PossibleAwardedItem_" + slotIndex);
			RealMoneyStoreRoboPassPossibleItemView component = val.GetComponent<RealMoneyStoreRoboPassPossibleItemView>();
			component.Initialise(slotIndex, dataSource);
			component.get_gameObject().SetActive(false);
		}

		private static RealMoneyStoreItemCardView InitialiseSlotItem(int slotIndex, IGameObjectFactory gofactory, GameObject template, UIGrid container, RealMoneyStoreSlotDisplayType viewType, IRealMoneyStoreItemDataSource dataSource)
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			GameObject val = gofactory.Build(template);
			val.get_transform().set_parent(container.get_transform());
			val.get_transform().set_localScale(new Vector3(1f, 1f, 1f));
			val.set_name("RealMoneySlotItem_" + viewType + slotIndex);
			RealMoneyStoreItemCardView component = val.GetComponent<RealMoneyStoreItemCardView>();
			component.Initialise(slotIndex, viewType, dataSource);
			component.get_gameObject().SetActive(false);
			return component;
		}
	}
}
