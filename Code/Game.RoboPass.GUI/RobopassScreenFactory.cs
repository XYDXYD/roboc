using Game.ECS.GUI.Components;
using Game.ECS.GUI.Implementors;
using Game.RoboPass.EntityDescriptors;
using Game.RoboPass.GUI.Components;
using Game.RoboPass.GUI.EntityDescriptors;
using Game.RoboPass.GUI.EntityViews;
using Game.RoboPass.GUI.Implementors;
using Game.RoboPass.Implementors;
using Simulation;
using Svelto.ECS;
using Svelto.Factories;
using System.Collections.Generic;
using UnityEngine;

namespace Game.RoboPass.GUI
{
	internal class RobopassScreenFactory
	{
		private readonly IEntityFactory _entityFactory;

		private readonly IGameObjectFactory _gameObjectFactory;

		internal RobopassScreenFactory(IEntityFactory entityFactory, IGameObjectFactory gameObjectFactory)
		{
			_entityFactory = entityFactory;
			_gameObjectFactory = gameObjectFactory;
		}

		internal void Build(IGUIInputController guiInputController)
		{
			GameObject val = _gameObjectFactory.Build("GUI_RoboPass");
			RoboPassSeasonInfoImplementor component = val.GetComponent<RoboPassSeasonInfoImplementor>();
			RewardsGridsImplementor component2 = val.GetComponent<RewardsGridsImplementor>();
			RootGOImplementor component3 = val.GetComponent<RootGOImplementor>();
			int instanceID = val.GetInstanceID();
			component.Initialize(instanceID);
			component2.Initialize(instanceID);
			GUIDisplayImplementor gUIDisplayImplementor = new GUIDisplayImplementor(GuiScreens.RoboPassScreen, HudStyle.Full, doesntHideOnSwitch: false, hasBackground: true, isScreenBlurred: false, ShortCutMode.OnlyGUINoSwitching, TopBarStyle.FullScreenInterface, component.IsShown);
			guiInputController.AddDisplayScreens(new IGUIDisplay[1]
			{
				gUIDisplayImplementor
			});
			RoboPassSeasonPlayerInfoImplementor roboPassSeasonPlayerInfoImplementor = new RoboPassSeasonPlayerInfoImplementor(instanceID);
			List<object> list = new List<object>();
			list.Add(component2);
			list.Add(component);
			list.Add(component3);
			list.Add(roboPassSeasonPlayerInfoImplementor);
			List<object> list2 = list;
			_entityFactory.BuildEntity<RoboPassSeasonScreenEntityDescriptor>(instanceID, list2.ToArray());
			_entityFactory.BuildEntity<RoboPassSeasonDataEntityDescriptor>(instanceID, new object[2]
			{
				component,
				roboPassSeasonPlayerInfoImplementor
			});
			RoboPassXpGradeUIEntityDescriptorHolder componentInChildren = val.GetComponentInChildren<RoboPassXpGradeUIEntityDescriptorHolder>(true);
			GameObject gameObject = componentInChildren.get_gameObject();
			instanceID = gameObject.GetInstanceID();
			MonoBehaviour[] componentsInChildren = gameObject.GetComponentsInChildren<MonoBehaviour>(true);
			list2 = new List<object>(componentsInChildren);
			_entityFactory.BuildEntity<RoboPassXpGradeUIEntityDescriptor>(instanceID, list2.ToArray());
			RoboPassSeasonTitleUIEntityDescriptorHolder componentInChildren2 = val.GetComponentInChildren<RoboPassSeasonTitleUIEntityDescriptorHolder>(true);
			GameObject gameObject2 = componentInChildren2.get_gameObject();
			instanceID = gameObject2.GetInstanceID();
			componentsInChildren = gameObject2.GetComponentsInChildren<MonoBehaviour>(true);
			list2 = new List<object>(componentsInChildren);
			_entityFactory.BuildEntity<RoboPassSeasonTitleUIEntityDescriptor>(instanceID, list2.ToArray());
			RoboPassSeasonTimeUIEntityDescriptorHolder componentInChildren3 = val.GetComponentInChildren<RoboPassSeasonTimeUIEntityDescriptorHolder>(true);
			GameObject gameObject3 = componentInChildren3.get_gameObject();
			instanceID = gameObject3.GetInstanceID();
			componentsInChildren = gameObject3.GetComponentsInChildren<MonoBehaviour>(true);
			list2 = new List<object>(componentsInChildren);
			_entityFactory.BuildEntity<RoboPassSeasonTimeUIEntityDescriptor>(instanceID, list2.ToArray());
		}

