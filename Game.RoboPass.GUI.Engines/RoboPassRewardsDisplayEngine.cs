using Game.RoboPass.Components;
using Game.RoboPass.EntityViews;
using Game.RoboPass.GUI.Components;
using Game.RoboPass.GUI.EntityViews;
using Mothership;
using Mothership.ItemShop;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;

namespace Game.RoboPass.GUI.Engines
{
	internal class RoboPassRewardsDisplayEngine : MultiEntityViewsEngine<RoboPassSeasonScreenEntityView, RoboPassSeasonDataEntityView>, IQueryingEntityViewEngine, IEngine
	{
		private const int NOT_VALID_PAGE_NUMBER = -1;

		private readonly ICubeInventory _cubeInventory;

		private readonly ICurrenciesTracker _currenciesTracker;

		private readonly IServiceRequestFactory _serviceReqFactory;

		private readonly RobopassScreenFactory _robopassScreenFactory;

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public RoboPassRewardsDisplayEngine(ICubeInventory cubeInventory, ICurrenciesTracker currenciesTracker, RobopassScreenFactory robopassScreenFactory, IServiceRequestFactory serviceReqFactory)
		{
			_cubeInventory = cubeInventory;
			_currenciesTracker = currenciesTracker;
			_robopassScreenFactory = robopassScreenFactory;
			_serviceReqFactory = serviceReqFactory;
		}

		public void Ready()
		{
		}

		protected override void Add(RoboPassSeasonScreenEntityView entityView)
		{
			_robopassScreenFactory.BuildRewardsUI(entityView.rewardsGridsComponent);
			entityView.rewardsGridsComponent.pageChanged.NotifyOnValueSet((Action<int, int>)RefreshRewardsUi);
		}

		protected override void Remove(RoboPassSeasonScreenEntityView entityView)
		{
			entityView.rewardsGridsComponent.pageChanged.StopNotify((Action<int, int>)RefreshRewardsUi);
		}

		protected override void Add(RoboPassSeasonDataEntityView entityView)
		{
			entityView.roboPassSeasonPlayerInfoComponent.dataUpdated.NotifyOnValueSet((Action<int, bool>)RefreshRewardsUi);
		}

		protected override void Remove(RoboPassSeasonDataEntityView entityView)
		{
			entityView.roboPassSeasonPlayerInfoComponent.dataUpdated.StopNotify((Action<int, bool>)RefreshRewardsUi);
		}

		private void RefreshRewardsUi(int entityID, int currentPageNumber)
		{
			RoboPassSeasonScreenEntityView roboPassSeasonScreenEV = entityViewsDB.QueryEntityView<RoboPassSeasonScreenEntityView>(entityID);
			RefreshRewardsUi(roboPassSeasonScreenEV, currentPageNumber);
		}

		private void RefreshRewardsUi(int entityID, bool dataUpdated)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			if (dataUpdated)
			{
				RoboPassSeasonScreenEntityView roboPassSeasonScreenEV = entityViewsDB.QueryEntityViews<RoboPassSeasonScreenEntityView>().get_Item(0);
				RefreshRewardsUi(roboPassSeasonScreenEV, -1);
				TaskRunner.get_Instance().Run((Func<IEnumerator>)RefreshInventoryDataAfterUnlockingReward);
			}
		}

