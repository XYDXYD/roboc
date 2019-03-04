using Game.ECS.GUI.Implementors;
using Simulation;
using Svelto.Factories;
using Svelto.IoC;
using UnityEngine;

namespace Mothership
{
	internal static class BuyPremiumAfterBattleScreenFactory
	{
		private const int MAXIMUM_NUM_PREMIUM_SLOTS = 4;

		public static void Build(IGameObjectFactory goFactory, IGUIInputController guiInputController, BuyPremiumAfterBattlePresenter presenter, BuyPremiumAfterBattleDataSource dataSource, IContainer container)
		{
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			GameObject val = goFactory.Build("GUI_Buy_PremiumCC_Popup");
			BuyPremiumAfterBattleScreen component = val.GetComponent<BuyPremiumAfterBattleScreen>();
			component.Initialize(val.GetInstanceID());
			GUIDisplayImplementor gUIDisplayImplementor = new GUIDisplayImplementor(GuiScreens.BuyPremiumAfterBattle, HudStyle.HideAll, doesntHideOnSwitch: false, hasBackground: false, isScreenBlurred: false, ShortCutMode.NoKeyboardInputAllowed, TopBarStyle.OffScreen, component.isShown);
			guiInputController.AddDisplayScreens(new IGUIDisplay[1]
			{
				gUIDisplayImplementor
			});
			presenter.SetView(component);
			presenter.SetDataSource(dataSource);
			presenter.AddBackButtonListener(component);
			for (int i = 0; i < 4; i++)
			{
				RealMoneyStoreItemCardView cardViewButton = InitialiseSlotItem(i, goFactory, component.PremiumStoreItemTemplate, component.ContainerPremiumItems, dataSource);
				presenter.AddCardViewButtonListener(cardViewButton);
			}
			GameObject val2 = goFactory.Build("GUI_ConfirmPurchase_SingleItem");
			val2.get_transform().set_parent(val.get_transform());
			val2.get_transform().set_localScale(new Vector3(1f, 1f, 1f));
			RealMoneyStoreItemInfoView component2 = val2.GetComponent<RealMoneyStoreItemInfoView>();
			component2.Initialise(val2.GetInstanceID());
			presenter.SetItemInfoView(component2);
			presenter.AddInfoViewButtonListeners(component2);
			component2.get_gameObject().SetActive(false);
			component.get_gameObject().SetActive(false);
		}

		private static RealMoneyStoreItemCardView InitialiseSlotItem(int slotIndex, IGameObjectFactory gofactory, GameObject template, UIGrid container, BuyPremiumAfterBattleDataSource dataSource)
		{
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			GameObject val = gofactory.Build(template);
			val.get_transform().set_parent(container.get_transform());
			container.Reposition();
			val.get_transform().set_localScale(new Vector3(1f, 1f, 1f));
			val.set_name("BuyPremiumSlot_" + slotIndex);
			RealMoneyStoreItemCardView component = val.GetComponent<RealMoneyStoreItemCardView>();
			component.Initialise(slotIndex, RealMoneyStoreSlotDisplayType.PremiumRow, dataSource);
			component.get_gameObject().SetActive(false);
			return component;
		}
	}
}
