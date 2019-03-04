using System;
using UnityEngine;

namespace Robocraft.GUI
{
	[RequireComponent(typeof(UIToggle))]
	[RequireComponent(typeof(BoxCollider))]
	public class UITickBoxAdapter : MonoBehaviour
	{
		private UIToggle _toggleContainer;

		private BoxCollider _collider;

		private UIButton[] _buttons;

		public Action<bool> OnTickedStateChanged;

		private bool _isTicked;

		public UITickBoxAdapter()
			: this()
		{
		}

		public void Setup()
		{
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Expected O, but got Unknown
			_toggleContainer = this.GetComponentInChildren<UIToggle>();
			_collider = this.GetComponent<BoxCollider>();
			_buttons = this.GetComponentsInChildren<UIButton>();
			_isTicked = _toggleContainer.get_value();
			_toggleContainer.onChange.Add(new EventDelegate(this, "OnTickStateChangedDelegate"));
		}

		public void Enable()
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Invalid comparison between Unknown and I4
			_collider.set_enabled(true);
			int num = 0;
			for (num = 0; num < _buttons.Length; num++)
			{
				UIButton val = _buttons[num];
				if ((int)val.get_state() == 3)
				{
					val.set_state(0);
				}
			}
		}

		public void Disable()
		{
			_collider.set_enabled(false);
			int num = 0;
			for (num = 0; num < _buttons.Length; num++)
			{
				UIButton val = _buttons[num];
				val.set_state(3);
			}
		}

		public void OnTickStateChangedDelegate()
		{
			if (_isTicked != _toggleContainer.get_value())
			{
				OnTickedStateChanged(_toggleContainer.get_value());
			}
		}

		public void SetTickState(bool setting)
		{
			_isTicked = setting;
			_toggleContainer.Set(setting, true);
		}
	}
}
