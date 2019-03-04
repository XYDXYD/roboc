using Mothership.GUI.Social;
using Robocraft.GUI.Iteration2;
using Svelto.DataStructures;
using Svelto.Factories;
using Svelto.UI.Comms.SignalChain;
using System;
using UnityEngine;

namespace Mothership
{
	internal class RobotShopFilterView : MonoBehaviour, IChainListener, IRobotShopFilterView
	{
		public GameObject scrollableContainer;

		public GameObject listElement;

		private UIScrollView _uiScrollView;

		private UIWidget _thisUIWidget;

		private UIWidget _scrollUIWidget;

		private RobotShopFilter _robotShopFilter = RobotShopFilter.CPU;

		private FasterList<UILabel> _filterLabels = new FasterList<UILabel>();

		private FasterList<GameObject> _selectionGOs = new FasterList<GameObject>();

		private FasterList<UIWidget> _labelWidgets = new FasterList<UIWidget>();

		private FasterList<string> _filterStringKeys = new FasterList<string>();

		private UIScrollScrollView[] _scrollscrollViews;

		private UIScrollBar[] _scrollBars;

		private uint _currentIndex;

		private uint _previousIndex;

		private Action<uint> _onFilterUpdated;

		private Action<RobotShopFilter, bool> _onFilterClicked;

		private const int LABEL_HEIGHT_PADDING = 5;

		private const int LABEL_WIDTH_PADDING = 100;

		public RobotShopFilterView()
			: this()
		{
		}

		public void InitList(RobotShopFilter robotShopFilter, FasterList<string> filterStringKeys, IGameObjectFactory gameObjectFactory, Action<uint> filterUpdatedCallback, Action<RobotShopFilter, bool> filterClickedCallback, uint defaultSelectedIndex)
		{
			_filterStringKeys = filterStringKeys;
			_robotShopFilter = robotShopFilter;
			_onFilterUpdated = filterUpdatedCallback;
			_onFilterClicked = filterClickedCallback;
			_uiScrollView = scrollableContainer.GetComponentInChildren<UIScrollView>();
			UIGrid componentInChildren = scrollableContainer.GetComponentInChildren<UIGrid>();
			for (int i = 0; i < filterStringKeys.get_Count(); i++)
			{
				GameObject val = NGUITools.AddChild(componentInChildren.get_gameObject(), listElement);
				UIButtonBroadcasterUInt componentInChildren2 = val.GetComponentInChildren<UIButtonBroadcasterUInt>();
				componentInChildren2.listener = this.get_transform();
				componentInChildren2.sendValue = (uint)i;
				UILabel componentInChildren3 = val.GetComponentInChildren<UILabel>();
				val.SetActive(true);
				UISprite componentInChildren4 = componentInChildren3.get_transform().get_parent().GetComponentInChildren<UISprite>(true);
				if (componentInChildren4 != null)
				{
					componentInChildren4.get_gameObject().SetActive(false);
					_selectionGOs.Add(componentInChildren4.get_gameObject());
				}
				UIScrollScrollView component = val.GetComponent<UIScrollScrollView>();
				component.scrollView = _uiScrollView;
				_labelWidgets.Add(val.GetComponent<UIWidget>());
				_filterLabels.Add(componentInChildren3);
			}
			_scrollUIWidget = scrollableContainer.GetComponent<UIWidget>();
			_scrollscrollViews = this.GetComponentsInChildren<UIScrollScrollView>(true);
			_scrollBars = this.GetComponentsInChildren<UIScrollBar>(true);
			SetupScrollSize(_filterStringKeys.get_Count());
			_currentIndex = defaultSelectedIndex;
			UpdateFilterStrings(filterStringKeys);
			DisplaySelectedItem(defaultSelectedIndex);
			_thisUIWidget = this.GetComponent<UIWidget>();
			componentInChildren.set_repositionNow(true);
			scrollableContainer.SetActive(true);
		}

		private void SetupScrollSize(int count)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			Vector2 localSize = _labelWidgets.get_Item(0).get_localSize();
			float y = localSize.y;
			float num = (float)count * (y + 5f);
			EnableScroll(num > (float)_scrollUIWidget.get_height());
			_uiScrollView.ResetPosition();
		}

		private void EnableScroll(bool enable)
		{
			for (int i = 0; i < _scrollscrollViews.Length; i++)
			{
				_scrollscrollViews[i].set_enabled(enable);
			}
			for (int j = 0; j < _scrollBars.Length; j++)
			{
				_scrollBars[j].set_barSize(1f);
				_scrollBars[j].set_enabled(enable);
			}
		}

		public void UpdateFilterStrings(FasterList<string> filterStringKeys = null)
		{
			int num = 0;
			if (filterStringKeys != null)
			{
				_filterStringKeys = filterStringKeys;
			}
			for (int i = 0; i < _filterLabels.get_Count(); i++)
			{
				if (_filterStringKeys.get_Count() > i)
				{
					_labelWidgets.get_Item(i).get_gameObject().SetActive(true);
					string @string = StringTableBase<StringTable>.Instance.GetString(_filterStringKeys.get_Item(i));
					_filterLabels.get_Item(i).set_text(@string);
					num++;
				}
				else
				{
					_labelWidgets.get_Item(i).get_gameObject().SetActive(false);
				}
			}
			SetupScrollSize(num);
			DisplaySelectedItem(_currentIndex);
		}

		public unsafe void HideView()
		{
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Expected O, but got Unknown
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Expected O, but got Unknown
			if (scrollableContainer != null)
			{
				if (scrollableContainer.get_activeSelf())
				{
					_onFilterClicked(_robotShopFilter, arg2: false);
				}
				UICamera.onSelect = Delegate.Remove((Delegate)UICamera.onSelect, (Delegate)new BoolDelegate((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			}
		}

		public uint GetValue()
		{
			return _currentIndex;
		}

		public void Reset()
		{
			_currentIndex = 0u;
			DisplaySelectedItem(_currentIndex);
		}

		public unsafe void ToggleListView()
		{
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Expected O, but got Unknown
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Expected O, but got Unknown
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Expected O, but got Unknown
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Expected O, but got Unknown
			_onFilterClicked(_robotShopFilter, scrollableContainer.get_activeSelf());
			if (scrollableContainer.get_activeSelf())
			{
				LayoutUtility.KeepWidgetOnTopOfParentAndInScreen(_thisUIWidget, _scrollUIWidget);
				UICamera.onSelect = Delegate.Combine((Delegate)UICamera.onSelect, (Delegate)new BoolDelegate((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			}
			else
			{
				UICamera.onSelect = Delegate.Remove((Delegate)UICamera.onSelect, (Delegate)new BoolDelegate((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			}
		}

		private void CheckIfClosePopup(GameObject go, bool selected)
		{
			if (selected && (go == null || !SocialStaticUtilities.IsParentOf(this.get_transform(), go.get_transform())))
			{
				HideView();
			}
		}

		void IChainListener.Listen(object message)
		{
			uint num = (uint)message;
			if (num != _currentIndex && _onFilterUpdated != null)
			{
				_currentIndex = num;
				_onFilterUpdated(num);
				DisplaySelectedItem(num);
			}
		}

		private void DisplaySelectedItem(uint index)
		{
			if (_selectionGOs.get_Count() > 0)
			{
				_selectionGOs.get_Item((int)_previousIndex).SetActive(false);
				_selectionGOs.get_Item((int)index).SetActive(true);
			}
			_previousIndex = index;
		}
	}
}
