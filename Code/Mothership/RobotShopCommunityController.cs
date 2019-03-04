using Achievements;
using Authentication;
using Mothership.RobotShop;
using Services.Analytics;
using Svelto.Command;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Mothership
{
	internal sealed class RobotShopCommunityController : IInitialize, IWaitForFrameworkDestruction
	{
		private const int PAGE_SIZE = 50;

		private const int POLL_TIMES = 5;

		private const int TOP_ROBOTS_NUM = 5;

		private const int STARTING_PAGE = 1;

		private FasterList<CRFItem> _items;

		private HashSet<int> _itemKeys;

		private bool _allLoaded;

		private bool _showTopRobots;

		private int _partCategoryKey;

		private int _requestedItemIndex;

		private RobotShopCommunityListView _view;

		private RobotShopModelView _modelView;

		private readonly CRFShopItemListDependency _parameters = new CRFShopItemListDependency();

		private readonly List<CubeTypeData> _allPartList = new List<CubeTypeData>();

		private readonly List<ItemCategory> _partCategoryList = new List<ItemCategory>();

		private readonly RobotShopFilterStringsHelper _robotShopFilterStringsHelper = new RobotShopFilterStringsHelper();

		private readonly Dictionary<ItemCategory, List<CubeTypeData>> _partsByCategory = new Dictionary<ItemCategory, List<CubeTypeData>>();

		private TiersData _tiersData;

		[Inject]
		internal IServiceRequestFactory serviceFactory
		{
			private get;
			set;
		}

		[Inject]
		internal RobotShopObserver observer
		{
			private get;
			set;
		}

		[Inject]
		internal LoadingIconPresenter loadingPresenter
		{
			private get;
			set;
		}

		[Inject]
		internal ICurrenciesTracker currenciesTracker
		{
			private get;
			set;
		}

		[Inject]
		internal ICubeList cubeCatalogue
		{
			private get;
			set;
		}

		[Inject]
		internal ICPUPower cpuPower
		{
			private get;
			set;
		}

		[Inject]
		internal LocalisationWrapper localiseWrapper
		{
			private get;
			set;
		}

		[Inject]
		internal CrfItemListLoader crfItemListLoader
		{
			private get;
			set;
		}

		[Inject]
		internal ICommandFactory commandFactory
		{
			private get;
			set;
		}

		[Inject]
		internal RobotCostCalculator robotCostCalculator
		{
			private get;
			set;
		}

		[Inject]
		internal IAchievementManager achievementManager
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

		void IInitialize.OnDependenciesInjected()
		{
			observer.OnShowRobotShopListEvent += Show;
			observer.OnRobotShopOpenedEvent += Show;
			observer.OnHideRobotShopEvent += Hide;
			observer.OnRobotInvalidatedEvent += RobotInvalidated;
			LocalisationWrapper localiseWrapper = this.localiseWrapper;
			localiseWrapper.OnLocalisationChanged = (Action)Delegate.Combine(localiseWrapper.OnLocalisationChanged, new Action(RefreshFilterStrings));
			TaskRunner.get_Instance().Run((Func<IEnumerator>)InitFilterStrings);
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			observer.OnShowRobotShopListEvent -= Show;
			observer.OnRobotShopOpenedEvent -= Show;
			observer.OnHideRobotShopEvent -= Hide;
			observer.OnRobotInvalidatedEvent -= RobotInvalidated;
			LocalisationWrapper localiseWrapper = this.localiseWrapper;
			localiseWrapper.OnLocalisationChanged = (Action)Delegate.Remove(localiseWrapper.OnLocalisationChanged, new Action(RefreshFilterStrings));
			if (_view != null)
			{
				_view.OnRobotPreviewEvent -= OnRobotPreview;
				_view.OnSearchParametersUpdatedEvent -= OnSearchParametersUpdated;
				_view.OnTextSearchStringUpdatedEvent -= OnTextSearchStringUpdated;
				_view.ShowMoreEvent -= ShowMore;
				RobotShopCommunityListView view = _view;
				view.OnClaimCommunityShopEarningsEvent = (Action)Delegate.Remove(view.OnClaimCommunityShopEarningsEvent, new Action(ClaimCommunityShopEarnings));
			}
			if (_modelView != null)
			{
				_modelView.OnDevOnlySetFeaturedRequestedEvent -= OnDevOnlySetFeatured;
				_modelView.OnDevOnlyHideFeaturedRequestedEvent -= OnDevOnlyHideFeatured;
				_modelView.OnDevOnlyRestoreFeaturedRequestedEvent -= OnDevOnlyRestoreFeatured;
				_modelView.OnRemovePlayerShopSubmissionRequestedEvent -= OnRemovePlayerShopSubmissionRequested;
				_modelView.OnReportAbuseRequestedEvent -= OnReportAbuseRequested;
				_modelView.OnPreviousRobotRequestedEvent -= ShowPreviousRobot;
				_modelView.OnNextRobotRequestedEvent -= ShowNextRobot;
				_modelView.OnCopyRobotRequestedEvent -= CopyRobot;
			}
		}

		public void SetupView(RobotShopCommunityListView view)
		{
			_view = view;
			_view.OnRobotPreviewEvent += OnRobotPreview;
			_view.OnSearchParametersUpdatedEvent += OnSearchParametersUpdated;
			_view.OnTextSearchStringUpdatedEvent += OnTextSearchStringUpdated;
			_view.ShowMoreEvent += ShowMore;
			RobotShopCommunityListView view2 = _view;
			view2.OnClaimCommunityShopEarningsEvent = (Action)Delegate.Combine(view2.OnClaimCommunityShopEarningsEvent, new Action(ClaimCommunityShopEarnings));
		}

		public void SetupModelView(RobotShopModelView modelView)
		{
			_modelView = modelView;
			_modelView.OnDevOnlySetFeaturedRequestedEvent += OnDevOnlySetFeatured;
			_modelView.OnDevOnlyHideFeaturedRequestedEvent += OnDevOnlyHideFeatured;
			_modelView.OnDevOnlyRestoreFeaturedRequestedEvent += OnDevOnlyRestoreFeatured;
			_modelView.OnRemovePlayerShopSubmissionRequestedEvent += OnRemovePlayerShopSubmissionRequested;
			_modelView.OnReportAbuseRequestedEvent += OnReportAbuseRequested;
			_modelView.OnPreviousRobotRequestedEvent += ShowPreviousRobot;
			_modelView.OnNextRobotRequestedEvent += ShowNextRobot;
			_modelView.OnCopyRobotRequestedEvent += CopyRobot;
		}

		private IEnumerator InitFilterStrings()
		{
			ILoadRobotShopConfigRequest loadRobotShopConfigRequest = serviceFactory.Create<ILoadRobotShopConfigRequest>();
			TaskService<LoadRobotShopConfigResponse> loadRobotShopConfigTask = new TaskService<LoadRobotShopConfigResponse>(loadRobotShopConfigRequest);
			yield return new HandleTaskServiceWithError(loadRobotShopConfigTask, delegate
			{
				loadingPresenter.NotifyLoading("RobotShopLoadingScreen");
			}, delegate
			{
				loadingPresenter.NotifyLoadingDone("RobotShopLoadingScreen");
			}).GetEnumerator();
			if (cubeCatalogue.cubeKeysWithoutObsolete == null || cubeCatalogue.cubeKeysWithoutObsolete.get_Count() < 0)
			{
				yield return null;
			}
			FasterList<ItemDescriptor> descriptorList = new FasterList<ItemDescriptor>();
			FasterList<CubeTypeID> cubeKeys = cubeCatalogue.cubeKeysWithoutObsolete;
			for (int i = 0; i < cubeKeys.get_Count(); i++)
			{
				CubeTypeID index = cubeKeys.get_Item(i);
				CubeTypeData cubeTypeData = cubeCatalogue.CubeTypeDataOf(index);
				if (cubeTypeData == null)
				{
					continue;
				}
				ItemDescriptor itemDescriptor = cubeTypeData.cubeData.itemDescriptor;
				if ((cubeTypeData.cubeData.itemType == ItemType.Weapon || cubeTypeData.cubeData.itemType == ItemType.Movement) && !descriptorList.Contains(itemDescriptor))
				{
					_allPartList.Add(cubeTypeData);
					ItemCategory itemCategory = itemDescriptor.itemCategory;
					if (!_partsByCategory.ContainsKey(itemCategory))
					{
						_partsByCategory[itemCategory] = new List<CubeTypeData>();
					}
					_partsByCategory[itemCategory].Add(cubeTypeData);
					if (!_partCategoryList.Contains(itemDescriptor.itemCategory))
					{
						_partCategoryList.Add(itemDescriptor.itemCategory);
					}
					descriptorList.Add(itemDescriptor);
				}
			}
			ILoadTiersBandingRequest request = serviceFactory.Create<ILoadTiersBandingRequest>();
			TaskService<TiersData> task = new TaskService<TiersData>(request);
			task.Execute();
			_tiersData = task.result;
			if (_view == null)
			{
				yield return null;
			}
			InitialiseFilters();
		}

		private void InitialiseFilters()
		{
			FasterList<string> filterStrings = _robotShopFilterStringsHelper.GenerateSortByFilter();
			_view.InitialiseFilterString(RobotShopFilter.SortBy, filterStrings);
			FasterList<string> filterStrings2 = _robotShopFilterStringsHelper.GenerateSearchByFilter();
			_view.InitialiseFilterString(RobotShopFilter.SearchBy, filterStrings2);
			FasterList<string> filterStrings3 = _robotShopFilterStringsHelper.GeneratePartCategoryFilter(_partCategoryList);
			_view.InitialiseFilterString(RobotShopFilter.PartCategories, filterStrings3);
			FasterList<string> filterStrings4 = _robotShopFilterStringsHelper.GenerateRobotTierFilter(_tiersData);
			_view.InitialiseFilterString(RobotShopFilter.RobotTier, filterStrings4);
		}

		private void Show()
		{
			_view.Show();
			Load();
		}

		private void Hide()
		{
			_view.Hide();
			_requestedItemIndex = 0;
			if (crfItemListLoader.isLoading)
			{
				crfItemListLoader.AbortLastLoad();
				loadingPresenter.NotifyLoadingDone("RobotShopLoadingScreen");
			}
		}

		private void RobotInvalidated()
		{
			Console.Log("forcing reload of shop robot index: " + _requestedItemIndex);
			OnRobotPreview(_requestedItemIndex, clearTheCache: true);
		}

		private void Load()
		{
			LoadEarnings();
			InitSearch(showTopRobots: true);
		}

		private void OnOperationFailed(ServiceBehaviour behaviour)
		{
			loadingPresenter.NotifyLoadingDone("RobotShopLoadingScreen");
			behaviour.SetAlternativeBehaviour(delegate
			{
			}, StringTableBase<StringTable>.Instance.GetString("strCancel"));
			ErrorWindow.ShowServiceErrorWindow(behaviour, delegate
			{
				loadingPresenter.NotifyLoading("RobotShopLoadingScreen");
			});
		}

		private void InitSearch(bool showTopRobots)
		{
			_items = new FasterList<CRFItem>();
			_itemKeys = new HashSet<int>();
			_allLoaded = false;
			_parameters.page = 1u;
			_parameters.pageSize = 50u;
			_showTopRobots = showTopRobots;
			ExecuteRequest();
		}

		private void LoadEarnings()
		{
			_view.earningButton.set_isEnabled(false);
			ILoadShopEarningsRequest loadShopEarningsRequest = serviceFactory.Create<ILoadShopEarningsRequest>();
			loadShopEarningsRequest.SetAnswer(new ServiceAnswer<LoadShopEarningsResponse>(OnShopEarningsLoaded, OnLoadEarningsFailed));
			loadShopEarningsRequest.Execute();
		}

		private void OnShopEarningsLoaded(LoadShopEarningsResponse earnings)
		{
			_view.LoadEarnings(earnings);
		}

		private void OnLoadEarningsFailed(ServiceBehaviour behaviour)
		{
			LoadShopEarningsResponse earnings = new LoadShopEarningsResponse();
			_view.LoadEarnings(earnings);
			ErrorWindow.ShowServiceErrorWindow(behaviour);
		}

		private void ClaimCommunityShopEarnings()
		{
			loadingPresenter.NotifyLoading("RobotShopLoadingScreen");
			IClaimShopEarningsRequest claimShopEarningsRequest = serviceFactory.Create<IClaimShopEarningsRequest>();
			claimShopEarningsRequest.SetAnswer(new ServiceAnswer<LoadShopEarningsResponse>(HandleCommunityShopEarningsCollected, OnOperationFailed));
			claimShopEarningsRequest.Execute();
		}

		private void HandleCommunityShopEarningsCollected(LoadShopEarningsResponse response)
		{
			loadingPresenter.NotifyLoadingDone("RobotShopLoadingScreen");
			_view.ShowCollectedEarningsPopup(response.earnCount, response.earnings);
			LoadEarnings();
			currenciesTracker.RefreshWallet(delegate(Wallet wallet)
			{
				TaskRunner.get_Instance().Run(HandleAnalytics(response.earnCount, response.earnings, wallet.RobitsBalance));
			});
			achievementManager.EarnRobitsFromCRF(response.earnCount);
		}

		private void ShowMore()
		{
			_parameters.page++;
			observer.OnShowMoreRobots();
			ExecuteRequest();
		}

		private void ExecuteRequest()
		{
			TaskRunner.get_Instance().Run((Func<IEnumerator>)LoadRobots);
		}

		private IEnumerator HandleAnalytics(int earnCount, int earnings, long robitsBalance)
		{
			loadingPresenter.NotifyLoading("HandleAnalytics");
			LogRobotShopCollectedEarningsDependency collectedEarningsDependency = new LogRobotShopCollectedEarningsDependency(earnCount, earnings);
			TaskService logRobotShopCollectedEarningsRequest = analyticsRequestFactory.Create<ILogRobotShopCollectedEarningsRequest, LogRobotShopCollectedEarningsDependency>(collectedEarningsDependency).AsTask();
			yield return logRobotShopCollectedEarningsRequest;
			if (!logRobotShopCollectedEarningsRequest.succeeded)
			{
				Console.LogError("Log RobotShop Collected Earnings Request failed. " + logRobotShopCollectedEarningsRequest.behaviour.exceptionThrown);
			}
			LogPlayerCurrencyEarnedDependency playerCurrencyEarnedDependency = new LogPlayerCurrencyEarnedDependency(CurrencyType.Robits.ToString(), earnings, robitsBalance, 0, "Factory", string.Empty);
			TaskService logPlayerCurrencyEarnedRequest = analyticsRequestFactory.Create<ILogPlayerCurrencyEarnedRequest, LogPlayerCurrencyEarnedDependency>(playerCurrencyEarnedDependency).AsTask();
			yield return logPlayerCurrencyEarnedRequest;
			if (!logPlayerCurrencyEarnedRequest.succeeded)
			{
				Console.LogError("Log Player Earned Currency Request failed. " + logPlayerCurrencyEarnedRequest.behaviour.exceptionThrown);
			}
			loadingPresenter.NotifyLoadingDone("HandleAnalytics");
		}

		private IEnumerator LoadRobots()
		{
			loadingPresenter.NotifyLoading("RobotShopLoadingScreen");
			List<CRFItem> newItems = new List<CRFItem>();
			int totalDiscardedRobots = 0;
			for (int i = 0; i < 5; i++)
			{
				List<CRFItem> robotShopItemList = null;
				crfItemListLoader.LoadItemList(_parameters, delegate(List<CRFItem> results)
				{
					robotShopItemList = results;
				}, OnItemFailedToLoad);
				while (robotShopItemList == null)
				{
					yield return null;
				}
				int discardedRobotsCount;
				List<CRFItem> setupItems = SetupResponseList(robotShopItemList, out discardedRobotsCount);
				totalDiscardedRobots += discardedRobotsCount;
				if (setupItems.Count == 0)
				{
					break;
				}
				newItems.AddRange(setupItems);
				if (newItems.Count >= 50)
				{
					newItems = newItems.GetRange(0, 50);
					totalDiscardedRobots += newItems.Count - 50;
					break;
				}
				_parameters.page++;
			}
			if (totalDiscardedRobots > 0)
			{
				_parameters.page--;
			}
			List<CRFItem> sortedList = SortResponseList(newItems);
			if (sortedList.Count < 50)
			{
				_allLoaded = true;
			}
			for (int j = 0; j < sortedList.Count; j++)
			{
				CRFItem cRFItem = sortedList[j];
				if (!_itemKeys.Contains(cRFItem.robotShopItem.id) && !cRFItem.isExpired)
				{
					_itemKeys.Add(cRFItem.robotShopItem.id);
					_items.Add(cRFItem);
				}
			}
			_view.DisplayItems(_items, !_allLoaded);
			loadingPresenter.NotifyLoadingDone("RobotShopLoadingScreen");
		}

		private void OnItemFailedToLoad(Exception e)
		{
			loadingPresenter.NotifyLoadingDone("RobotShopLoadingScreen");
			Console.LogException(e);
			GenericErrorData error = new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strRobotShopError"), StringTableBase<StringTable>.Instance.GetString("strGetRobotShopListError"));
			ErrorWindow.ShowErrorWindow(error);
		}

		private List<CRFItem> SetupResponseList(List<CRFItem> itemList, out int discardedRobotCount)
		{
			discardedRobotCount = 0;
			if (itemList.Count == 0)
			{
				return itemList;
			}
			List<CRFItem> list = new List<CRFItem>();
			bool flag = _parameters.minimumCpu == cpuPower.MaxCpuPower;
			int minRobotRanking = _parameters.minRobotRanking;
			int maxRobotRanking = _parameters.maxRobotRanking;
			for (int i = 0; i < itemList.Count; i++)
			{
				CRFItem cRFItem = itemList[i];
				if (!_itemKeys.Contains(Convert.ToInt32(cRFItem.robotShopItem.id)))
				{
					SetupItem(cRFItem);
					bool flag2 = true;
					flag2 = ((!flag) ? (!cRFItem.isMegabot && (minRobotRanking == -1 || cRFItem.robotShopItem.totalRobotRanking >= minRobotRanking) && (maxRobotRanking == -1 || cRFItem.robotShopItem.totalRobotRanking <= maxRobotRanking) && cRFItem.robotCPUToPlayer <= cpuPower.MaxMegabotCpuPower) : (cRFItem.isMegabot && cRFItem.robotCPUToPlayer <= cpuPower.MaxMegabotCpuPower));
					if (i == 0 && cRFItem.robotShopItem.featured)
					{
						flag2 = true;
					}
					if (cRFItem.LockedCubes != null && cRFItem.LockedCubes.Count > 0)
					{
						flag2 &= _parameters.showLockedRobots;
					}
					if (flag2)
					{
						list.Add(cRFItem);
					}
					else
					{
						discardedRobotCount++;
					}
					if (list.Count == 50)
					{
						break;
					}
				}
			}
			return list;
		}

		private List<CRFItem> SortResponseList(List<CRFItem> itemList)
		{
			if (itemList.Count == 0)
			{
				return itemList;
			}
			CRFItem cRFItem = null;
			if (itemList[0].robotShopItem.featured)
			{
				cRFItem = itemList[0];
				itemList.RemoveAt(0);
			}
			List<int> list = new List<int>();
			List<CRFItem> list2 = new List<CRFItem>();
			List<CRFItem> list3 = new List<CRFItem>();
			List<CRFItem> list4 = new List<CRFItem>();
			itemList.Sort(SortCRFList);
			for (int i = 0; i < itemList.Count; i++)
			{
				CRFItem cRFItem2 = itemList[i];
				if (_showTopRobots && _parameters.sortMode == ItemSortMode.MOST_BOUGHT && list.Count < 5 && !cRFItem2.isExpired && !list.Contains(cRFItem2.robotShopItem.id))
				{
					list.Add(cRFItem2.robotShopItem.id);
					list2.Add(cRFItem2);
				}
				else if (!cRFItem2.playerOwnAllCubes || cRFItem2.isMyRobot)
				{
					list4.Add(cRFItem2);
				}
				else
				{
					list3.Add(cRFItem2);
				}
			}
			list3.Sort(SortCRFList);
			list4.Sort(SortCRFList);
			List<CRFItem> list5 = new List<CRFItem>();
			if (cRFItem != null)
			{
				list5.Add(cRFItem);
			}
			list5.AddRange(list2);
			list5.AddRange(list3);
			list5.AddRange(list4);
			return list5;
		}

		private int SortCRFList(CRFItem itemA, CRFItem itemB)
		{
			int num = 0;
			switch (_parameters.sortMode)
			{
			case ItemSortMode.SUGGESTED:
				num = ((!(Random.get_value() < 0.5f)) ? 1 : (-1));
				break;
			case ItemSortMode.ADDED:
				num = itemB.robotShopItem.addedDate.CompareTo(itemA.robotShopItem.addedDate);
				break;
			case ItemSortMode.COMBAT_RATING:
				num = itemB.robotShopItem.combatRating.CompareTo(itemA.robotShopItem.combatRating);
				break;
			case ItemSortMode.COSMETIC_RATING:
				num = itemB.robotShopItem.styleRating.CompareTo(itemA.robotShopItem.styleRating);
				break;
			case ItemSortMode.CPU:
				num = itemB.robotCPUToPlayer.CompareTo(itemA.robotCPUToPlayer);
				break;
			case ItemSortMode.MOST_BOUGHT:
				num = itemB.robotShopItem.rentCount.CompareTo(itemA.robotShopItem.rentCount);
				break;
			}
			if (num == 0)
			{
				num = itemB.robotShopItem.addedDate.CompareTo(itemA.robotShopItem.addedDate);
			}
			return num;
		}

		private void SetupItem(CRFItem item)
		{
			item.isMyRobot = item.robotShopItem.addedBy.Equals(User.Username);
			item.robotCPUToPlayer = RobotCPUCalculator.CalculateRobotCPU(item.robotShopItem.cubeCounts, cubeCatalogue, cpuPower.MaxCosmeticCpuPool);
			item.isMegabot = (item.robotCPUToPlayer > cpuPower.MaxCpuPower);
			item.TierStr = RRAndTiers.ConvertRobotRankingToTierString((uint)item.robotShopItem.totalRobotRanking, item.isMegabot, _tiersData);
			HashSet<uint> hashSet = new HashSet<uint>();
			foreach (KeyValuePair<uint, uint> cubeCount in item.robotShopItem.cubeCounts)
			{
				hashSet.Add(cubeCount.Key);
			}
			robotCostCalculator.GetListOfUnlockedCubes(hashSet, out Dictionary<uint, SunkCube> cubeTypesThatAreLockedForPlayer);
			List<uint> list2 = item.LockedCubes = new List<uint>(cubeTypesThatAreLockedForPlayer.Keys);
		}

		private void OnRobotPreview(int index)
		{
			_requestedItemIndex = index;
			OnRobotPreview(_items.get_Item(_requestedItemIndex), clearTheCache: false);
		}

		private void OnRobotPreview(int index, bool clearTheCache)
		{
			_requestedItemIndex = index;
			OnRobotPreview(_items.get_Item(_requestedItemIndex), clearTheCache);
		}

		private void OnRobotPreview(CRFItem item, bool clearTheCache)
		{
			loadingPresenter.NotifyLoading("RobotShopLoadingScreen");
			ILoadCRFShopItemDataRequest loadCRFShopItemDataRequest = serviceFactory.Create<ILoadCRFShopItemDataRequest, int>(item.robotShopItem.id);
			if (clearTheCache)
			{
				loadCRFShopItemDataRequest.ClearTheCache();
			}
			loadCRFShopItemDataRequest.SetAnswer(new ServiceAnswer<LoadCRFShopItemDataRequestResponse>(delegate(LoadCRFShopItemDataRequestResponse response)
			{
				OnRobotShopItemDataLoaded(item, response);
			}, OnOperationFailed));
			loadCRFShopItemDataRequest.Execute();
		}

		private void CopyRobot()
		{
			CRFItem item = _items.get_Item(_requestedItemIndex);
			commandFactory.Build<CopyRobotFromCRFCommand>().Inject(item).Execute();
		}

		private void OnDevOnlySetFeatured()
		{
			OnDevOnlySetFeatured(_requestedItemIndex);
		}

		private void OnDevOnlySetFeatured(int index)
		{
			loadingPresenter.NotifyLoading("RobotShopLoadingScreen");
			int id = _items.get_Item(_requestedItemIndex).robotShopItem.id;
			ISetCRFRobotAsFeaturedRequest setCRFRobotAsFeaturedRequest = serviceFactory.Create<ISetCRFRobotAsFeaturedRequest, int>(id);
			setCRFRobotAsFeaturedRequest.SetAnswer(new ServiceAnswer(OnFeaturedRobotRequestCompleted, OnFeaturedRobotRequestFailed));
			setCRFRobotAsFeaturedRequest.Execute();
		}

		private void OnDevOnlyHideFeatured()
		{
			OnDevOnlyHideFeatured(_requestedItemIndex);
		}

		private void OnDevOnlyHideFeatured(int index)
		{
			loadingPresenter.NotifyLoading("RobotShopLoadingScreen");
			int id = _items.get_Item(_requestedItemIndex).robotShopItem.id;
			IHideCRFFeaturedRobotRequest hideCRFFeaturedRobotRequest = serviceFactory.Create<IHideCRFFeaturedRobotRequest, int>(id);
			hideCRFFeaturedRobotRequest.SetAnswer(new ServiceAnswer(OnFeaturedRobotRequestCompleted, OnFeaturedRobotRequestFailed));
			hideCRFFeaturedRobotRequest.Execute();
		}

		private void OnDevOnlyRestoreFeatured()
		{
			OnDevOnlyRestoreFeatured(_requestedItemIndex);
		}

		private void OnDevOnlyRestoreFeatured(int index)
		{
			loadingPresenter.NotifyLoading("RobotShopLoadingScreen");
			int id = _items.get_Item(_requestedItemIndex).robotShopItem.id;
			IRestoreCRFFeaturedRobotRequest restoreCRFFeaturedRobotRequest = serviceFactory.Create<IRestoreCRFFeaturedRobotRequest, int>(id);
			restoreCRFFeaturedRobotRequest.SetAnswer(new ServiceAnswer(OnFeaturedRobotRequestCompleted, OnFeaturedRobotRequestFailed));
			restoreCRFFeaturedRobotRequest.Execute();
		}

		private void OnFeaturedRobotRequestCompleted()
		{
			loadingPresenter.NotifyLoadingDone("RobotShopLoadingScreen");
			observer.OnRobotFeatureChange();
			RestartSearch();
		}

		private void OnFeaturedRobotRequestFailed(ServiceBehaviour behaviour)
		{
			loadingPresenter.NotifyLoadingDone("RobotShopLoadingScreen");
			ErrorWindow.ShowServiceErrorWindow(behaviour);
		}

		private void OnRobotShopItemDataLoaded(CRFItem item, LoadCRFShopItemDataRequestResponse data)
		{
			_items.get_Item(_requestedItemIndex).robotShopItem.SetCubeData(data.robotData);
			observer.OnShowRobot(data.robotData, data.colorData, item.robotShopItem.name, item.robotShopItem.id, item.robotCPUToPlayer);
			bool hasMultipleRobots = 1 < _items.get_Count();
			_modelView.ShowItem(item, hasMultipleRobots);
			loadingPresenter.NotifyLoadingDone("RobotShopLoadingScreen");
		}

		private void ShowPreviousRobot()
		{
			int index = (_requestedItemIndex != 0) ? (_requestedItemIndex - 1) : (_items.get_Count() - 1);
			OnRobotPreview(index);
		}

		private void ShowNextRobot()
		{
			int index = (_requestedItemIndex != _items.get_Count() - 1) ? (_requestedItemIndex + 1) : 0;
			OnRobotPreview(index);
		}

		private void OnTextSearchStringUpdated(UserSearchParameters searchParameters)
		{
			_parameters.textFilter = searchParameters.textFilter;
		}

		private void OnSearchParametersUpdated(UserSearchParameters searchParameters)
		{
			_parameters.sortMode = searchParameters.sortByMode;
			if (searchParameters.partCategoryIndex != 0)
			{
				ItemCategory itemCategory = (ItemCategory)(_partCategoryKey = (int)_partCategoryList[(int)(searchParameters.partCategoryIndex - 1)]);
			}
			else
			{
				_partCategoryKey = 0;
			}
			_parameters.movementCategoryGroups = CreateItemCategoryDependency(ItemType.Movement);
			_parameters.weaponCategoryGroups = CreateItemCategoryDependency(ItemType.Weapon);
			uint tierIndex = searchParameters.robotTierIndex - 1;
			_parameters.minimumCpu = -1;
			_parameters.maximumCpu = -1;
			_parameters.minRobotRanking = -1;
			_parameters.maxRobotRanking = -1;
			if (RRAndTiers.IsMegabotTier(tierIndex, _tiersData))
			{
				_parameters.minimumCpu = (int)cpuPower.MaxCpuPower;
			}
			else if (searchParameters.robotTierIndex != 0)
			{
				_parameters.minRobotRanking = Math.Max(1, (int)RRAndTiers.GetTierLowerRRLimit(tierIndex, _tiersData));
				_parameters.maxRobotRanking = (int)RRAndTiers.GetTierUpperRRLimit(tierIndex, _tiersData);
			}
			_parameters.noFiltersSelected = searchParameters.NoFiltersSelected;
			_parameters.playerFilter = searchParameters.showMyRobots;
			_parameters.showFeaturedRobots = searchParameters.showFeaturedOnly;
			_parameters.showLockedRobots = searchParameters.showLockedBots;
			_parameters.devOnlyShowHiddenRobots = searchParameters.devOnlyShowHiddenRobots;
			_parameters.textFilter = searchParameters.textFilter;
			_parameters.textSearchField = searchParameters.textSearchField;
			RestartSearch();
		}

		private static bool IsCategoryOfValidType(ItemCategory category, ItemType itemType)
		{
			switch (itemType)
			{
			case ItemType.Movement:
				return category >= ItemCategory.Wheel && category <= ItemCategory.Propeller;
			case ItemType.Weapon:
				return category >= ItemCategory.Laser && category <= ItemCategory.Chaingun;
			default:
				return false;
			}
		}

		private bool IsFilterByAllPartsOfCategory()
		{
			return _partCategoryKey != 0;
		}

		private string CreateItemCategoryDependency(ItemType itemType)
		{
			string result = "NONE";
			if (IsFilterByAllPartsOfCategory() && IsCategoryOfValidType((ItemCategory)_partCategoryKey, itemType))
			{
				result = (_partCategoryKey * 100000).ToString();
			}
			return result;
		}

		private void RestartSearch()
		{
			if (_view.get_gameObject().get_activeInHierarchy())
			{
				InitSearch(showTopRobots: false);
			}
		}

		private void OnRemovePlayerShopSubmissionRequested()
		{
			loadingPresenter.NotifyLoading("RobotShopLoadingScreen");
			int id = _items.get_Item(_requestedItemIndex).robotShopItem.id;
			IRemovePlayerShopSubmission removePlayerShopSubmission = serviceFactory.Create<IRemovePlayerShopSubmission, int>(id);
			removePlayerShopSubmission.SetAnswer(new ServiceAnswer<LoadShopEarningsResponse>(OnRemovePlayerShopSubmissionDone, OnOperationFailed));
			removePlayerShopSubmission.Execute();
		}

		private void OnRemovePlayerShopSubmissionDone(LoadShopEarningsResponse response)
		{
			HandleCommunityShopEarningsCollected(response);
			observer.OnRobotDeleted();
			RestartSearch();
		}

		private void OnReportAbuseRequested(string reason)
		{
			loadingPresenter.NotifyLoading("RobotShopLoadingScreen");
			ReportCommunityShopItemDependency reportCommunityShopItemDependency = new ReportCommunityShopItemDependency();
			reportCommunityShopItemDependency.itemId = _items.get_Item(_requestedItemIndex).robotShopItem.id;
			reportCommunityShopItemDependency.reason = reason;
			IReportCommunityShopItemRequest reportCommunityShopItemRequest = serviceFactory.Create<IReportCommunityShopItemRequest, ReportCommunityShopItemDependency>(reportCommunityShopItemDependency);
			reportCommunityShopItemRequest.SetAnswer(new ServiceAnswer(OnReportDone, delegate
			{
				OnReportDone();
			}));
			reportCommunityShopItemRequest.Execute();
		}

		private void OnReportDone()
		{
			loadingPresenter.NotifyLoadingDone("RobotShopLoadingScreen");
			RestartSearch();
			observer.OnHideRobot(refreshList: true);
		}

		private void RefreshFilterStrings()
		{
			_view.RefreshFilterStrings();
		}
	}
}
