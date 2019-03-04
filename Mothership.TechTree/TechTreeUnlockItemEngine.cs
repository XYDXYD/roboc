using Mothership.ItemShop;
using Services.Analytics;
using Services.TechTree;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Mothership.TechTree
{
	internal class TechTreeUnlockItemEngine : MultiEntityViewsEngine<TechTreeItemDispatchableEntityView, TechTreeDialogueEntityView>, IQueryingEntityViewEngine, IEngine
	{
		private LoadingIconPresenter _loadingIconPresenter;

		private TechTreeUnlockableEntityView _nodeToUnlock;

		private TechTreeDialogueEntityView _notEnoughTPDialogue;

		private TechTreeDialogueEntityView _confirmUnlockDialogue;

		private TechTreeDialogueEntityView _celebrateUnlockDialogue;

		[Inject]
		internal ICubeInventory cubeInventory
		{
			private get;
			set;
		}

		[Inject]
		internal ICubeList cubeList
		{
			private get;
			set;
		}

		[Inject]
		internal IServiceRequestFactory serviceFactory
		{
			private get;
			set;
		}

		[Inject]
		internal TechPointsTracker techPointsTracker
		{
			private get;
			set;
		}

		[Inject]
		internal IAnalyticsRequestFactory analyticsRequestFactory
		{
			private get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			get;
			set;
		}

		public TechTreeUnlockItemEngine(LoadingIconPresenter loadingIconPresenter)
		{
			_loadingIconPresenter = loadingIconPresenter;
		}

		public void Ready()
		{
		}

		protected override void Add(TechTreeItemDispatchableEntityView techTreeItemDispatchableEntityView)
		{
			techTreeItemDispatchableEntityView.dispatcherComponent.IsClicked.NotifyOnValueSet((Action<int, bool>)DisplayUnlockDialogueIfUnlockable);
		}

		protected override void Remove(TechTreeItemDispatchableEntityView techTreeItemDispatchableEntityView)
		{
			techTreeItemDispatchableEntityView.dispatcherComponent.IsClicked.StopNotify((Action<int, bool>)DisplayUnlockDialogueIfUnlockable);
		}

		protected override void Add(TechTreeDialogueEntityView techTreeDialogueEntityView)
		{
			techTreeDialogueEntityView.dialogueButtonsComponent.ConfirmButton.NotifyOnValueSet((Action<int, bool>)OnConfirmButtonPressed);
			techTreeDialogueEntityView.dialogueButtonsComponent.CancelButton.NotifyOnValueSet((Action<int, bool>)OnCancelButtonPressed);
			techTreeDialogueEntityView.dialogueButtonsComponent.Dismissed.NotifyOnValueSet((Action<int, bool>)OnDialogDismissed);
			switch (techTreeDialogueEntityView.dialogueTypeComponent.Type)
			{
			case TechTreeDialogueType.NotEnoughTP:
				_notEnoughTPDialogue = techTreeDialogueEntityView;
				break;
			case TechTreeDialogueType.ConfirmUnlock:
				_confirmUnlockDialogue = techTreeDialogueEntityView;
				break;
			case TechTreeDialogueType.CelebrateUnlock:
				_celebrateUnlockDialogue = techTreeDialogueEntityView;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		protected override void Remove(TechTreeDialogueEntityView techTreeDialogueEntityView)
		{
			techTreeDialogueEntityView.dialogueButtonsComponent.ConfirmButton.StopNotify((Action<int, bool>)OnConfirmButtonPressed);
			techTreeDialogueEntityView.dialogueButtonsComponent.CancelButton.StopNotify((Action<int, bool>)OnCancelButtonPressed);
			techTreeDialogueEntityView.dialogueButtonsComponent.Dismissed.StopNotify((Action<int, bool>)OnDialogDismissed);
			switch (techTreeDialogueEntityView.dialogueTypeComponent.Type)
			{
			case TechTreeDialogueType.NotEnoughTP:
				_notEnoughTPDialogue = null;
				break;
			case TechTreeDialogueType.ConfirmUnlock:
				_confirmUnlockDialogue = null;
				break;
			case TechTreeDialogueType.CelebrateUnlock:
				_celebrateUnlockDialogue = null;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		private void OnDialogDismissed(int instanceId, bool dismissed)
		{
			if (dismissed)
			{
				TaskRunner.get_Instance().Run(HoverSlotInNextFrame(_nodeToUnlock.gameObjectComponent.gameObject));
			}
		}

		private void OnCancelButtonPressed(int instanceId, bool value)
		{
			TechTreeDialogueEntityView techTreeDialogueEntityView = entityViewsDB.QueryEntityView<TechTreeDialogueEntityView>(instanceId);
			techTreeDialogueEntityView.gameObjectComponent.gameObject.SetActive(false);
			TaskRunner.get_Instance().Run(HoverSlotInNextFrame(_nodeToUnlock.gameObjectComponent.gameObject));
		}

		private void OnConfirmButtonPressed(int instanceId, bool value)
		{
			TechTreeDialogueEntityView techTreeDialogueEntityView = entityViewsDB.QueryEntityView<TechTreeDialogueEntityView>(instanceId);
			techTreeDialogueEntityView.gameObjectComponent.gameObject.SetActive(false);
			TaskRunner.get_Instance().Run(UnlockItems());
		}

		private void DisplayUnlockDialogueIfUnlockable(int instanceId, bool value)
		{
			TechTreeUnlockableEntityView techTreeUnlockableEntityView = entityViewsDB.QueryEntityView<TechTreeUnlockableEntityView>(instanceId);
			ITechTreeItemStateComponent stateComponent = techTreeUnlockableEntityView.stateComponent;
			if (!stateComponent.IsUnLocked.get_value() && stateComponent.IsUnlockable.get_value())
			{
				_nodeToUnlock = techTreeUnlockableEntityView;
				techPointsTracker.RefreshUserTechPointsAmount(DisplayCorrectDialogue);
			}
		}

		private IEnumerator UnlockItems()
		{
			SerialTaskCollection serialTaskCollection = new SerialTaskCollection(2, (string)null);
			IUnlockTechTreeNodeRequest unlockTechTreeNodeReq = serviceFactory.Create<IUnlockTechTreeNodeRequest>();
			unlockTechTreeNodeReq.Inject(_nodeToUnlock.iDsComponent.NodeID);
			TaskService<Dictionary<string, TechTreeItemData>> unlockTechTreeNodeTS = new TaskService<Dictionary<string, TechTreeItemData>>(unlockTechTreeNodeReq);
			serialTaskCollection.Add(unlockTechTreeNodeTS);
			yield return serialTaskCollection;
			if (!unlockTechTreeNodeTS.succeeded)
			{
				Console.LogError("Failed to unlock node");
				yield break;
			}
			Dictionary<string, TechTreeItemData> nodesToUpdate = unlockTechTreeNodeTS.result;
			foreach (KeyValuePair<string, TechTreeItemData> item in nodesToUpdate)
			{
				TechTreeItemData value = item.Value;
				TechTreeItemEntityView techTreeItemEntityView = entityViewsDB.QueryEntityView<TechTreeItemEntityView>(item.Key.GetHashCode());
				techTreeItemEntityView.stateComponent.IsUnlockable.set_value(value.isUnlockable);
				techTreeItemEntityView.stateComponent.IsUnLocked.set_value(value.isUnlocked);
			}
			CubeTypeID cubeID = _nodeToUnlock.iDsComponent.CubeID;
			ISetNewInventoryCubesRequest request = serviceFactory.Create<ISetNewInventoryCubesRequest>();
			request.Inject(new HashSet<uint>
			{
				cubeID.ID
			});
			yield return new TaskService(request);
			cubeInventory.RefreshAndForget();
			int updatedTechPointsBalance = 0;
			yield return techPointsTracker.RefreshUserTechPointsAmountEnumerator(delegate(int balance)
			{
				updatedTechPointsBalance = balance;
			});
			FasterListEnumerator<ItemShopDisplayEntityView> enumerator2 = entityViewsDB.QueryEntityViews<ItemShopDisplayEntityView>().GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					ItemShopDisplayEntityView current2 = enumerator2.get_Current();
					current2.itemShopDisplayComponent.lastRefreshReason = RefreshReason.ShopRefresh;
					current2.itemShopDisplayComponent.refresh.set_value(true);
				}
			}
			finally
			{
				((IDisposable)enumerator2).Dispose();
			}
			IGetTechTreeDataRequest getTechTreeDataReq = serviceFactory.Create<IGetTechTreeDataRequest>();
			getTechTreeDataReq.ClearCache();
			CubeTypeData cubeData = cubeList.CubeTypeDataOf(cubeID);
			string sprite = (!cubeData.cubeData.localiseSprite) ? cubeData.spriteName : StringTableBase<StringTable>.Instance.GetString(cubeData.spriteName);
			yield return HandleAnalytics(cubeData, updatedTechPointsBalance);
			_celebrateUnlockDialogue.dialogueNodeComponent.nodeSprite = sprite;
			_celebrateUnlockDialogue.dialogueNodeComponent.nodeName = StringTableBase<StringTable>.Instance.GetString(cubeData.nameStrKey);
			_celebrateUnlockDialogue.gameObjectComponent.gameObject.SetActive(true);
			yield return HoverSlotInNextFrame(_nodeToUnlock.gameObjectComponent.gameObject);
		}

		private IEnumerator HandleAnalytics(CubeTypeData cubeData, int updatedTechPointsBalance)
		{
			_loadingIconPresenter.NotifyLoading("HandleAnalytics");
			yield return SendCubeUnlockedLog(cubeData);
			yield return SendCurrencySpentLog(cubeData, updatedTechPointsBalance);
			_loadingIconPresenter.NotifyLoadingDone("HandleAnalytics");
		}

		private IEnumerator SendCubeUnlockedLog(CubeTypeData cubeData)
		{
			int unlockedTechs = 0;
			TaskService<Dictionary<string, TechTreeItemData>> getTechTreeDataRequest = serviceFactory.Create<IGetTechTreeDataRequest>().AsTask();
			yield return getTechTreeDataRequest;
			if (!getTechTreeDataRequest.succeeded)
			{
				Console.LogError("Get Tech Tree Data Request failed. " + getTechTreeDataRequest.behaviour.exceptionThrown);
				yield break;
			}
			Dictionary<string, TechTreeItemData>.Enumerator enumerator = getTechTreeDataRequest.result.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.Value.isUnlocked)
				{
					unlockedTechs++;
				}
			}
			LogCubeUnlockedDependency cubeUnlockedDependency = new LogCubeUnlockedDependency(_nodeToUnlock.costComponent.TPCost, cubeData.nameStrKey, unlockedTechs);
			TaskService logCubeUnlockedRequest = analyticsRequestFactory.Create<ILogCubeUnlockedRequest, LogCubeUnlockedDependency>(cubeUnlockedDependency).AsTask();
			yield return logCubeUnlockedRequest;
			if (!logCubeUnlockedRequest.succeeded)
			{
				Console.LogError("Log Cube Unlocked Request failed. " + logCubeUnlockedRequest.behaviour.exceptionThrown);
			}
		}

		private IEnumerator SendCurrencySpentLog(CubeTypeData cubeData, int updatedTechPointsBalance)
		{
			LogPlayerCurrencySpentDependency playerCurrencySpentDependency = new LogPlayerCurrencySpentDependency("TechPoints", Convert.ToInt32(_nodeToUnlock.costComponent.TPCost), updatedTechPointsBalance, "TechTree", cubeData.nameStrKey);
			TaskService logPlayerCurrencySpentRequest = analyticsRequestFactory.Create<ILogPlayerCurrencySpentRequest, LogPlayerCurrencySpentDependency>(playerCurrencySpentDependency).AsTask();
			yield return logPlayerCurrencySpentRequest;
			if (!logPlayerCurrencySpentRequest.succeeded)
			{
				Console.LogError("Log Player Currency Spent Request failed. " + logPlayerCurrencySpentRequest.behaviour.exceptionThrown);
			}
		}

		private static IEnumerator HoverSlotInNextFrame(GameObject slot)
		{
			yield return null;
			UICamera.set_currentScheme(2);
			UICamera.set_hoveredObject(slot);
			UICamera.set_controllerNavigationObject(slot);
			UICamera.controller.current = slot;
		}

		private void DisplayCorrectDialogue(int techPointsBalance)
		{
			uint tPCost = _nodeToUnlock.costComponent.TPCost;
			if (techPointsBalance >= tPCost)
			{
				_confirmUnlockDialogue.dialogueLabelsComponent.TPCostLabel.set_text(tPCost.ToString());
				_confirmUnlockDialogue.gameObjectComponent.gameObject.SetActive(true);
			}
			else
			{
				_notEnoughTPDialogue.gameObjectComponent.gameObject.SetActive(true);
			}
		}
	}
}