		internal void BuildRewardsUI(IRewardsGridsComponent rewardsGridsComp)
		{
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			GameObject freeRewardTemplateGo = rewardsGridsComp.freeRewardTemplateGo;
			UIGrid freeRewardsUiGrid = rewardsGridsComp.freeRewardsUiGrid;
			GameObject deluxeRewardTemplateGo = rewardsGridsComp.deluxeRewardTemplateGo;
			UIGrid deluxeRewardsUiGrid = rewardsGridsComp.deluxeRewardsUiGrid;
			Transform transform = freeRewardsUiGrid.get_gameObject().get_transform();
			Transform transform2 = deluxeRewardsUiGrid.get_gameObject().get_transform();
			int num = rewardsGridsComp.columnLimit = freeRewardsUiGrid.maxPerLine;
			rewardsGridsComp.maxPageNumber = 1;
			for (int i = 0; i < num; i++)
			{
				GameObject val = _gameObjectFactory.Build(freeRewardTemplateGo);
				RoboPassRewardView component = val.GetComponent<RoboPassRewardView>();
				Transform transform3 = val.get_transform();
				transform3.set_parent(transform);
				transform3.set_localPosition(Vector3.get_zero());
				transform3.set_localRotation(Quaternion.get_identity());
				transform3.set_localScale(Vector3.get_one());
				GameObject val2 = _gameObjectFactory.Build(deluxeRewardTemplateGo);
				RoboPassRewardView component2 = val2.GetComponent<RoboPassRewardView>();
				Transform transform4 = val2.get_transform();
				transform4.set_parent(transform2);
				transform4.set_localPosition(Vector3.get_zero());
				transform4.set_localRotation(Quaternion.get_identity());
				transform4.set_localScale(Vector3.get_one());
				int instanceID = val.GetInstanceID();
				component.Initialize(isDeluxeCell_: false);
				object[] array = new object[1]
				{
					component
				};
				_entityFactory.BuildEntity<RoboPassRewardEntityDescriptor>(instanceID, array);
				instanceID = val2.GetInstanceID();
				component2.Initialize(isDeluxeCell_: true);
				array = new object[1]
				{
					component2
				};
				_entityFactory.BuildEntity<RoboPassRewardEntityDescriptor>(instanceID, array);
				val.SetActive(true);
				val2.SetActive(true);
			}
			freeRewardsUiGrid.Reposition();
			deluxeRewardsUiGrid.Reposition();
		}

		internal void BuildButtonsUI(RoboPassSeasonScreenEntityView roboPassSeasonScreenEV)
		{
			IRootGOComponent rootGOComponent = roboPassSeasonScreenEV.rootGOComponent;
			GameObject rootGO = rootGOComponent.rootGO;
			RoboPassScreenGetPremiumEntityDescriptorHolder componentInChildren = rootGO.GetComponentInChildren<RoboPassScreenGetPremiumEntityDescriptorHolder>(true);
			GameObject gameObject = componentInChildren.get_gameObject();
			int instanceID = gameObject.GetInstanceID();
			RoboPassGoToStoreButtonImplementor componentInChildren2 = gameObject.GetComponentInChildren<RoboPassGoToStoreButtonImplementor>(true);
			componentInChildren2.Initialize(instanceID);
			UiElementVisibleImplementor component = gameObject.GetComponent<UiElementVisibleImplementor>();
			object[] array = new object[2]
			{
				componentInChildren2,
				component
			};
			_entityFactory.BuildEntity<RoboPassScreenGetPremiumEntityDescriptor>(instanceID, array);
			RoboPassGetRoboPassPlusButtonEntityDescriptorHolder componentInChildren3 = rootGO.GetComponentInChildren<RoboPassGetRoboPassPlusButtonEntityDescriptorHolder>(true);
			GameObject gameObject2 = componentInChildren3.get_gameObject();
			instanceID = gameObject2.GetInstanceID();
			componentInChildren2 = gameObject2.GetComponent<RoboPassGoToStoreButtonImplementor>();
			componentInChildren2.Initialize(instanceID);
			component = gameObject2.GetComponent<UiElementVisibleImplementor>();
			array = new object[2]
			{
				componentInChildren2,
				component
			};
			_entityFactory.BuildEntity<RoboPassGetRoboPassPlusButtonEntityDescriptor>(instanceID, array);
		}
	}
}
