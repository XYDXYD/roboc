using System;
using UnityEngine;

namespace Robocraft.GUI
{
	[RequireComponent(typeof(UIButton))]
	public class UIButtonAdapter : MonoBehaviour
	{
		private UIButton[] _buttons;

		private bool _hide;

		public Action OnClickHandler;

		public Action OnEnterHandler;

		public Action OnLeaveHandler;

		public UIButtonAdapter()
			: this()
		{
		}

		public void Setup()
		{
			_buttons = this.GetComponents<UIButton>();
		}

		public void Disable()
		{
			for (int i = 0; i < _buttons.Length; i++)
			{
				_buttons[i].set_isEnabled(false);
			}
		}

		public void Enable()
		{
			for (int i = 0; i < _buttons.Length; i++)
			{
				_buttons[i].set_isEnabled(true);
			}
		}

		public void OnClick()
		{
			OnClickHandler();
		}

		private void OnHover(bool isOver)
		{
			if (isOver)
			{
				if (OnEnterHandler != null)
				{
					OnEnterHandler();
				}
			}
			else if (OnLeaveHandler != null)
			{
				OnLeaveHandler();
			}
		}
	}
}
