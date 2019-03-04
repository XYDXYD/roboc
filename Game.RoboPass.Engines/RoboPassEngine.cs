using Authentication;
using Game.RoboPass.Components;
using Game.RoboPass.EntityViews;
using Mothership;
using Mothership.ItemShop;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using Utility;

namespace Game.RoboPass.Engines
{
	internal class RoboPassEngine : IQueryingEntityViewEngine, IWaitForFrameworkDestruction, IEngine
	{
		private readonly ICubeInventory _cubeInventory;

		private readonly ICurrenciesTracker _currenciesTracker;

		private readonly IServiceRequestFactory _serviceReqFactory;

		private readonly ReloadRobopassObserver _reloadRobopassObserver;

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public RoboPassEngine(ICubeInventory cubeInventory, ICurrenciesTracker currenciesTracker, IServiceRequestFactory serviceReqFactory, ReloadRobopassObserver reloadRobopassObserver)
		{
			_cubeInventory = cubeInventory;
			_currenciesTracker = currenciesTracker;
			_serviceReqFactory = serviceReqFactory;
			_reloadRobopassObserver = reloadRobopassObserver;
		}

		public void Ready()
		{
			_reloadRobopassObserver.AddAction((Action)StartRefreshRoboPassAndRelatedDataTask);
			TaskRunner.get_Instance().Run(LoadRoboPassSeasonData(clearCache: false));
		}

		public void OnFrameworkDestroyed()
		{
			_reloadRobopassObserver.RemoveAction((Action)StartRefreshRoboPassAndRelatedDataTask);
		}

		private void StartRefreshRoboPassAndRelatedDataTask()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Expected O, but got Unknown
			SerialTaskCollection val = new SerialTaskCollection(2, (string)null);
			val.Add(LoadRoboPassSeasonData(clearCache: true));
			val.Add(LoadRoboPassRelatedData());
			TaskRunner.get_Instance().Run((IEnumerator)val);
		}

		private IEnumerator LoadRoboPassSeasonData(bool clearCache)
		{
			ILoadRoboPassSeasonConfigRequest loadRoboPassSeasonConfigReq = _serviceReqFactory.Create<ILoadRoboPassSeasonConfigRequest>();
			if (clearCache)
			{
				loadRoboPassSeasonConfigReq.ClearCache();
			}
			TaskService<RoboPassSeasonData> loadRoboPassSeasonConfigTS = loadRoboPassSeasonConfigReq.AsTask();
			yield return loadRoboPassSeasonConfigTS;
			if (!loadRoboPassSeasonConfigTS.succeeded)
			{
				throw new Exception("Failed to get RoboPass season data");
			}
			RoboPassSeasonData roboPassSeasonData = loadRoboPassSeasonConfigTS.result;
			FasterReadOnlyList<RoboPassSeasonDataEntityView> roboPassSeasonDataEVs = entityViewsDB.QueryEntityViews<RoboPassSeasonDataEntityView>();
			while (roboPassSeasonDataEVs.get_Count() == 0)
			{
				yield return null;
				roboPassSeasonDataEVs = entityViewsDB.QueryEntityViews<RoboPassSeasonDataEntityView>();
			}
			RoboPassSeasonDataEntityView roboPassSeasonDataEV = roboPassSeasonDataEVs.get_Item(0);
			IRoboPassSeasonInfoComponent roboPassSeasonInfoComp = roboPassSeasonDataEV.roboPassSeasonInfoComponent;
			if (roboPassSeasonData == null)
			{
				roboPassSeasonInfoComp.isValidSeason = false;
				roboPassSeasonInfoComp.dataUpdated.set_value(true);
				yield break;
			}
			RoboPassSeasonRewardData[][] gradesRewards = roboPassSeasonData.gradesRewards;
			int gradesHighestIndex = gradesRewards.Length - 1;
			TimeSpan timeRemaining = roboPassSeasonData.endDateTimeUTC - DateTime.Now;
			string robopassSeasonNameNotLocalized = roboPassSeasonData.nameString;
			string robopassSeasonName = StringTableBase<StringTable>.Instance.GetString(robopassSeasonNameNotLocalized);
			ILoadPlayerRoboPassSeasonRequest loadPlayerRoboPassSeasonReq = _serviceReqFactory.Create<ILoadPlayerRoboPassSeasonRequest>();
			if (clearCache)
			{
				loadPlayerRoboPassSeasonReq.ClearCache();
			}
			TaskService<PlayerRoboPassSeasonData> loadPlayerRoboPassSeasonTS = loadPlayerRoboPassSeasonReq.AsTask();
			yield return loadPlayerRoboPassSeasonTS;
			if (!loadPlayerRoboPassSeasonTS.succeeded)
			{
				throw new Exception("Failed to get RoboPass player season data");
			}
			PlayerRoboPassSeasonData playerRoboPassSeasonData = loadPlayerRoboPassSeasonTS.result;
			if (playerRoboPassSeasonData == null)
			{
				Console.Log("No season data was found for the player '" + User.DisplayName + "'");
				playerRoboPassSeasonData = new PlayerRoboPassSeasonData();
			}
			int currentGradeIndex = playerRoboPassSeasonData.currentGradeIndex;
			int deltaXPToShow = playerRoboPassSeasonData.deltaXpToShow;
			float progressInGrade = playerRoboPassSeasonData.progressInGrade;
			bool hasDeluxe = playerRoboPassSeasonData.hasDeluxe;
			roboPassSeasonInfoComp.gradesHighestIndex = gradesHighestIndex;
			roboPassSeasonInfoComp.gradesRewards = gradesRewards;
			roboPassSeasonInfoComp.robopassSeasonNameKey = robopassSeasonNameNotLocalized;
			roboPassSeasonInfoComp.robopassSeasonName = robopassSeasonName;
			roboPassSeasonInfoComp.timeRemaining = timeRemaining;
			roboPassSeasonInfoComp.xpBetweenGrades = roboPassSeasonData.xpBetweenGrades;
			roboPassSeasonInfoComp.isValidSeason = true;
			roboPassSeasonInfoComp.dataUpdated.set_value(true);
			IRoboPassSeasonPlayerInfoComponent roboPassSeasonPlayerXPComp = roboPassSeasonDataEV.roboPassSeasonPlayerInfoComponent;
			roboPassSeasonPlayerXPComp.currentGradeIndex = currentGradeIndex;
			roboPassSeasonPlayerXPComp.deltaXPToShow = deltaXPToShow;
			roboPassSeasonPlayerXPComp.hasDeluxe = hasDeluxe;
			roboPassSeasonPlayerXPComp.progressInGrade = progressInGrade;
			roboPassSeasonPlayerXPComp.dataUpdated.set_value(true);
		}

		private IEnumerator LoadRoboPassRelatedData()
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