		private void RefreshRewardsUi(RoboPassSeasonScreenEntityView roboPassSeasonScreenEV, int currentPageNumber)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			IRewardsGridsComponent rewardsGridsComponent = roboPassSeasonScreenEV.rewardsGridsComponent;
			int columnLimit = rewardsGridsComponent.columnLimit;
			RoboPassSeasonDataEntityView roboPassSeasonDataEntityView = entityViewsDB.QueryEntityViews<RoboPassSeasonDataEntityView>().get_Item(0);
			IRoboPassSeasonInfoComponent roboPassSeasonInfoComponent = roboPassSeasonDataEntityView.roboPassSeasonInfoComponent;
			int gradesHighestIndex = roboPassSeasonInfoComponent.gradesHighestIndex;
			IRoboPassSeasonPlayerInfoComponent roboPassSeasonPlayerInfoComponent = roboPassSeasonDataEntityView.roboPassSeasonPlayerInfoComponent;
			int num = roboPassSeasonPlayerInfoComponent.currentGradeIndex + 1;
			rewardsGridsComponent.maxPageNumber = Mathf.CeilToInt((float)roboPassSeasonInfoComponent.gradesHighestIndex / (float)columnLimit);
			if (currentPageNumber == -1)
			{
				currentPageNumber = Mathf.CeilToInt((float)num / (float)columnLimit);
			}
			rewardsGridsComponent.currentPageNumber = currentPageNumber;
			FasterReadOnlyList<RoboPassRewardEntityView> val = entityViewsDB.QueryEntityViews<RoboPassRewardEntityView>();
			int count = val.get_Count();
			int num2 = currentPageNumber - 1;
			int num3 = columnLimit * num2 + 1;
			RoboPassSeasonRewardData[][] gradesRewards = roboPassSeasonInfoComponent.gradesRewards;
			int i = 0;
			int num4 = num3;
			for (; i < count; i++)
			{
				RoboPassRewardEntityView roboPassRewardEntityView = val.get_Item(i);
				if (i > 0 && i % 2 == 0)
				{
					num4++;
				}
				IRoboPassRewardUICellComponent roboPassRewardUiCellComponent = roboPassRewardEntityView.roboPassRewardUiCellComponent;
				roboPassRewardUiCellComponent.visible = true;
				roboPassRewardUiCellComponent.rewardGradeLabel = num4.ToString();
				roboPassRewardUiCellComponent.rewardUnlockedWidgetVisible = false;
				roboPassRewardUiCellComponent.rewardLockedWidgetVisible = (num4 > num);
				int num5 = num4 - 1;
				if (num5 <= gradesHighestIndex)
				{
					roboPassRewardUiCellComponent.rewardName = null;
					roboPassRewardUiCellComponent.rewardSprite = null;
					roboPassRewardUiCellComponent.rewardType = null;
					RoboPassSeasonRewardData[] array = gradesRewards[num5];
					RoboPassSeasonRewardData[] array2 = array;
					foreach (RoboPassSeasonRewardData roboPassSeasonRewardData in array2)
					{
						if (roboPassSeasonRewardData.isDeluxe == roboPassRewardUiCellComponent.isDeluxeCell)
						{
							roboPassRewardUiCellComponent.rewardName = roboPassSeasonRewardData.rewardName;
							roboPassRewardUiCellComponent.isSpriteFullSize = roboPassSeasonRewardData.spriteFullSize;
							roboPassRewardUiCellComponent.rewardSprite = roboPassSeasonRewardData.spriteName;
							roboPassRewardUiCellComponent.rewardType = roboPassSeasonRewardData.categoryName;
							if (num4 <= num)
							{
								roboPassRewardUiCellComponent.rewardUnlockedWidgetVisible = true;
							}
						}
					}
				}
				else
				{
					roboPassRewardUiCellComponent.visible = false;
				}
			}
		}

		private IEnumerator RefreshInventoryDataAfterUnlockingReward()
		{
			_cubeInventory.RefreshAndForget();
			yield return _currenciesTracker.RefreshUserWalletEnumerator();
			FasterListEnumerator<ItemShopDisplayEntityView> enumerator = entityViewsDB.QueryEntityViews<ItemShopDisplayEntityView>().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ItemShopDisplayEntityView current = enumerator.get_Current();
					IItemShopDisplayComponent itemShopDisplayComponent = current.itemShopDisplayComponent;
					itemShopDisplayComponent.lastRefreshReason = RefreshReason.ShopRefresh;
					itemShopDisplayComponent.refresh.set_value(true);
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			ILoadAllCustomisationInfoRequest loadAllCustomisationInfoReq = _serviceReqFactory.Create<ILoadAllCustomisationInfoRequest>();
			loadAllCustomisationInfoReq.ClearCache();
			yield return loadAllCustomisationInfoReq.AsTask();
		}
	}
}
