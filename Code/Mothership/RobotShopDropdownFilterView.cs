using Svelto.DataStructures;
using Svelto.Factories;
using System;
using UnityEngine;

namespace Mothership
{
	internal class RobotShopDropdownFilterView : MonoBehaviour, IRobotShopFilterView
	{
		public UILabel selectedLabel;

		public UIPopupList popupList;

		private FasterList<string> _filterStringKeys = new FasterList<string>();

		private uint _defaultSelectedIndex;

		private uint _currentIndex;

		private Action<uint> _onFilterUpdated;

		public RobotShopDropdownFilterView()
			: this()
		{
		}

		public unsafe void InitList(RobotShopFilter robotShopFilter, FasterList<string> filterStringKeys, IGameObjectFactory gameObjectFactory, Action<uint> filterUpdatedCallback, Action<RobotShopFilter, bool> filterClickedCallback, uint defaultSelectedIndex)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Expected O, but got Unknown
			_filterStringKeys = filterStringKeys;
			_onFilterUpdated = filterUpdatedCallback;
			EventDelegate.Add(popupList.onChange, new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			_defaultSelectedIndex = defaultSelectedIndex;
			_currentIndex = _defaultSelectedIndex;
			UpdateFilterStrings(filterStringKeys);
			DisplaySelectedItem(_defaultSelectedIndex);
		}

		public void UpdateFilterStrings(FasterList<string> filterStringKeys = null)
		{
			popupList.Clear();
			if (filterStringKeys != null)
			{
				_filterStringKeys = filterStringKeys;
			}
			for (int i = 0; i < _filterStringKeys.get_Count(); i++)
			{
				string @string = StringTableBase<StringTable>.Instance.GetString(_filterStringKeys.get_Item(i));
				popupList.AddItem(@string, (object)i);
			}
			DisplaySelectedItem(_currentIndex);
		}

		public void HideView()
		{
		}

		public uint GetValue()
		{
			return _currentIndex;
		}

		public void Reset()
		{
			_currentIndex = _defaultSelectedIndex;
			DisplaySelectedItem(_currentIndex);
		}

		private void DisplaySelectedItem(uint index)
		{
			selectedLabel.set_text(StringTableBase<StringTable>.Instance.GetString(_filterStringKeys.get_Item((int)index)).ToUpper());
		}

		private void OnItemSelected()
		{
			uint num = (uint)(int)popupList.get_data();
			if (num != _currentIndex && _onFilterUpdated != null)
			{
				_currentIndex = num;
				_onFilterUpdated(num);
				DisplaySelectedItem(num);
			}
		}
	}
}
