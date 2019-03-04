using Game.ECS.GUI.Implementors;
using Simulation;
using Svelto.Factories;
using UnityEngine;

namespace Mothership
{
	internal class BuyRoboPassAfterBattleScreenFactory
	{
		private const int MAXIMUM_NUM_ITEM_SLOTS = 4;

		private readonly IGameObjectFactory _goFactory;

		private readonly IGUIInputController _guiInputController;

		public BuyRoboPassAfterBattleScreenFactory(IGameObjectFactory goFactory, IGUIInputController guiInputController)
		{
			_goFactory = goFactory;
			_guiInputController = guiInputController;
		}

		public void Build(BuyRoboPassAfterBattlePresenter presenter, IRealMoneyStoreItemDataSource itemDataSource, IRealMoneyStorePossibleRoboPassItemsDataSource roboPassItemsDataSource)
		{
			GameObject val = _goFactory.Build("GUI_ConfirmPurchase_RoboPass");
			RealMoneyStoreItemInfoView component = val.GetComponent<RealMoneyStoreItemInfoView>();
			component.Initialise(val.GetInstanceID());
			component.get_gameObject().SetActive(false);
			GUIDisplayImplementor gUIDisplayImplementor = new GUIDisplayImplementor(GuiScreens.BuyRoboPassAfterBattle, HudStyle.HideAll, doesntHideOnSwitch: false, hasBackground: false, isScreenBlurred: false, ShortCutMode.NoKeyboardInputAllowed, TopBarStyle.OffScreen, component.isShown);
			_guiInputController.AddDisplayScreens(new IGUIDisplay[1]
			{
				gUIDisplayImplementor
			});
			presenter.SetView(component);
			presenter.SetDataSource(itemDataSource);
			presenter.AddButtonListener(component);
			for (int i = 0; i < 4; i++)
			{
				RealMoneyStoreRoboPassPossibleItemView realMoneyStoreRoboPassPossibleItemView = CreateSlotItem(i, component.RoboPassPossibleItemTemplateGO, component.RoboPassPossibleItemsContainer);
				realMoneyStoreRoboPassPossibleItemView.Initialise(i, roboPassItemsDataSource);
				realMoneyStoreRoboPassPossibleItemView.get_gameObject().SetActive(false);
			}
		}

		private RealMoneyStoreRoboPassPossibleItemView CreateSlotItem(int slotIndex, GameObject template, UIGrid container)
		{
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			GameObject val = _goFactory.Build(template);
			val.get_transform().set_parent(container.get_transform());
			container.Reposition();
			val.get_transform().set_localScale(new Vector3(1f, 1f, 1f));
			val.set_name("BuyRoboPassSlot_" + slotIndex);
			return val.GetComponent<RealMoneyStoreRoboPassPossibleItemView>();
		}
	}
}
