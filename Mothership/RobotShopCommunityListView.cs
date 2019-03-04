using Svelto.DataStructures;
using Svelto.Factories;
using Svelto.IoC;
using Svelto.UI.Comms.SignalChain;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Mothership
{
	internal sealed class RobotShopCommunityListView : MonoBehaviour, IInitialize, IChainListener
	{
		public UIPanel slidingContents;

		public UISlider scrollBar;

		public UISliderPanelMouseWheelScroller wheelScroller;

		public UIWidget listItemGameObject;

		public GameObject moreResultButton;

		public GameObject sortByFilterGO;

		public GameObject robotTierGO;

		public GameObject partCategoryFilterGO;

		public GameObject searchFieldFilterGO;

		public GameObject ShowMyBotsCheckBox;

		public GameObject ShowFeaturedBotsCheckBox;

		public GameObject ShowLockedBotsCheckBox;

		public GameObject DevOnlyShowHiddenBotsCheckBox;

		public GameObject DevOnlyShowHiddenBotsFilter;

		public GameObject filterPanel;

		public UIButton clearFilterButton;

		public UILabel earningsLabel;

		public CollectedEarningsPopup collectedEarningsPopup;

		public UIButton earningButton;

		public UIInputWithFocusEvents searchLabelInput;

		public UILabel searchLabel;

		public float outerMargin = 5f;

		public float innerMargin = 15f;

		public bool isCentered;

		private const float ITEM_SHIFT_PER_SCROLL = 0.33f;

		private const float MIN_OFFSET = 0f;

		public Action OnClaimCommunityShopEarningsEvent;

		private IRobotShopFilterView _sortByFilter;

		private IRobotShopFilterView _robotTierFilter;

		private IRobotShopFilterView _partCategoryFilter;

		private IRobotShopFilterView _searchFieldFilter;

		private Vector2 _itemSize;

		private Vector2 _lastPanelSize;

		private int _prevStartItemIndex = -1;

		private int _prevStartItemViewIndex = -1;

		private UIPanel _parentPanel;

		private FasterList<CRFItem> _items;

		private List<RobotShopCommunityItemView> _itemViews = new List<RobotShopCommunityItemView>();

		private UserSearchParameters _searchParameters;

		private float _sliderPos;

		private float _prevOffset;

		private bool _showMore;

		private bool _lockLayout;

		private InitScrollBarPhase _scrollBarInitPhase;

		[Inject]
		internal RobotShopController robotShop
		{
			private get;
			set;
		}

		[Inject]
		internal RobotShopCommunityController controller
		{
			private get;
			set;
		}

		[Inject]
		internal RobotThumbnailFetcher thumbnailsFetcher
		{
			private get;
			set;
		}

		[Inject]
		internal IGUIInputControllerMothership guiInputController
		{
			private get;
			set;
		}

		[Inject]
		internal IGameObjectFactory gameObjectFactory
		{
			private get;
			set;
		}

		internal event Action<int> OnRobotPreviewEvent;

		internal event Action<UserSearchParameters> OnSearchParametersUpdatedEvent;

		internal event Action<UserSearchParameters> OnTextSearchStringUpdatedEvent;

		internal event Action ShowMoreEvent;

		public RobotShopCommunityListView()
			: this()
		{
		}

		void IInitialize.OnDependenciesInjected()
		{
			_sortByFilter = sortByFilterGO.GetComponent<IRobotShopFilterView>();
			_robotTierFilter = robotTierGO.GetComponent<IRobotShopFilterView>();
			_partCategoryFilter = partCategoryFilterGO.GetComponent<IRobotShopFilterView>();
			_searchFieldFilter = searchFieldFilterGO.GetComponent<IRobotShopFilterView>();
			controller.SetupView(this);
		}

		private unsafe void Start()
		{
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Expected O, but got Unknown
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Expected O, but got Unknown
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Expected O, but got Unknown
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Expected O, but got Unknown
			Vector3[] localCorners = listItemGameObject.get_localCorners();
			_itemSize.x = localCorners[2].x - localCorners[1].x;
			_itemSize.y = localCorners[1].y - localCorners[0].y;
			listItemGameObject.get_gameObject().SetActive(false);
			_parentPanel = slidingContents.get_parent().GetComponent<UIPanel>();
			moreResultButton.SetActive(false);
			searchLabelInput.GetComponent<UIButton>().onClick.Add(new EventDelegate(new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/)));
			searchLabelInput.onSubmit.Add(new EventDelegate(new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/)));
			searchLabelInput.OnInputLoseFocus += OnSearchLabelLoseFocus;
			_searchParameters = new UserSearchParameters();
			ShowMyBotsCheckBox.SetActive(false);
			ShowFeaturedBotsCheckBox.SetActive(_searchParameters.showFeaturedOnly);
			ShowLockedBotsCheckBox.SetActive(_searchParameters.showLockedBots);
			DevOnlyShowHiddenBotsFilter.SetActive(robotShop.IsDeveloper());
			DevOnlyShowHiddenBotsCheckBox.SetActive(false);
			UpdateClearFilterState();
		}

		internal void InitialiseFilterString(RobotShopFilter filter, FasterList<string> filterStrings)
		{
			switch (filter)
			{
			case RobotShopFilter.RobotTier:
				_robotTierFilter.InitList(RobotShopFilter.RobotTier, filterStrings, gameObjectFactory, OnRobotTierUpdated, CloseFilterLists, 0u);
				break;
			case RobotShopFilter.PartCategories:
				_partCategoryFilter.InitList(RobotShopFilter.PartCategories, filterStrings, gameObjectFactory, OnPartCategoryUpdate, CloseFilterLists, 0u);
				break;
			case RobotShopFilter.SearchBy:
				_searchFieldFilter.InitList(RobotShopFilter.SearchBy, filterStrings, gameObjectFactory, OnSearchFieldFilterUpdated, CloseFilterLists, 0u);
				break;
			case RobotShopFilter.SortBy:
				_sortByFilter.InitList(RobotShopFilter.SortBy, filterStrings, gameObjectFactory, OnSortByUpdated, CloseFilterLists, 0u);
				break;
			}
		}

		internal void LoadEarnings(LoadShopEarningsResponse earnings)
		{
			earningButton.set_isEnabled(earnings.earnings != 0);
			if (earnings.earnings != 0)
			{
				earningButton.SetState(0, true);
			}
			else
			{
				earningButton.SetState(3, true);
			}
			earningsLabel.set_text(earnings.earnings.ToString("N0"));
		}

		internal void RefreshFilterStrings()
		{
			_sortByFilter.UpdateFilterStrings();
			_searchFieldFilter.UpdateFilterStrings();
			_partCategoryFilter.UpdateFilterStrings();
			_robotTierFilter.UpdateFilterStrings();
		}

		internal void Show()
		{
			wheelScroller.SetEnabled(!filterPanel.get_activeSelf());
		}

		internal void Hide()
		{
			_sortByFilter.HideView();
			_robotTierFilter.HideView();
			_partCategoryFilter.HideView();
			_searchFieldFilter.HideView();
			filterPanel.SetActive(false);
			wheelScroller.SetEnabled(enabled: false);
			ClearAllFilters();
		}

		private IRobotShopFilterView GetFilterView(RobotShopFilter robotShopFilterView)
		{
			switch (robotShopFilterView)
			{
			case RobotShopFilter.SearchBy:
				return _searchFieldFilter;
			case RobotShopFilter.SortBy:
				return _sortByFilter;
			case RobotShopFilter.PartCategories:
				return _partCategoryFilter;
			case RobotShopFilter.RobotTier:
				return _robotTierFilter;
			default:
				Console.LogError("Invalid RobotShopFilter Filter: " + robotShopFilterView.ToString());
				return null;
			}
		}

		private void OnRobotTierUpdated(uint robotTierIndex)
		{
			_searchParameters.robotTierIndex = robotTierIndex;
			OnSearchParametersUpdated();
			UpdateClearFilterState();
		}

		private void OnPartCategoryUpdate(uint partCategoryIndex)
		{
			_searchParameters.partCategoryIndex = partCategoryIndex;
			OnSearchParametersUpdated();
			UpdateClearFilterState();
		}

		private void OnSortByUpdated(uint sortModeIndex)
		{
			_searchParameters.sortByMode = (ItemSortMode)Enum.GetValues(typeof(ItemSortMode)).GetValue(sortModeIndex);
			OnSearchParametersUpdated();
			UpdateClearFilterState();
		}

		private void OnSearchFieldFilterUpdated(uint searchFieldIndex)
		{
			_searchParameters.textSearchField = (TextSearchField)searchFieldIndex;
			OnSearchParametersUpdated();
			UpdateClearFilterState();
		}

		private void OnSearchLabelUpdated()
		{
			searchLabelInput.RemoveFocus();
			_searchParameters.textFilter = searchLabelInput.get_value();
			guiInputController.SetShortCutMode(ShortCutMode.AllShortCuts);
			UpdateClearFilterState();
			OnSearchParametersUpdated();
		}

		private void OnSearchLabelLoseFocus()
		{
			_searchParameters.textFilter = searchLabelInput.get_value();
			guiInputController.SetShortCutMode(ShortCutMode.AllShortCuts);
			UpdateClearFilterState();
			OnTextSearchStringUpdated();
		}

		private void Update()
		{
			if (PreAllocateItemsIfNeeded())
			{
				LayoutItems();
			}
			switch (_scrollBarInitPhase)
			{
			case InitScrollBarPhase.Uninitialized:
				scrollBar.get_foregroundWidget().set_enabled(true);
				_scrollBarInitPhase = InitScrollBarPhase.AnchorsSet;
				break;
			case InitScrollBarPhase.AnchorsSet:
				scrollBar.ForceUpdate();
				_scrollBarInitPhase = InitScrollBarPhase.Done;
				break;
			}
		}

		internal void DisplayItems(FasterList<CRFItem> items, bool showMore)
		{
			PreAllocateItemsIfNeeded();
			if (items != _items)
			{
				_prevStartItemIndex = -1;
				_prevStartItemViewIndex = -1;
				SetScrollBarValue(0f);
				_prevOffset = 0f;
			}
			_items = items;
			_showMore = showMore;
			LayoutItems();
		}

		public void SlidePanel()
		{
			if (!_lockLayout)
			{
				_sliderPos = UIProgressBar.current.get_value();
				_prevOffset = -1f;
				LayoutItems();
			}
		}

		private bool PreAllocateItemsIfNeeded()
		{
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			if (_lastPanelSize.x == slidingContents.get_width() && _lastPanelSize.y == slidingContents.get_height())
			{
				return false;
			}
			Vector3[] localCorners = slidingContents.get_localCorners();
			Vector3 val = localCorners[1];
			Vector3 val2 = localCorners[3];
			float num = val2.x - val.x;
			float num2 = val.y - val2.y;
			float num3 = num - (float)scrollBar.get_backgroundWidget().get_width() - 2f * outerMargin;
			float num4 = num2 - 2f * outerMargin;
			int num5 = Mathf.Max(1, (int)(num3 / (_itemSize.x + innerMargin)));
			int num6 = Mathf.Max(1, (int)(num4 / (_itemSize.y + innerMargin)));
			int num7 = num5 * (2 + num6);
			if (_itemViews.Count < num7)
			{
				int num8 = num7 - _itemViews.Count;
				for (int i = 0; i < num8; i++)
				{
					GameObject val3 = Object.Instantiate<GameObject>(listItemGameObject.get_gameObject());
					val3.get_transform().SetParent(slidingContents.get_transform());
					RobotShopCommunityItemView component = val3.GetComponent<RobotShopCommunityItemView>();
					_itemViews.Add(component);
				}
			}
			else if (num7 < _itemViews.Count)
			{
				int num9 = _itemViews.Count - num7;
				for (int j = 0; j < num9; j++)
				{
					Object.Destroy(_itemViews[_itemViews.Count - 1].get_gameObject());
					_itemViews.RemoveAt(_itemViews.Count - 1);
				}
			}
			_lastPanelSize.x = slidingContents.get_width();
			_lastPanelSize.y = slidingContents.get_height();
			return true;
		}

		private void LayoutItems()
		{
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0287: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0537: Unknown result type (might be due to invalid IL or missing references)
			//IL_0545: Unknown result type (might be due to invalid IL or missing references)
			//IL_0563: Unknown result type (might be due to invalid IL or missing references)
			//IL_057c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0581: Unknown result type (might be due to invalid IL or missing references)
			//IL_0586: Unknown result type (might be due to invalid IL or missing references)
			//IL_0629: Unknown result type (might be due to invalid IL or missing references)
			//IL_063b: Unknown result type (might be due to invalid IL or missing references)
			if (_items == null)
			{
				for (int i = 0; i < _itemViews.Count; i++)
				{
					_itemViews[i].get_gameObject().SetActive(false);
				}
				return;
			}
			Vector3[] localCorners = slidingContents.get_localCorners();
			Vector3 val = localCorners[1];
			Vector3 val2 = localCorners[3];
			float num = val2.x - val.x;
			float num2 = val.y - val2.y;
			float num3 = num - (float)scrollBar.get_backgroundWidget().get_width() - 2f * outerMargin;
			float num4 = num2 - 2f * outerMargin;
			int num5 = Mathf.Max(1, (int)(num3 / (_itemSize.x + innerMargin)));
			int num6 = _items.get_Count() / num5;
			if (_items.get_Count() % num5 != 0)
			{
				num6++;
			}
			if (_showMore)
			{
				num6++;
			}
			float num7 = (num6 != 0) ? ((float)num6 * (_itemSize.y + innerMargin) - innerMargin) : 0f;
			bool flag = num7 <= num4;
			int num8 = 0;
			float num9 = 0f;
			if (!flag)
			{
				float num10 = 0f;
				float num11 = num7 + 2f * outerMargin;
				float num12 = num11 - num2;
				float num13 = 0f;
				if (_prevOffset < 0f)
				{
					num13 = num10 + (num12 - num10) * _sliderPos;
				}
				else
				{
					num13 = _prevOffset;
					SetScrollBarValue((num13 - num10) / (num12 - num10));
				}
				_prevOffset = num13;
				if (num13 < outerMargin)
				{
					num9 = num13;
				}
				else
				{
					int num14 = (int)(num13 / (_itemSize.y + innerMargin));
					num9 = num13 - (float)num14 * (_itemSize.y + innerMargin);
					num8 = num14 * num5;
					while (_items.get_Count() <= num8)
					{
						num8 -= num5;
					}
				}
				float num15 = num4 / (_itemSize.y + innerMargin);
				float num16 = (float)num6 - num15;
				float scrollSpeed = 1f / num16 / 0.33f;
				wheelScroller.scrollSpeed = scrollSpeed;
			}
			Vector3 val3 = slidingContents.get_localCorners()[1];
			if (isCentered)
			{
				val3.x += (slidingContents.get_width() - outerMargin * 0.5f - (_itemSize.x + innerMargin) * (float)num5) * 0.5f;
			}
			else
			{
				val3.x += outerMargin;
			}
			val3.y -= outerMargin;
			val3.y += num9;
			val3.x += _itemSize.x * 0.5f;
			val3.y -= _itemSize.y * 0.5f;
			float x = val3.x;
			int count = _itemViews.Count;
			int j = 0;
			if (_prevStartItemIndex != -1 && Mathf.Abs(num8 - _prevStartItemIndex) < count)
			{
				j = _prevStartItemViewIndex;
				for (j += num8 - _prevStartItemIndex; j < 0; j += count)
				{
				}
				while (count <= j)
				{
					j -= count;
				}
			}
			int k = 0;
			if (_items.get_Count() != 0)
			{
				_prevStartItemIndex = num8;
				_prevStartItemViewIndex = j;
				do
				{
					RobotShopCommunityItemView itemView = _itemViews[j];
					CRFItem item = _items.get_Item(num8);
					GameObject gameObject = itemView.get_gameObject();
					if (itemView.crfItemData != item)
					{
						itemView.SetItemData(item, _searchParameters.sortByMode, this.get_transform(), num8, onRobotExpired, inventoryShowsLockStateInsteadOfCount: true);
						if (item.thumbnail != null)
						{
							itemView.DisplayTexture(item.thumbnail);
						}
						else
						{
							string thumbnailURL = item.robotShopItem.thumbnailURL;
							if (string.IsNullOrEmpty(thumbnailURL))
							{
								Console.LogError("Missing CRF thumbnail url for " + item.robotShopItem.name);
								item.thumbnail = null;
								itemView.DisplayTexture(null);
							}
							else
							{
								thumbnailsFetcher.LoadTexture(thumbnailURL, delegate(Texture2D texture)
								{
									item.thumbnail = texture;
									if (itemView.crfItemData == item)
									{
										itemView.DisplayTexture(texture);
									}
								});
							}
						}
					}
					gameObject.SetActive(true);
					gameObject.get_transform().set_localPosition(val3);
					gameObject.get_transform().set_localScale(Vector3.get_one());
					if (k % num5 == num5 - 1)
					{
						val3 -= new Vector3(0f, _itemSize.y + innerMargin);
						val3.x = x;
					}
					else
					{
						val3.x += _itemSize.x;
						val3.x += innerMargin;
					}
					k++;
					j = (j + 1) % count;
					if (k == count)
					{
						break;
					}
					num8++;
				}
				while (num8 != _items.get_Count());
			}
			moreResultButton.SetActive(_showMore);
			if (_showMore)
			{
				moreResultButton.get_transform().set_localPosition(val3);
				moreResultButton.get_transform().set_localScale(Vector3.get_one());
			}
			for (; k < count; k++)
			{
				_itemViews[j].get_gameObject().SetActive(false);
				j = (j + 1) % count;
			}
			_parentPanel.SetDirty();
		}

		private void onRobotExpired()
		{
			OnSearchParametersUpdated();
		}

		private void SetScrollBarValue(float value)
		{
			_lockLayout = true;
			scrollBar.set_value(value);
			_sliderPos = value;
			_lockLayout = false;
		}

		public void Listen(object message)
		{
			wheelScroller.SetEnabled(!filterPanel.get_activeSelf());
			if (message is int)
			{
				int obj = (int)message;
				this.OnRobotPreviewEvent(obj);
			}
			else
			{
				if (!(message is ButtonType))
				{
					return;
				}
				switch ((ButtonType)message)
				{
				case ButtonType.ShowMoreRobot:
					if (this.ShowMoreEvent != null)
					{
						this.ShowMoreEvent();
					}
					break;
				case ButtonType.RobotShopShowMyBots:
					_searchParameters.showMyRobots = !_searchParameters.showMyRobots;
					ShowMyBotsCheckBox.SetActive(_searchParameters.showMyRobots);
					UpdateClearFilterState();
					OnSearchParametersUpdated();
					break;
				case ButtonType.CRFFilterShowFeatured:
					_searchParameters.showFeaturedOnly = !_searchParameters.showFeaturedOnly;
					ShowFeaturedBotsCheckBox.SetActive(_searchParameters.showFeaturedOnly);
					UpdateClearFilterState();
					OnSearchParametersUpdated();
					break;
				case ButtonType.RobotShopHideLockedBots:
					_searchParameters.showLockedBots = !_searchParameters.showLockedBots;
					ShowLockedBotsCheckBox.SetActive(_searchParameters.showLockedBots);
					UpdateClearFilterState();
					OnSearchParametersUpdated();
					break;
				case ButtonType.DevOnlyCRFFilterShowHidden:
					_searchParameters.devOnlyShowHiddenRobots = !_searchParameters.devOnlyShowHiddenRobots;
					DevOnlyShowHiddenBotsCheckBox.SetActive(_searchParameters.devOnlyShowHiddenRobots);
					UpdateClearFilterState();
					OnSearchParametersUpdated();
					break;
				case ButtonType.ClaimCommunityShopEarnings:
					if (OnClaimCommunityShopEarningsEvent != null)
					{
						OnClaimCommunityShopEarningsEvent();
					}
					break;
				case ButtonType.Close:
					ClosePopups();
					break;
				case ButtonType.RobotShopClearFilter:
					ClearAllFilters();
					break;
				case ButtonType.RobotShopShowFilter:
					filterPanel.SetActive(true);
					wheelScroller.SetEnabled(enabled: false);
					break;
				case ButtonType.RobotShopHideFilter:
					filterPanel.SetActive(false);
					wheelScroller.SetEnabled(enabled: true);
					break;
				}
			}
		}

		public void ResetFilter(RobotShopFilter filter)
		{
			GetFilterView(filter)?.Reset();
		}

		public void ShowCollectedEarningsPopup(int buyCount, int earnings)
		{
			collectedEarningsPopup.Show(buyCount, earnings);
		}

		private void ClearAllFilters()
		{
			_searchParameters = null;
			_partCategoryFilter.Reset();
			_robotTierFilter.Reset();
			_searchFieldFilter.Reset();
			_sortByFilter.Reset();
			_searchParameters = new UserSearchParameters();
			ShowMyBotsCheckBox.SetActive(_searchParameters.showMyRobots);
			ShowFeaturedBotsCheckBox.SetActive(_searchParameters.showFeaturedOnly);
			DevOnlyShowHiddenBotsCheckBox.SetActive(_searchParameters.devOnlyShowHiddenRobots);
			searchLabelInput.set_value(_searchParameters.textFilter);
			OnSearchParametersUpdated();
			UpdateClearFilterState();
		}

		private void UpdateClearFilterState()
		{
			clearFilterButton.set_isEnabled(_partCategoryFilter.GetValue() != 0 || _robotTierFilter.GetValue() != 0 || _searchFieldFilter.GetValue() != 0 || _sortByFilter.GetValue() != 0 || ShowMyBotsCheckBox.get_activeSelf() || ShowFeaturedBotsCheckBox.get_activeSelf() || ShowLockedBotsCheckBox.get_activeSelf() || DevOnlyShowHiddenBotsCheckBox.get_activeSelf() || searchLabelInput.get_value().Length > 0);
		}

		private void CloseFilterLists(RobotShopFilter filter, bool filterOpened)
		{
			if (filterOpened)
			{
				if (filter != RobotShopFilter.SearchBy)
				{
					_searchFieldFilter.HideView();
				}
				if (filter != RobotShopFilter.SortBy)
				{
					_sortByFilter.HideView();
				}
				if (filter != RobotShopFilter.PartCategories)
				{
					_partCategoryFilter.HideView();
				}
				if (filter != RobotShopFilter.RobotTier)
				{
					_robotTierFilter.HideView();
				}
			}
		}

		private void ClosePopups()
		{
			collectedEarningsPopup.Hide();
		}

		private void OnSearchParametersUpdated()
		{
			if (_searchParameters != null && this.OnSearchParametersUpdatedEvent != null)
			{
				this.OnSearchParametersUpdatedEvent(_searchParameters);
			}
		}

		private void OnTextSearchStringUpdated()
		{
			if (_searchParameters != null && this.OnTextSearchStringUpdatedEvent != null)
			{
				this.OnTextSearchStringUpdatedEvent(_searchParameters);
			}
		}
	}
}
