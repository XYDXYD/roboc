using System;
using UnityEngine;

namespace Robocraft.GUI
{
	[RequireComponent(typeof(UIPopupList))]
	public class UIPopUpListAdapter : MonoBehaviour
	{
		public Action<int> OnValueChanged = delegate
		{
		};

		private UIPopupList _popUpList;

		private string _lastSelection = string.Empty;

		public UIPopUpListAdapter()
			: this()
		{
		}

		public unsafe void Setup()
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Expected O, but got Unknown
			_popUpList = this.GetComponent<UIPopupList>();
			EventDelegate.Add(_popUpList.onChange, new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		public void SetCurrentSelection(string currentValue)
		{
			if (_lastSelection != currentValue)
			{
				_popUpList.Set(currentValue, true);
				_lastSelection = currentValue;
			}
		}

		private void OnValueChange()
		{
			if (_lastSelection != _popUpList.get_value() && UIPopupList.get_isOpen())
			{
				OnValueChanged((int)_popUpList.get_data());
				_lastSelection = _popUpList.get_value();
			}
		}
	}
}
