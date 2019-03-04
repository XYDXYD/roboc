using Game.ECS.GUI.Implementors;
using Game.RoboPass.GUI.EntityDescriptors;
using Game.RoboPass.GUI.Implementors;
using Simulation;
using Svelto.ECS;
using Svelto.Factories;
using UnityEngine;

namespace Game.RoboPass.GUI
{
	public class RobopassBattleSummaryScreenFactory
	{
		private IEntityFactory _entityFactory;

		private IGameObjectFactory _gameObjectFactory;

		internal RobopassBattleSummaryScreenFactory(IEntityFactory entityFactory, IGameObjectFactory gameObjectFactory)
		{
			_entityFactory = entityFactory;
			_gameObjectFactory = gameObjectFactory;
		}

		internal void BuildBattleSummaryUI(IGUIInputController guiInputController)
		{
			GameObject val = _gameObjectFactory.Build("BattleSummary_RoboPass");
			RoboPassBattleSummaryScreenImplementor component = val.GetComponent<RoboPassBattleSummaryScreenImplementor>();
			RewardedItemsPanelImplementor componentInChildren = val.GetComponentInChildren<RewardedItemsPanelImplementor>(true);
			NextGradeRewardsImplementor componentInChildren2 = val.GetComponentInChildren<NextGradeRewardsImplementor>();
			BattleSummaryRewardItemPurchaseImplementor component2 = val.GetComponent<BattleSummaryRewardItemPurchaseImplementor>();
			int instanceID = val.GetInstanceID();
			component.Initialize(instanceID);
			componentInChildren.Initialize(instanceID);
			component2.Initialize(instanceID);
			GUIDisplayImplementor gUIDisplayImplementor = new GUIDisplayImplementor(GuiScreens.RoboPassBattleSummaryScreen, HudStyle.Full, doesntHideOnSwitch: true, hasBackground: false, isScreenBlurred: true, ShortCutMode.NoKeyboardInputAllowed, TopBarStyle.OffScreen, component.IsShown);
			guiInputController.AddDisplayScreens(new IGUIDisplay[1]
			{
				gUIDisplayImplementor
			});
			object[] array = new object[5]
			{
				gUIDisplayImplementor,
				component,
				componentInChildren,
				componentInChildren2,
				component2
			};
			_entityFactory.BuildEntity<RoboPassBattleSummaryScreenEntityDescriptor>(instanceID, array);
			BuildRewardItems(val);
		}

		private void BuildRewardItems(GameObject screenGO)
		{
			BattleSummaryRewardItemImplementor[] componentsInChildren = screenGO.GetComponentsInChildren<BattleSummaryRewardItemImplementor>(true);
			foreach (BattleSummaryRewardItemImplementor battleSummaryRewardItemImplementor in componentsInChildren)
			{
				int instanceID = battleSummaryRewardItemImplementor.get_gameObject().GetInstanceID();
				_entityFactory.BuildEntity<BattleSummaryRewardItemEntityDescriptor>(instanceID, new object[1]
				{
					battleSummaryRewardItemImplementor
				});
			}
		}
	}
}
